using Z80Sharp.Enums;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

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
            Registers.A = (byte)~Registers.A;
            Registers.F &= (byte)(FlagType.S | FlagType.Z | FlagType.PV | FlagType.C);
            Registers.F |= (byte)((byte)FlagType.H | (byte)FlagType.N | (byte)(FlagType.Y | FlagType.X) & Registers.A);
        }


        private void CCF()
        {
            bool original = Registers.IsFlagSet(FlagType.C);
            Registers.F = (byte)(Registers.F & ((byte)(FlagType.S | FlagType.Z | FlagType.PV | FlagType.C) | (byte)(FlagType.Y | FlagType.X) & Registers.A) ^ (byte)FlagType.C);
            Registers.SetFlagConditionally(FlagType.H, original);
        }

        [MethodImplAttribute(MethodImplOptions.AggressiveInlining)]
        private void SCF() => Registers.F = (byte)(Registers.F & (byte)(FlagType.S | FlagType.Z | FlagType.PV) | (byte)FlagType.C | (byte)(FlagType.Y | FlagType.X) & Registers.A);


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
        private void IM_M([ConstantExpected] InterruptMode mode) => Registers.InterruptMode = mode;
    }
}