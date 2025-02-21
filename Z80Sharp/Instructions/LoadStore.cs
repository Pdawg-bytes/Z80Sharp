using Z80Sharp.Enums;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_RR_RR(ref ushort dest, ref ushort source) => dest = source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_RR_NN(ref ushort dest) => dest = FetchImmediateWord();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_RR_NNMEM(ref ushort dest)
        {
            ushort source = FetchImmediateWord();
            Registers.MEMPTR = (ushort)(source + 1);
            dest = _memory.ReadWord(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_IR_NNMEM(ref ushort dest)
        {
            ushort source = FetchImmediateWord();
            Registers.MEMPTR = (ushort)(source + 1);
            dest = _memory.ReadWord(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_NNMEM_RR(ref ushort source)
        {
            ushort dest = FetchImmediateWord();
            Registers.MEMPTR = (ushort)(dest + 1);
            _memory.WriteWord(dest, source);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_R_R(ref byte dest, ref byte source) => dest = source;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_R_N(ref byte dest) => dest = Fetch();

        private void LD_A_R(ref byte source)
        {
            Registers.A = source;
            Registers.F = (byte)(Registers.F & (byte)FlagType.C);
            Registers.SetFlagBits((byte)(0xA8 & Registers.A));
            Registers.SetFlagConditionally(FlagType.Z, Registers.A == 0);
            Registers.SetFlagConditionally(FlagType.PV, Registers.IFF2);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_R_NNMEM(ref byte dest)
        {
            ushort source = FetchImmediateWord();
            Registers.MEMPTR = (ushort)(source + 1);
            dest = _memory.Read(source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_A_RRMEM(ref ushort source)
        {
            Registers.MEMPTR = (ushort)(source + 1);
            Registers.A = _memory.Read(source);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_R_RRMEM(ref byte dest, ref ushort source) => dest = _memory.Read(source);

        private void LD_R_IRDMEM(ref byte dest, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + (sbyte)Fetch());
            Registers.MEMPTR = ird;
            dest = _memory.Read(ird);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_NNMEM_R(ref byte source)
        {
            ushort dest = FetchImmediateWord();
            Registers.MEMPTR = (ushort)((source << 8) + ((dest + 1) & 0xFF));
            _memory.Write(dest, source);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_HLMEM_N() => _memory.Write(Registers.HL, Fetch());

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_HLMEM_R(ref byte source) => _memory.Write(Registers.HL, source);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LD_RRMEM_R(ref ushort dest, ref byte source)
        {
            Registers.MEMPTR = (ushort)((source << 8) + ((dest + 1) & 0xFF));
            _memory.Write(dest, source);
        }

        private void LD_IRDMEM_R(ref ushort indexAddressingMode, ref byte source)
        {
            ushort ird = (ushort)(indexAddressingMode + (sbyte)Fetch());
            Registers.MEMPTR = ird;
            _memory.Write(ird, source);
        }

        private void LD_IRDMEM_N(ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + (sbyte)Fetch());
            Registers.MEMPTR = ird;
            _memory.Write(ird, Fetch());
        }



        private void LDBlock(bool increment, bool repeat)
        {
            byte hlMem = _memory.Read(Registers.HL);
            _memory.Write(Registers.DE, hlMem);

            Registers.HL += (ushort)(increment ? 1 : -1);
            Registers.DE += (ushort)(increment ? 1 : -1);
            Registers.BC--;

            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0);
            Registers.F &= (byte)~(FlagType.N | FlagType.H);

            byte undoc = (byte)(hlMem + Registers.A);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x08) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x02) != 0);

            if (repeat && Registers.BC != 0)
            {
                Registers.PC -= 2;
                Registers.MEMPTR = (ushort)(Registers.PC + 1);
                // Apparently, the Z80 copies bits 11 and 13 of PC into X and Y when a repeat occurs. Why? I don't know.
                Registers.SetFlagConditionally(FlagType.X, (Registers.PC & 0x0800) != 0);
                Registers.SetFlagConditionally(FlagType.Y, (Registers.PC & 0x2000) != 0);
                _clock.LastOperationStatus = true;
                return;
            }
            _clock.LastOperationStatus = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LDI() => LDBlock(true, false);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LDIR() => LDBlock(true, true);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LDD() => LDBlock(false, false);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LDDR() => LDBlock(false, true);
    }
}