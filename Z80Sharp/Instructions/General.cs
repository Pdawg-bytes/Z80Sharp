using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void NOP()
        {
            LogInstructionExec("0x00: NOP");
        }


        private void LD_RR_NN(byte operatingRegister)
        {
            Registers.RegisterSet[operatingRegister + 1] = Fetch();
            Registers.RegisterSet[operatingRegister] = Fetch();
            LogInstructionExec($"0x{_currentInstruction:X2}: LD {Registers.RegisterName(operatingRegister, true)}, 0x{Registers.RegisterSet[operatingRegister]:X2}{Registers.RegisterSet[operatingRegister + 1]:X2}");
            // wait states
        }
        private void LD_SP_NN()
        {
            ushort word = FetchImmediateWord();
            Registers.SP = word;
            LogInstructionExec($"0x31: LD SP, 0x{word:X4}");
            // wait states
        }


        private void LD_R_N(byte operatingRegister)
        {
            byte n = Fetch();
            Registers.RegisterSet[operatingRegister] = n;
            LogInstructionExec($"0x{_currentInstruction:X2}: LD {Registers.RegisterName(operatingRegister)}, 0x{Registers.RegisterSet[operatingRegister]:X2}");
        }


        private void LD_HLMEM_N()
        {
            byte n = Fetch();
            _memory.Write(Registers.HL, n);
            LogInstructionExec($"0x36: LD (HL:0x{Registers.HL:X4}), 0x{n:X2}");
        }
        private void LD_B_HLMEM()
        {
            byte val = _memory.Read(Registers.HL);
            Registers.RegisterSet[B] = val;
            LogInstructionExec($"0x46: LD B, (HL:0x{Registers.HL:X4}):0x{val:X2}");
        }
    }
}