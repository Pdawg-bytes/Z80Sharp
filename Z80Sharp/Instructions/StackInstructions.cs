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
            ushort sp = Registers.SP;
            Registers.RegisterSet[operatingRegister + 1] = _memory.Read(sp++);
            Registers.RegisterSet[operatingRegister] = _memory.Read(sp++);
            Registers.SP = sp;
            LogInstructionExec($"0x{_currentInstruction}: POP {Registers.RegisterName(operatingRegister, true)}");
        }
        // We can't use the generic method because the register indexers for A and F are in the opposite order.
        private void POP_AF()
        {
            ushort sp = Registers.SP;
            Registers.RegisterSet[F] = _memory.Read(sp++);
            Registers.RegisterSet[A] = _memory.Read(sp++);
            Registers.SP = sp;
            LogInstructionExec($"0xF1: POP AF");
        }

        private void PUSH_RR(byte operatingRegister)
        {
            ushort sp = Registers.SP;
            _memory.Write(--sp, Registers.RegisterSet[operatingRegister]);
            _memory.Write(--sp, Registers.RegisterSet[operatingRegister + 1]);
            Registers.SP = sp;
            LogInstructionExec($"0x{_currentInstruction}: PUSH {Registers.RegisterName(operatingRegister, true)}");
        }
        // We can't use the generic method because the register indexers for A and F are in the opposite order.
        private void PUSH_AF()
        {
            ushort sp = Registers.SP;
            _memory.Write(--sp, Registers.RegisterSet[A]);
            _memory.Write(--sp, Registers.RegisterSet[F]);
            Registers.SP = sp;
            LogInstructionExec($"0xF5: PUSH AF");
        }
        private void PUSH_PC()
        {
            ushort sp = Registers.SP;
            _memory.Write(--sp, Registers.RegisterSet[PCi]);
            _memory.Write(--sp, Registers.RegisterSet[PCi + 1]);
            Registers.SP = sp;
        }
    }
}
