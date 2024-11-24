using Z80Sharp.Enums;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        #region Rotate Accumulator
        private void RLCA()
        {
            byte carry = (byte)((Registers.A >> 7) & 0x1);
            Registers.A = (byte)((Registers.A << 1) | carry);
            Registers.F &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagConditionally(FlagType.C, carry != 0);
            //LogInstructionExec("0x07: RLCA");
        }
        private void RRCA()
        {
            byte carry = (byte)(Registers.A & 0b00000001);
            Registers.A >>= 1;
            Registers.A |= (byte)(carry << 7);
            Registers.F &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagBits(carry);
            //LogInstructionExec("0x0F: RRCA");
        }

        private void RLA()
        {
            byte carry = (byte)(Registers.A >> 7);
            Registers.A <<= 1;
            Registers.A |= (byte)(Registers.F & (byte)FlagType.C);
            Registers.A = Registers.A;
            Registers.F &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagBits(carry);
            //LogInstructionExec("0x17: RLA");
        }
        private void RRA()
        {
            byte carry = (byte)(Registers.A & 0b00000001);
            Registers.A >>= 1;
            Registers.A |= (byte)((Registers.F & (byte)FlagType.C) << 7);
            Registers.A = Registers.A;
            Registers.F &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagBits(carry);
            //LogInstructionExec("0x1F: RRA");
        }
        #endregion

        #region Rotate Digit
        private void RRD()
        {
            byte memoryValue = _memory.Read(Registers.HL);

            _memory.Write(Registers.HL, (byte)((memoryValue >> 4) | (Registers.A << 4))); // Combine high of (HL) w/ low of A
            Registers.A = (byte)((Registers.A & 0xF0) | (memoryValue & 0x0F));     // Combine high of A w/ low of (HL)

            Registers.SetFlagConditionally(FlagType.S, (Registers.A & 0x80) > 0);  // (S) (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, Registers.A == 0);          // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(Registers.A)); // (PV) (Set if bit parity is even)
            Registers.F &= (byte)~(FlagType.N | FlagType.H);                       // (N, H) (Unconditionally reset)
            Registers.SetFlagConditionally(FlagType.X, (Registers.A & 0x20) > 0);  // (X)  (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (Registers.A & 0x08) > 0);  // (Y)  (Undocumented flag)
            //LogInstructionExec("0x67: RRD");
        }
        private void RLD()
        {
            byte regA = Registers.A;
            byte memoryValue = _memory.Read(Registers.HL);

            _memory.Write(Registers.HL, (byte)((memoryValue << 4) | (regA & 0x0F))); // Combine low of (HL) w/ low of A
            Registers.A = regA = (byte)((regA & 0xF0) | (memoryValue >> 4));         // Combine high of A w/ high of (HL)

            Registers.SetFlagConditionally(FlagType.S, (regA & 0x80) > 0);  // (S) (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, regA == 0);          // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(regA)); // (PV) (Set if bit parity is even)
            Registers.F &= (byte)~(FlagType.N | FlagType.H);                // (N, H) (Unconditionally reset)
            Registers.SetFlagConditionally(FlagType.X, (regA & 0x20) > 0);  // (X)  (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (regA & 0x08) > 0);  // (Y)  (Undocumented flag)
            //LogInstructionExec("0x6F: RLD");
        }
        #endregion


        #region Shift left/right arithmetic
        private void SLA_R(ref byte operatingRegister)
        {
            operatingRegister = ShiftArith(operatingRegister, BitDirection.Left);
        }
        private void SLA_RRMEM(ref ushort operatingRegister)
        {
            _memory.Write(operatingRegister, ShiftArith(_memory.Read(operatingRegister), BitDirection.Left));
        }
        private void SLA_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            _memory.Write(ird, ShiftArith(_memory.Read(ird), BitDirection.Left));
        }
        private void SLA_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            byte result = ShiftArith(_memory.Read(ird), BitDirection.Left);
            _memory.Write(ird, result);
            outputRegister = result;
        }

        private void SRA_R(ref byte operatingRegister)
        {
            operatingRegister = ShiftArith(operatingRegister, BitDirection.Right);
        }
        private void SRA_RRMEM(ref ushort operatingRegister)
        {
            _memory.Write(operatingRegister, ShiftArith(_memory.Read(operatingRegister), BitDirection.Right));
        }
        private void SRA_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            _memory.Write(ird, ShiftArith(_memory.Read(ird), BitDirection.Right));
        }
        private void SRA_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            byte result = ShiftArith(_memory.Read(ird), BitDirection.Right);
            _memory.Write(ird, result);
            outputRegister = result;
        }
        #endregion

        #region Shift left/right logical
        private void SLL_R(ref byte operatingRegister) // UNDOCUMENTED
        {
            operatingRegister = ShiftLogical(operatingRegister, BitDirection.Left);
        }
        private void SLL_RRMEM(ref ushort operatingRegister) // UNDOCUMENTED
        {
            _memory.Write(operatingRegister, ShiftLogical(_memory.Read(operatingRegister), BitDirection.Left));
        }
        private void SLL_IRDMEM(sbyte displacement, ref ushort indexAddressingMode) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            _memory.Write(ird, ShiftLogical(_memory.Read(ird), BitDirection.Left));
        }
        private void SLL_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            byte result = ShiftLogical(_memory.Read(ird), BitDirection.Left);
            _memory.Write(ird, result);
            outputRegister = result;
        }

        private void SRL_R(ref byte operatingRegister)
        {
            operatingRegister = ShiftLogical(operatingRegister, BitDirection.Right);
        }
        private void SRL_RRMEM(ref ushort operatingRegister)
        {
            _memory.Write(operatingRegister, ShiftLogical(_memory.Read(operatingRegister), BitDirection.Right));
        }
        private void SRL_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            _memory.Write(ird, ShiftLogical(_memory.Read(ird), BitDirection.Right));
        }
        private void SRL_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            byte result = ShiftLogical(_memory.Read(ird), BitDirection.Right);
            _memory.Write(ird, result);
            outputRegister = result;
        }
        #endregion


        #region Rotate left/right circular
        private void RLC_R(ref byte operatingRegister)
        {
            operatingRegister = RotateCircular(operatingRegister, BitDirection.Left);
        }
        private void RLC_RRMEM(ref ushort operatingRegister)
        {
            _memory.Write(operatingRegister, RotateCircular(_memory.Read(operatingRegister), BitDirection.Left));
        }
        private void RLC_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            _memory.Write(ird, RotateCircular(_memory.Read(ird), BitDirection.Left));
        }
        private void RLC_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            byte result = RotateCircular(_memory.Read(ird), BitDirection.Left);
            _memory.Write(ird, result);
            outputRegister = result;
        }

        private void RRC_R(ref byte operatingRegister)
        {
            operatingRegister = RotateCircular(operatingRegister, BitDirection.Right);
        }
        private void RRC_RRMEM(ref ushort operatingRegister)
        {
            _memory.Write(operatingRegister, RotateCircular(_memory.Read(operatingRegister), BitDirection.Right));
        }
        private void RRC_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            _memory.Write(ird, RotateCircular(_memory.Read(ird), BitDirection.Right));
        }
        private void RRC_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            byte result = RotateCircular(_memory.Read(ird), BitDirection.Left);
            _memory.Write(ird, result);
            outputRegister = result;
        }
        #endregion

        #region Rotate left/right through carry
        private void RL_R(ref byte operatingRegister)
        {
            operatingRegister = RotateThroughCarry(operatingRegister, BitDirection.Left);
        }
        private void RL_RRMEM(ref ushort operatingRegister)
        {
            _memory.Write(operatingRegister, RotateThroughCarry(_memory.Read(operatingRegister), BitDirection.Left));
        }
        private void RL_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            _memory.Write(ird, RotateThroughCarry(_memory.Read(ird), BitDirection.Left));
        }
        private void RL_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            byte result = RotateThroughCarry(_memory.Read(ird), BitDirection.Left);
            _memory.Write(ird, result);
            outputRegister = result;
        }

        private void RR_R(ref byte operatingRegister)
        {
            operatingRegister = RotateThroughCarry(operatingRegister, BitDirection.Right);
        }
        private void RR_RRMEM(ref ushort operatingRegister)
        {
            _memory.Write(operatingRegister, RotateThroughCarry(_memory.Read(operatingRegister), BitDirection.Right));
        }
        private void RR_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            _memory.Write(ird, RotateThroughCarry(_memory.Read(ird), BitDirection.Right));
        }
        private void RR_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            byte result = RotateThroughCarry(_memory.Read(ird), BitDirection.Right);
            _memory.Write(ird, result);
            outputRegister = result;
        }
        #endregion
    }
}