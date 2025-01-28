using Z80Sharp.Enums;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        // Reference: https://stackoverflow.com/questions/8119577/z80-daa-instruction
        // I copied this code...I wish there was a better way to do this while keeping it 100% accurate.
        private void DAA()
        {
            int t = 0;

            if (Registers.IsFlagSet(FlagType.H) || ((Registers.A & 0xF) > 9)) t++;
            if (Registers.IsFlagSet(FlagType.C) || (Registers.A > 0x99)) { t += 2; Registers.SetFlag(FlagType.C); }

            if (Registers.IsFlagSet(FlagType.N) && !Registers.IsFlagSet(FlagType.H)) Registers.ClearFlag(FlagType.H);
            else
            {
                if (Registers.IsFlagSet(FlagType.N) && Registers.IsFlagSet(FlagType.H))
                    Registers.SetFlagConditionally(FlagType.H, (Registers.A & 0x0F) < 6);
                else
                    Registers.SetFlagConditionally(FlagType.H, (Registers.A & 0x0F) >= 0x0A);
            }

            switch (t)
            {
                case 1:
                    Registers.A += Registers.IsFlagSet(FlagType.N) ? (byte)0xFA : (byte)0x06; // -6:6
                    break;
                case 2:
                    Registers.A += Registers.IsFlagSet(FlagType.N) ? (byte)0xA0 : (byte)0x60; // -0x60:0x60
                    break;
                case 3:
                    Registers.A += Registers.IsFlagSet(FlagType.N) ? (byte)0x9A : (byte)0x66; // -0x66:0x66
                    break;
            }

            Registers.SetFlagConditionally(FlagType.S, (Registers.A & 0x80) != 0);  // (S)  (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, Registers.A == 0);           // (Z)  (Set if result is zero)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(Registers.A));  // (PV) (Set if bit parity is even)

            Registers.SetFlagConditionally(FlagType.X, (Registers.A & 0x08) != 0); // (X) (copy of bit 3)
            Registers.SetFlagConditionally(FlagType.Y, (Registers.A & 0x20) != 0); // (Y) (copy of bit 5)
        }

        private void NEG()
        {
            byte value = Registers.A;
            int result = 0 - value;
            Registers.A = (byte)result;

            Registers.SetFlagConditionally(FlagType.S, (result & 0x80) != 0); // (S)  (Set if result is negative)
            Registers.SetFlagConditionally(FlagType.Z, Registers.A == 0);     // (Z)  (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.H, (value & 0x0F) != 0);  // (H)  (Set if borrow from bit 4)
            Registers.SetFlagConditionally(FlagType.PV, value == 0x80);       // (PV) (Set if A == -128 (sbyte.Min)
            Registers.SetFlagConditionally(FlagType.C, value != 0);           // (C)  (Set if borrow occured)
            Registers.SetFlag(FlagType.N);                                    // (N)  (Unconditionally set)

            Registers.SetFlagConditionally(FlagType.X, (Registers.A & 0x08) != 0); // (X) (copy of bit 3)
            Registers.SetFlagConditionally(FlagType.Y, (Registers.A & 0x20) != 0); // (Y) (copy of bit 5)
        }

        #region INC instructions (RR, R, (HL), (IR + d))
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void INC_RR(ref ushort operatingRegister) => operatingRegister++;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void INC_R(ref byte operatingRegister) => operatingRegister = INCAny(operatingRegister);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void INC_HLMEM() => _memory.Write(Registers.HL, INCAny(_memory.Read(Registers.HL)));

        private void INC_IRDMEM(ref ushort indexAddressingMode)
        {
            ushort addr = (ushort)(indexAddressingMode + (sbyte)Fetch());
            _memory.Write(addr, INCAny(_memory.Read(addr)));
        }
        #endregion

        #region DEC instructions (RR, R, (HL), (IR + d))
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DEC_RR(ref ushort operatingRegister) => operatingRegister = (ushort)(operatingRegister - 1);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DEC_R(ref byte operatingRegister) => operatingRegister = DECAny(operatingRegister);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DEC_HLMEM() => _memory.Write(Registers.HL, DECAny(_memory.Read(Registers.HL)));

        private void DEC_IRDMEM(ref ushort indexAddressingMode)
        {
            ushort addr = (ushort)(indexAddressingMode + (sbyte)Fetch());
            _memory.Write(addr, DECAny(_memory.Read(addr)));
        }
        #endregion


        #region OR instructions (R, N, (RR), (IR + d))
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OR_R(ref byte operatingRegister) => ORAny(operatingRegister);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OR_N() => ORAny(Fetch());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OR_RRMEM(ref ushort operatingRegister) => ORAny(_memory.Read(operatingRegister));

        private void OR_IRDMEM(ref ushort indexAddressingMode) => ORAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));
        #endregion

        #region XOR instructions (R, N, (RR), (IR + d))
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void XOR_R(ref byte operatingRegister) => XORAny(operatingRegister);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void XOR_N() => XORAny(Fetch());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void XOR_RRMEM(ref ushort operatingRegister) => XORAny(_memory.Read(operatingRegister));

        private void XOR_IRDMEM(ref ushort indexAddressingMode) => XORAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));
        #endregion


        #region AND instructions (R, N, (RR), (IR + d))
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AND_R(ref byte operatingRegister) => ANDAny(operatingRegister);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AND_N() => ANDAny(Fetch());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AND_RRMEM(ref ushort operatingRegister) => ANDAny(_memory.Read(operatingRegister));

        private void AND_IRDMEM(ref ushort indexAddressingMode) => ANDAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));
        #endregion


        #region CP instructions (R, N, (RR), (IR + d))
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CMP_R(ref byte operatingRegister) => CMPAny(operatingRegister);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CMP_N() => CMPAny(Fetch());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CMP_RRMEM(ref ushort operatingRegister) => CMPAny(_memory.Read(operatingRegister));

        private void CMP_IRDMEM(ref ushort indexAddressingMode) => CMPAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));

        private void CMPBlock(bool increment, bool repeat)
        {
            byte hlMem = _memory.Read(Registers.HL);

            Registers.HL += (ushort)(increment ? 1 : -1);
            Registers.BC--;

            int diff = Registers.A - hlMem;

            Registers.F &= (byte)FlagType.C;
            Registers.F |= (byte)((byte)FlagType.N | (byte)(diff & (byte)FlagType.S));
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);
            Registers.F |= (byte)((Registers.A ^ hlMem ^ diff) & (byte)FlagType.H);
            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0);

            var n = diff - (((Registers.F & (byte)FlagType.H) != 0) ? 1 : 0);
            Registers.SetFlagConditionally(FlagType.Y, (n & 0b0000010) != 0);
            Registers.SetFlagConditionally(FlagType.X, (n & 0b0001000) != 0);

            if (repeat && Registers.BC != 0 && diff != 0)
            {
                Registers.PC -= 2;
                _clock.LastOperationStatus = true;
                return;
            }
            _clock.LastOperationStatus = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CPIR() => CMPBlock(true, true);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CPDR() => CMPBlock(false, true);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CPI() => CMPBlock(true, false);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CPD() => CMPBlock(false, false);
        #endregion


        #region ADD instructions ((A, R), (A, N), (A, (RR)), (A, (IR + d)), (HL, RR), (IR, RR))
        [MethodImpl(MethodImplOptions.AggressiveInlining)] 
        private void ADD_A_R(ref byte operatingRegister) => ADDAny(operatingRegister);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADD_A_N() => ADDAny(Fetch());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADD_A_RRMEM(ref ushort operatingRegister) => ADDAny(_memory.Read(operatingRegister));

        private void ADD_A_IRDMEM(ref ushort indexAddressingMode) => ADDAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADD_HL_RR(ref ushort operatingRegister) => Registers.HL = ADDWord(Registers.HL, operatingRegister);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADD_IR_RR(ref ushort mode, ref ushort operatingRegister) => mode = ADDWord(mode, operatingRegister);
        #endregion

        #region ADC instructions ((A, R), (A, N), (A, (RR)), (A, (IR + d)), (HL, RR))
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADC_A_R(ref byte operatingRegister) => ADCAny(operatingRegister);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADC_A_N() => ADCAny(Fetch());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADC_A_RRMEM(ref ushort operatingRegister) => ADCAny(_memory.Read(operatingRegister));

        private void ADC_A_IRDMEM(ref ushort indexAddressingMode) => ADCAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADC_HL_RR(ref ushort operatingRegister) => ADCHL(operatingRegister);
        #endregion


        #region SUB instructions (R, N, (RR), (IR + d))
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SUB_A_R(ref byte operatingRegister) => SUBAny(operatingRegister);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SUB_A_N() => SUBAny(Fetch());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SUB_A_RRMEM(ref ushort operatingRegister) => SUBAny(_memory.Read(operatingRegister));

        private void SUB_A_IRDMEM(ref ushort indexAddressingMode) => SUBAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));
        #endregion

        #region SBC instructions (A, R), (A, N), (A, (RR)), (A, (IR + d)), (HL, RR)
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SBC_A_R(ref byte operatingRegister) => SBCAny(operatingRegister);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SBC_A_N() => SBCAny(Fetch());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SBC_A_RRMEM(ref ushort operatingRegister) => SBCAny(_memory.Read(operatingRegister));

        private void SBC_A_IRDMEM(ref ushort indexAddressingMode) => SBCAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SBC_HL_RR(ref ushort operatingRegister) => SBCHL(operatingRegister);
        #endregion
    }
}