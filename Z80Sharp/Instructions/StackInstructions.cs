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
            byte rL = _memory.Read(Registers.SP);
            Registers.SP++;
            byte rH = _memory.Read(Registers.SP);
            Registers.SP++;

            operatingRegister = (ushort)((rH << 8) | rL);
            //LogInstructionExec($"0x{_currentInstruction:X2}: POP RR");
        }
        // We can't use the generic method because the register indexers for A and F are in the opposite order.
        private void POP_AF()
        {
            Registers.F = _memory.Read(Registers.SP);
            Registers.SP++;
            Registers.A = _memory.Read(Registers.SP);
            Registers.SP++;

            //LogInstructionExec($"0xF1: POP AF");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void POP_PC_SILENT()
        {
            byte pcL = _memory.Read(Registers.SP);
            Registers.SP++;
            byte pcH = _memory.Read(Registers.SP);
            Registers.SP++;

            Registers.PC = (ushort)((pcH << 8) | pcL);
        }

        private void PUSH_RR(ref ushort operatingRegister)
        {
            byte rH = (byte)((operatingRegister >> 8) & 0xff);
            byte rL = (byte)(operatingRegister & 0xff);

            Registers.SP--;
            _memory.Write(Registers.SP, rH);
            Registers.SP--;
            _memory.Write(Registers.SP, rL);
            //LogInstructionExec($"0x{_currentInstruction:X2}: PUSH RR");
        }
        // We can't use the generic method because the register indexers for A and F are in the opposite order.
        private void PUSH_AF()
        {
            byte afH = (byte)((Registers.AF >> 8) & 0xff);
            byte afL = (byte)(Registers.AF & 0xff);

            Registers.SP--;
            _memory.Write(Registers.SP, afH);
            Registers.SP--;
            _memory.Write(Registers.SP, afL);
            //LogInstructionExec($"0xF5: PUSH AF");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PUSH_PC_SILENT()
        {
            byte pcH = (byte)((Registers.PC >> 8) & 0xff);
            byte pcL = (byte)(Registers.PC & 0xff);

            Registers.SP--;
            _memory.Write(Registers.SP, pcH);
            Registers.SP--;
            _memory.Write(Registers.SP, pcL);
        }
    }
}