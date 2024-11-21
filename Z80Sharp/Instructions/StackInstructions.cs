using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        private void POP_RR(ref ushort operatingRegister)
        {
            operatingRegister = _memory.Read(Registers.SP);
            Registers.SP += 2;
            LogInstructionExec($"0x{_currentInstruction:X2}: POP RR");
        }
        // We can't use the generic method because the register indexers for A and F are in the opposite order.
        private void POP_AF()
        {
            Registers.AF = _memory.Read(Registers.SP);
            Registers.SP += 2;
            LogInstructionExec($"0xF1: POP AF");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void POP_PC_SILENT()
        {
            Registers.PC = _memory.Read(Registers.SP);
            Registers.SP += 2;
        }

        private void PUSH_RR(ref ushort operatingRegister)
        {
            _memory.WriteWord(Registers.SP, operatingRegister);
            Registers.SP -= 2;
            LogInstructionExec($"0x{_currentInstruction:X2}: PUSH RR");
        }
        // We can't use the generic method because the register indexers for A and F are in the opposite order.
        private void PUSH_AF()
        {
            _memory.WriteWord(Registers.SP, Registers.AF);
            Registers.SP -= 2;
            LogInstructionExec($"0xF5: PUSH AF");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PUSH_PC_SILENT()
        {
            _memory.WriteWord(Registers.SP, Registers.PC);
            Registers.SP -= 2;
        }
    }
}