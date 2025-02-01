using Z80Sharp.Enums;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADD_A_R(ref byte operatingRegister) => AddSub8WithCarry(operatingRegister, false, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADD_A_N() => AddSub8WithCarry(Fetch(), false, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADD_A_RRMEM(ref ushort operatingRegister) => AddSub8WithCarry(_memory.Read(operatingRegister), false, false);

        private void ADD_A_IRDMEM(ref ushort indexAddressingMode) => AddSub8WithCarry(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())), false, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADC_A_R(ref byte operatingRegister) => AddSub8WithCarry(operatingRegister, false, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADC_A_N() => AddSub8WithCarry(Fetch(), false, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADC_A_RRMEM(ref ushort operatingRegister) => AddSub8WithCarry(_memory.Read(operatingRegister), false, true);

        private void ADC_A_IRDMEM(ref ushort indexAddressingMode) => AddSub8WithCarry(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())), false, true);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SUB_A_R(ref byte operatingRegister) => AddSub8WithCarry(operatingRegister, true, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SUB_A_N() => AddSub8WithCarry(Fetch(), true, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SUB_A_RRMEM(ref ushort operatingRegister) => AddSub8WithCarry(_memory.Read(operatingRegister), true, false);

        private void SUB_A_IRDMEM(ref ushort indexAddressingMode) => AddSub8WithCarry(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())), true, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SBC_A_R(ref byte operatingRegister) => AddSub8WithCarry(operatingRegister, true, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SBC_A_N() => AddSub8WithCarry(Fetch(), true, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SBC_A_RRMEM(ref ushort operatingRegister) => AddSub8WithCarry(_memory.Read(operatingRegister), true, true);

        private void SBC_A_IRDMEM(ref ushort indexAddressingMode) => AddSub8WithCarry(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())), true, true);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void INC_R(ref byte operatingRegister) => operatingRegister = IncDec8(operatingRegister, false);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void INC_HLMEM() => _memory.Write(Registers.HL, IncDec8(_memory.Read(Registers.HL), false));

        private void INC_IRDMEM(ref ushort indexAddressingMode)
        {
            ushort addr = (ushort)(indexAddressingMode + (sbyte)Fetch());
            _memory.Write(addr, IncDec8(_memory.Read(addr), false));
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DEC_R(ref byte operatingRegister) => operatingRegister = IncDec8(operatingRegister, true);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DEC_HLMEM() => _memory.Write(Registers.HL, IncDec8(_memory.Read(Registers.HL), true));

        private void DEC_IRDMEM(ref ushort indexAddressingMode)
        {
            ushort addr = (ushort)(indexAddressingMode + (sbyte)Fetch());
            _memory.Write(addr, IncDec8(_memory.Read(addr), true));
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OR_R(ref byte operatingRegister) => Bitwise8(operatingRegister, BitOperation.Or);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OR_N() => Bitwise8(Fetch(), BitOperation.Or);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OR_RRMEM(ref ushort operatingRegister) => Bitwise8(_memory.Read(operatingRegister), BitOperation.Or);

        private void OR_IRDMEM(ref ushort indexAddressingMode) => Bitwise8(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())), BitOperation.Or);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void XOR_R(ref byte operatingRegister) => Bitwise8(operatingRegister, BitOperation.Xor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void XOR_N() => Bitwise8(Fetch(), BitOperation.Xor);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void XOR_RRMEM(ref ushort operatingRegister) => Bitwise8(_memory.Read(operatingRegister), BitOperation.Xor);

        private void XOR_IRDMEM(ref ushort indexAddressingMode) => Bitwise8(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())), BitOperation.Xor);


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AND_R(ref byte operatingRegister) => Bitwise8(operatingRegister, BitOperation.And);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AND_N() => Bitwise8(Fetch(), BitOperation.And);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void AND_RRMEM(ref ushort operatingRegister) => Bitwise8(_memory.Read(operatingRegister), BitOperation.And);

        private void AND_IRDMEM(ref ushort indexAddressingMode) => Bitwise8(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())), BitOperation.And);



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CMP_R(ref byte operatingRegister) => Compare8(operatingRegister);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CMP_N() => Compare8(Fetch());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void CMP_RRMEM(ref ushort operatingRegister) => Compare8(_memory.Read(operatingRegister));

        private void CMP_IRDMEM(ref ushort indexAddressingMode) => Compare8(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));

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

            int n = diff - (((Registers.F & (byte)FlagType.H) != 0) ? 1 : 0);
            Registers.SetFlagConditionally(FlagType.Y, (n & 0x02) != 0);
            Registers.SetFlagConditionally(FlagType.X, (n & 0x08) != 0);

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
    }
}