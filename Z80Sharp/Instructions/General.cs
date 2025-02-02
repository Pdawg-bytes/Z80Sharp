using Z80Sharp.Enums;
using System.Runtime.CompilerServices;
using System.Diagnostics.CodeAnalysis;

namespace Z80Sharp.Processor
{
    public partial class Z80
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


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EX_AF_AF_() => Registers.R16Exchange(ref Registers.AF, ref Registers.AF_);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void EX_DE_HL() => Registers.R16Exchange(ref Registers.DE, ref Registers.HL);

        private void EX_SPMEM_HL()
        {
            ushort sp = Registers.SP;
            ushort oldHl = Registers.HL;
            Registers.HL = _memory.ReadWord(sp);
            Registers.MEMPTR = Registers.HL;
            _memory.WriteWord(sp, oldHl);
        }
        private void EX_SPMEM_IR(ref ushort indexAddressingMode)
        {
            ushort sp = Registers.SP;
            ushort oldIr = indexAddressingMode;
            indexAddressingMode = _memory.ReadWord(sp);
            Registers.MEMPTR = indexAddressingMode;
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