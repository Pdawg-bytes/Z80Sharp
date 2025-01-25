using Z80Sharp.Enums;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void NOP() { }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void HALT()
        {
            if (!Halted)
            {
                Halted = true;
                Registers.PC--;
            }
        }


        private void CPL()
        {
            Registers.A ^= 0b11111111;
            Registers.SetFlagBits((byte)(FlagType.N | FlagType.H));

            Registers.SetFlagConditionally(FlagType.X, (Registers.A & 0x08) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (Registers.A & 0x20) != 0);
        }


        private void CCF()
        {
            Registers.F &= (byte)~(FlagType.H | FlagType.N);
            Registers.SetFlagBits((byte)((Registers.F << 4) & 0b00010000));
            Registers.InvertFlag(FlagType.C);
        }
        private void SCF()
        {
            Registers.SetFlag(FlagType.C);
            Registers.ClearFlag(FlagType.N);
            Registers.ClearFlag(FlagType.H);

            byte temp = (byte)(Registers.A | Registers.F);
            Registers.SetFlagConditionally(FlagType.X, (temp & 0x08) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (temp & 0x20) != 0);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EX_AF_AF_() => Registers.R16Exchange(ref Registers.AF, ref Registers.AF_);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EX_DE_HL() => Registers.R16Exchange(ref Registers.DE, ref Registers.HL);

        private void EX_SPMEM_HL()
        {
            ushort sp = Registers.SP;
            ushort oldHl = Registers.HL;
            Registers.HL = _memory.ReadWord(sp);
            _memory.WriteWord(sp, oldHl);
        }
        private void EX_SPMEM_IR(ref ushort indexAddressingMode)
        {
            ushort sp = Registers.SP;
            ushort oldIr = indexAddressingMode;
            indexAddressingMode = _memory.ReadWord(sp);
            _memory.WriteWord(sp, oldIr);
        }

        private void EXX()
        {
            Registers.R16Exchange(ref Registers.BC, ref Registers.BC_);
            Registers.R16Exchange(ref Registers.DE, ref Registers.DE_);
            Registers.R16Exchange(ref Registers.HL, ref Registers.HL_);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void DI() => Registers.IFF1 = Registers.IFF2 = false;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EI() => Registers.IFF1 = Registers.IFF2 = true;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IM_M(InterruptMode mode) => Registers.InterruptMode = mode;
    }
}