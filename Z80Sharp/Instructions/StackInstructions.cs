using System.Runtime.CompilerServices;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void POP_RR(ref ushort operatingRegister)
        {
            operatingRegister = _memory.ReadWord(Registers.SP);
            Registers.SP += 2;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void POP_AF()
        {
            Registers.F = _memory.Read(Registers.SP);
            Registers.SP++;
            Registers.A = _memory.Read(Registers.SP);
            Registers.SP++;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void POP_PC()
        {
            Registers.PC = _memory.ReadWord(Registers.SP);
            Registers.SP += 2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PUSH_RR(ref ushort operatingRegister)
        {
            Registers.SP -= 2;
            _memory.WriteWord(Registers.SP, operatingRegister);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PUSH_AF()
        {
            Registers.SP--;
            _memory.Write(Registers.SP, Registers.A);
            Registers.SP--;
            _memory.Write(Registers.SP, Registers.F);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PUSH_PC()
        {
            Registers.SP -= 2;
            _memory.WriteWord(Registers.SP, Registers.PC);
        }
    }
}