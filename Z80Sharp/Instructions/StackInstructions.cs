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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void POP_RR(ref ushort operatingRegister)
        {
            operatingRegister = _memory.ReadWord(Registers.SP);
            Registers.SP += 2;
        }
        // We can't use the generic method because the register indexers for A and F are in the opposite order.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void POP_AF()
        {
            Registers.F = _memory.Read(Registers.SP);
            Registers.SP++;
            Registers.A = _memory.Read(Registers.SP);
            Registers.SP++;

            //LogInstructionExec($"0xF1: POP AF");
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
        // We can't use the generic method because the register indexers for A and F are in the opposite order.
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PUSH_AF()
        {
            Registers.SP--;
            _memory.Write(Registers.SP, Registers.A);
            Registers.SP--;
            _memory.Write(Registers.SP, Registers.F);
            //LogInstructionExec($"0xF5: PUSH AF");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PUSH_PC()
        {
            Registers.SP -= 2;
            _memory.WriteWord(Registers.SP, Registers.PC);
        }
    }
}