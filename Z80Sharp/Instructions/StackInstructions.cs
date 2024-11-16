using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void POP_RR(byte operatingRegister)
        {
            Registers.RegisterSet[operatingRegister + 1] = _memory.Read(Registers.SP);
            Registers.SP++;
            Registers.RegisterSet[operatingRegister] = _memory.Read(Registers.SP);
            Registers.SP++;
            //LogInstructionExec($"0x{_currentInstruction:X2}: POP {Registers.RegisterName(operatingRegister, true)}");
        }
        // We can't use the generic method because the register indexers for A and F are in the opposite order.
        private void POP_AF()
        {
            Registers.RegisterSet[F] = _memory.Read(Registers.SP);
            Registers.SP++;
            Registers.RegisterSet[A] = _memory.Read(Registers.SP);
            Registers.SP++;
            //LogInstructionExec($"0xF1: POP AF");
        }
        private void POP_PC_SILENT()
        {
            byte pcL = _memory.Read(Registers.SP);
            Registers.SP++;
            byte pcH = _memory.Read(Registers.SP);
            Registers.SP++;

            Registers.PC = (ushort)((pcH << 8) + pcL);
        }

        private void PUSH_RR(byte operatingRegister)
        {
            Registers.SP--;
            _memory.Write(Registers.SP, Registers.RegisterSet[operatingRegister]);
            Registers.SP--;
            _memory.Write(Registers.SP, Registers.RegisterSet[operatingRegister + 1]);
            //LogInstructionExec($"0x{_currentInstruction:X2}: PUSH {Registers.RegisterName(operatingRegister, true)}");
        }
        // We can't use the generic method because the register indexers for A and F are in the opposite order.
        private void PUSH_AF()
        {
            Registers.SP--;
            _memory.Write(Registers.SP, Registers.RegisterSet[A]);
            Registers.SP--;
            _memory.Write(Registers.SP, Registers.RegisterSet[F]);
            //LogInstructionExec($"0xF5: PUSH AF");
        }
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
