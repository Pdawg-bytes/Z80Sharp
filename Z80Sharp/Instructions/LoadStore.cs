using Z80Sharp.Enums;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_RR_RR(ref ushort dest, ref ushort source) => dest = source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_RR_NN(ref ushort operatingRegister) => operatingRegister = FetchImmediateWord();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_RR_NNMEM(ref ushort operatingRegister) => operatingRegister = _memory.ReadWord(FetchImmediateWord());


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_R_R(ref byte dest, ref byte source) => dest = source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_R_N(ref byte operatingRegister) => operatingRegister = Fetch();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_R_NNMEM(ref byte operatingRegister) => operatingRegister = _memory.Read(FetchImmediateWord());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_R_RRMEM(ref byte dest, ref ushort source) => dest = _memory.Read(source);

        private void LD_R_IRDMEM(ref byte dest, ref ushort indexAddressingMode) => dest = _memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch()));


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_NNMEM_R(ref byte operatingRegister) => _memory.Write(FetchImmediateWord(), operatingRegister);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_NNMEM_RR(ref ushort operatingRegister) => _memory.WriteWord(FetchImmediateWord(), operatingRegister);


        private void LD_IRDMEM_R(ref ushort indexAddressingMode, ref byte source) => _memory.Write((ushort)(indexAddressingMode + (sbyte)Fetch()), source);

        private void LD_IRDMEM_N(ref ushort indexAddressingMode) => _memory.Write((ushort)(indexAddressingMode + (sbyte)Fetch()), Fetch());


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_HLMEM_N() => _memory.Write(Registers.HL, Fetch());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_RRMEM_R(ref ushort dest, ref byte source) => _memory.Write(dest, source);


        private void LDI()
        {
            byte hlMem = _memory.Read(Registers.HL);
            _memory.Write(Registers.DE, hlMem);
            Registers.HL++;
            Registers.DE++;
            Registers.BC--;

            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0);
            Registers.F &= (byte)~(FlagType.N | FlagType.H);

            byte undoc = (byte)(Registers.A + hlMem);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x20) > 0);
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LDIR()
        {
            LDI();
            if (Registers.BC != 0)
            {
                Registers.PC -= 2;
                _clock.LastOperationStatus = false;
                return;
            }
            _clock.LastOperationStatus = true;
        }

        private void LDD()
        {
            byte hlMem = _memory.Read(Registers.HL);
            _memory.Write(Registers.DE, hlMem);
            Registers.HL--;
            Registers.DE--;
            Registers.BC--;

            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0);
            Registers.F &= (byte)~(FlagType.N | FlagType.H);

            byte undoc = (byte)(Registers.A + hlMem);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x20) > 0);
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LDDR()
        {
            LDD();
            if (Registers.BC != 0)
            {
                Registers.PC -= 2;
                _clock.LastOperationStatus = false;
                return;
            }
            _clock.LastOperationStatus = true;
        }
    }
}