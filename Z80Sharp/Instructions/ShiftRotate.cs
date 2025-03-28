﻿using Z80Sharp.Enums;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        #region Rotate Accumulator
        private void RLCA()
        {
            byte carry = (byte)((Registers.A >> 7) & 0x1);
            Registers.A = (byte)((Registers.A << 1) | carry);
            Registers.F &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagConditionally(FlagType.C, carry != 0);
            Registers.SetFlagConditionally(FlagType.X, (Registers.A & 0x08) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (Registers.A & 0x20) != 0);
        }
        private void RRCA()
        {
            byte carry = (byte)(Registers.A & 0b00000001);
            Registers.A >>= 1;
            Registers.A |= (byte)(carry << 7);
            Registers.F &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagBits(carry);
            Registers.SetFlagConditionally(FlagType.X, (Registers.A & 0x08) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (Registers.A & 0x20) != 0);
        }

        private void RLA()
        {
            byte carry = (byte)(Registers.A >> 7);
            Registers.A <<= 1;
            Registers.A |= (byte)(Registers.F & (byte)FlagType.C);
            Registers.A = Registers.A;
            Registers.F &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagBits(carry);
            Registers.SetFlagConditionally(FlagType.X, (Registers.A & 0x08) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (Registers.A & 0x20) != 0);
        }
        private void RRA()
        {
            byte carry = (byte)(Registers.A & 0b00000001);
            Registers.A >>= 1;
            Registers.A |= (byte)((Registers.F & (byte)FlagType.C) << 7);
            Registers.A = Registers.A;
            Registers.F &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagBits(carry);
            Registers.SetFlagConditionally(FlagType.X, (Registers.A & 0x08) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (Registers.A & 0x20) != 0);
        }
        #endregion

        #region Rotate Digit
        private void RRD()
        {
            byte memoryValue = _memory.Read(Registers.HL);

            _memory.Write(Registers.HL, (byte)((memoryValue >> 4) | (Registers.A << 4))); // Combine high of (HL) w/ low of A
            Registers.A = (byte)((Registers.A & 0xF0) | (memoryValue & 0x0F));            // Combine high of A w/ low of (HL)

            Registers.MEMPTR = (ushort)(Registers.HL + 1);

            Registers.SetFlagConditionally(FlagType.S, (Registers.A & 0x80) > 0);  // (S) (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, Registers.A == 0);          // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(Registers.A)); // (PV) (Set if bit parity is even)
            Registers.F &= (byte)~(FlagType.N | FlagType.H);                       // (N, H) (Unconditionally reset)
            Registers.SetFlagConditionally(FlagType.X, (Registers.A & 0x08) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (Registers.A & 0x20) != 0);
        }
        private void RLD()
        {
            byte regA = Registers.A;
            byte memoryValue = _memory.Read(Registers.HL);

            _memory.Write(Registers.HL, (byte)((memoryValue << 4) | (regA & 0x0F))); // Combine low of (HL) w/ low of A
            Registers.A = regA = (byte)((regA & 0xF0) | (memoryValue >> 4));         // Combine high of A w/ high of (HL)

            Registers.MEMPTR = (ushort)(Registers.HL + 1);

            Registers.SetFlagConditionally(FlagType.S, (regA & 0x80) > 0);  // (S) (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, regA == 0);          // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(regA)); // (PV) (Set if bit parity is even)
            Registers.F &= (byte)~(FlagType.N | FlagType.H);                // (N, H) (Unconditionally reset)
            Registers.SetFlagConditionally(FlagType.X, (Registers.A & 0x08) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (Registers.A & 0x20) != 0);
        }
        #endregion


        #region Shift left/right arithmetic
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SLA_R(ref byte operatingRegister) => operatingRegister = ShiftArith(operatingRegister, BitDirection.Left);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SLA_RRMEM(ref ushort operatingRegister) => _memory.Write(operatingRegister, ShiftArith(_memory.Read(operatingRegister), BitDirection.Left));

        private void SLA_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            _memory.Write(ird, ShiftArith(_memory.Read(ird), BitDirection.Left));
        }

        private void SLA_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            byte result = ShiftArith(_memory.Read(ird), BitDirection.Left);
            _memory.Write(ird, result);
            outputRegister = result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SRA_R(ref byte operatingRegister) => operatingRegister = ShiftArith(operatingRegister, BitDirection.Right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SRA_RRMEM(ref ushort operatingRegister) => _memory.Write(operatingRegister, ShiftArith(_memory.Read(operatingRegister), BitDirection.Right));

        private void SRA_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            _memory.Write(ird, ShiftArith(_memory.Read(ird), BitDirection.Right));
        }

        private void SRA_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            byte result = ShiftArith(_memory.Read(ird), BitDirection.Right);
            _memory.Write(ird, result);
            outputRegister = result;
        }
        #endregion

        #region Shift left/right logical
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SLL_R(ref byte operatingRegister) => operatingRegister = ShiftLogical(operatingRegister, BitDirection.Left);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SLL_RRMEM(ref ushort operatingRegister) => _memory.Write(operatingRegister, ShiftLogical(_memory.Read(operatingRegister), BitDirection.Left));

        private void SLL_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            _memory.Write(ird, ShiftLogical(_memory.Read(ird), BitDirection.Left));
        }

        private void SLL_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            byte result = ShiftLogical(_memory.Read(ird), BitDirection.Left);
            _memory.Write(ird, result);
            outputRegister = result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SRL_R(ref byte operatingRegister) => operatingRegister = ShiftLogical(operatingRegister, BitDirection.Right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SRL_RRMEM(ref ushort operatingRegister) => _memory.Write(operatingRegister, ShiftLogical(_memory.Read(operatingRegister), BitDirection.Right));

        private void SRL_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            _memory.Write(ird, ShiftLogical(_memory.Read(ird), BitDirection.Right));
        }

        private void SRL_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            byte result = ShiftLogical(_memory.Read(ird), BitDirection.Right);
            _memory.Write(ird, result);
            outputRegister = result;
        }
        #endregion


        #region Rotate left/right circular
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RLC_R(ref byte operatingRegister) => operatingRegister = RotateCircular(operatingRegister, BitDirection.Left);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RLC_RRMEM(ref ushort operatingRegister) => _memory.Write(operatingRegister, RotateCircular(_memory.Read(operatingRegister), BitDirection.Left));

        private void RLC_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            _memory.Write(ird, RotateCircular(_memory.Read(ird), BitDirection.Left));
        }

        private void RLC_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            byte result = RotateCircular(_memory.Read(ird), BitDirection.Left);
            _memory.Write(ird, result);
            outputRegister = result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RRC_R(ref byte operatingRegister) => operatingRegister = RotateCircular(operatingRegister, BitDirection.Right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RRC_RRMEM(ref ushort operatingRegister) => _memory.Write(operatingRegister, RotateCircular(_memory.Read(operatingRegister), BitDirection.Right));

        private void RRC_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            _memory.Write(ird, RotateCircular(_memory.Read(ird), BitDirection.Right));
        }

        private void RRC_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            byte result = RotateCircular(_memory.Read(ird), BitDirection.Right);
            _memory.Write(ird, result);
            outputRegister = result;
        }
        #endregion

        #region Rotate left/right through carry
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RL_R(ref byte operatingRegister) => operatingRegister = RotateThroughCarry(operatingRegister, BitDirection.Left);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RL_RRMEM(ref ushort operatingRegister) => _memory.Write(operatingRegister, RotateThroughCarry(_memory.Read(operatingRegister), BitDirection.Left));

        private void RL_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            _memory.Write(ird, RotateThroughCarry(_memory.Read(ird), BitDirection.Left));
        }

        private void RL_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            byte result = RotateThroughCarry(_memory.Read(ird), BitDirection.Left);
            _memory.Write(ird, result);
            outputRegister = result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RR_R(ref byte operatingRegister) => operatingRegister = RotateThroughCarry(operatingRegister, BitDirection.Right);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RR_RRMEM(ref ushort operatingRegister) => _memory.Write(operatingRegister, RotateThroughCarry(_memory.Read(operatingRegister), BitDirection.Right));
 
        private void RR_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            _memory.Write(ird, RotateThroughCarry(_memory.Read(ird), BitDirection.Right));
        }

        private void RR_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            byte result = RotateThroughCarry(_memory.Read(ird), BitDirection.Right);
            _memory.Write(ird, result);
            outputRegister = result;
        }
        #endregion
    }
}