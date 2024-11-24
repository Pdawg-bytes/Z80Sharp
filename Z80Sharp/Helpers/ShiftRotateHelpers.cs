using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using Z80Sharp.Helpers;
using static Z80Sharp.Registers.ProcessorRegisters;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte ShiftArith(byte operand, [ConstantExpected] BitDirection shiftDirection)
        {
            byte result = 0;
            byte carry = 0;

            switch (shiftDirection)
            {
                case BitDirection.Left:
                    carry = (byte)(operand >> 7);
                    result = (byte)(operand << 1);
                    break;
                case BitDirection.Right:
                    carry = (byte)(operand & 1);
                    result = (byte)((operand >> 1) | (operand & 0x80));
                    break;
            }

            UpdateShiftRotateFlags(result, carry);

            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte ShiftLogical(byte operand, [ConstantExpected] BitDirection shiftDirection)
        {
            int result = 0;
            byte carry = 0;

            switch (shiftDirection)
            {
                case BitDirection.Left:
                    carry = (byte)(operand >> 7);
                    result = (operand << 1) + 1;
                    break;
                case BitDirection.Right:
                    carry = (byte)(operand & 1);
                    result = (operand >> 1);
                    break;
            }

            Registers.SetFlagConditionally(FlagType.C, carry != 0);
            Registers.SetFlagConditionally(FlagType.PV, CheckParity((byte)result));
            Registers.SetFlagConditionally(FlagType.Z, (byte)result == 0);
            Registers.SetFlagConditionally(FlagType.S, (result & 0x80) != 0);
            Registers.F &= (byte)~(FlagType.N | FlagType.H);

            return (byte)result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte RotateCircular(byte operand, [ConstantExpected] BitDirection shiftDirection)
        {
            int result = 0;
            byte carry = 0;

            switch (shiftDirection)
            {
                case BitDirection.Left:
                    carry = (byte)((operand & 0x80) >> 7);
                    result = operand << 1;
                    Registers.SetFlagConditionally(FlagType.PV, CheckParity((byte)(result | carry)));
                    break;
                case BitDirection.Right:
                    carry = (byte)(operand & 1);
                    result = (operand >> 1) | (carry << 7);
                    Registers.SetFlagConditionally(FlagType.PV, CheckParity((byte)result));
                    break;
            }

            Registers.SetFlagConditionally(FlagType.C, carry != 0);
            Registers.SetFlagConditionally(FlagType.Z, (byte)result == 0);
            Registers.SetFlagConditionally(FlagType.S, (result & 0x80) != 0);
            Registers.F &= (byte)~(FlagType.N | FlagType.H);

            return (byte)result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte RotateThroughCarry(byte operand, [ConstantExpected] BitDirection shiftDirection)
        {
            int result = 0;
            byte operandCarry = 0;
            byte carry = (byte)(Registers.F & 1);

            switch (shiftDirection)
            {
                case BitDirection.Left:
                    operandCarry = (byte)((operand & 0x80) >> 7);
                    result = (operand << 1) | operandCarry;
                    Registers.SetFlagConditionally(FlagType.PV, CheckParity((byte)(result | carry)));
                    break;
                case BitDirection.Right:
                    operandCarry = (byte)(operand & 1);
                    result = (operand >> 1) | (operandCarry << 7);
                    Registers.SetFlagConditionally(FlagType.PV, CheckParity((byte)result));
                    break;
            }

            Registers.SetFlagConditionally(FlagType.C, operandCarry != 0);
            Registers.SetFlagConditionally(FlagType.Z, (byte)result == 0);
            Registers.SetFlagConditionally(FlagType.S, (result & 0x80) != 0);
            Registers.F &= (byte)~(FlagType.N | FlagType.H);

            return (byte)result;
        }
    }
}