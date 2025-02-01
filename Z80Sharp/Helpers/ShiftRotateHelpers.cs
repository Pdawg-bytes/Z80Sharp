using Z80Sharp.Enums;
using System.Diagnostics.CodeAnalysis;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
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

        private byte ShiftLogical(byte operand, [ConstantExpected] BitDirection shiftDirection)
        {
            byte result = 0;
            byte carry = 0;

            switch (shiftDirection)
            {
                case BitDirection.Left:
                    carry = (byte)(operand >> 7);
                    result = (byte)(operand << 1 | 1);
                    break;
                case BitDirection.Right:
                    carry = (byte)(operand & 1);
                    result = (byte)(operand >> 1);
                    break;
            }

            UpdateShiftRotateFlags(result, carry);

            return result;
        }


        private byte RotateCircular(byte operand, [ConstantExpected] BitDirection shiftDirection)
        {
            byte result = 0;
            byte carry = 0;

            switch (shiftDirection)
            {
                case BitDirection.Left:
                    carry = (byte)(operand >> 7);
                    result = (byte)(operand << 1 | carry);
                    break;
                case BitDirection.Right:
                    carry = (byte)(operand & 1);
                    result = (byte)((operand >> 1) | (carry << 7));
                    break;
            }

            UpdateShiftRotateFlags(result, carry);

            return result;
        }

        private byte RotateThroughCarry(byte operand, [ConstantExpected] BitDirection shiftDirection)
        {
            byte result = 0;
            byte operandCarry = 0;
            byte carry = (byte)(Registers.F & 1);

            switch (shiftDirection)
            {
                case BitDirection.Left:
                    operandCarry = (byte)(operand >> 7);
                    result = (byte)(operand << 1 | carry);
                    break;
                case BitDirection.Right:
                    operandCarry = (byte)(operand & 1);
                    result = (byte)(operand >> 1 | carry << 7);
                    Registers.SetFlagConditionally(FlagType.PV, CheckParity(result));
                    break;
            }

            UpdateShiftRotateFlags(result, operandCarry);

            return result;
        }
    }
}