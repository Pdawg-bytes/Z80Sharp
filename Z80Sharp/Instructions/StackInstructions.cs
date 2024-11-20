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
        private ushort tempSP;
        private void POP_RR([ConstantExpected] byte operatingRegister)
        {
            tempSP = Registers.SP;
            Registers.RegisterSet[operatingRegister + 1] = _memory.Read(tempSP);
            tempSP++;
            Registers.RegisterSet[operatingRegister] = _memory.Read(tempSP);
            tempSP++;
            Registers.SP = tempSP;
            //LogInstructionExec($"0x{_currentInstruction:X2}: POP {Registers.RegisterName(operatingRegister, true)}");
        }
        // We can't use the generic method because the register indexers for A and F are in the opposite order.
        private void POP_AF()
        {
            tempSP = Registers.SP;
            Registers.RegisterSet[F] = _memory.Read(tempSP);
            tempSP++;
            Registers.RegisterSet[A] = _memory.Read(tempSP);
            tempSP++;
            Registers.SP = tempSP;
            //LogInstructionExec($"0xF1: POP AF");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void POP_PC_SILENT()
        {
            tempSP = Registers.SP;
            byte pcL = _memory.Read(tempSP);
            tempSP++;
            byte pcH = _memory.Read(tempSP);
            tempSP++;

            Registers.SP = tempSP;
            Registers.PC = (ushort)((pcH << 8) | pcL);
        }

        private void PUSH_RR([ConstantExpected] byte operatingRegister)
        {
            tempSP = Registers.SP;
            tempSP--;
            _memory.Write(tempSP, Registers.RegisterSet[operatingRegister]);
            tempSP--;
            _memory.Write(tempSP, Registers.RegisterSet[operatingRegister + 1]);
            Registers.SP = tempSP;
            //LogInstructionExec($"0x{_currentInstruction:X2}: PUSH {Registers.RegisterName(operatingRegister, true)}");
        }
        // We can't use the generic method because the register indexers for A and F are in the opposite order.
        private void PUSH_AF()
        {
            tempSP = Registers.SP;
            tempSP--;
            _memory.Write(tempSP, Registers.RegisterSet[A]);
            tempSP--;
            _memory.Write(tempSP, Registers.RegisterSet[F]);
            Registers.SP = tempSP;
            //LogInstructionExec($"0xF5: PUSH AF");
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void PUSH_PC_SILENT()
        {
            tempSP = Registers.SP;
            byte pcH = (byte)((Registers.PC >> 8) & 0xff);
            byte pcL = (byte)(Registers.PC & 0xff);

            tempSP--;
            _memory.Write(tempSP, pcH);
            tempSP--;
            _memory.Write(tempSP, pcL);
            Registers.SP = tempSP;
        }
    }
}
