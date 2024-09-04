using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void NOP()
        {
            LogInstructionExec("0x00: NOP");
        }


        private void LD_BC_NN()
        {
            ushort word = FetchImmediateWord();
            Registers.BC = word;
            LogInstructionExec($"0x01: LD BC, 0x{word:X4}");
            // wait states
        }
        private void LD_DE_NN()
        {
            ushort word = FetchImmediateWord();
            Registers.DE = word;
            LogInstructionExec($"0x11: LD DE, 0x{word:X4}");
            // wait states
        }
        private void LD_HL_NN()
        {
            ushort word = FetchImmediateWord();
            Registers.HL = word;
            LogInstructionExec($"0x21: LD HL, 0x{word:X4}");
            // wait states
        }
        private void LD_SP_NN()
        {
            ushort word = FetchImmediateWord();
            Registers.SP = word;
            LogInstructionExec($"0x31: LD SP, 0x{word:X4}");
            // wait states
        }


        private void LD_B_N()
        {
            byte n = Fetch();
            Registers.RegisterSet[B] = n;
            LogInstructionExec($"0x06: LD B, 0x{n:X2}");
        }
        private void LD_D_N()
        {
            byte n = Fetch();
            Registers.RegisterSet[D] = n;
            LogInstructionExec($"0x16: LD D, 0x{n:X2}");
        }
        private void LD_H_N()
        {
            byte n = Fetch();
            Registers.RegisterSet[H] = n;
            LogInstructionExec($"0x26: LD H, 0x{n:X2}");
        }
        private void LD_C_N()
        {
            byte n = Fetch();
            Registers.RegisterSet[C] = n;
            LogInstructionExec($"0x0E: LD C, 0x{n:X2}");
        }
        private void LD_E_N()
        {
            byte n = Fetch();
            Registers.RegisterSet[E] = n;
            LogInstructionExec($"0x1E: LD E, 0x{n:X2}");
        }
        private void LD_L_N()
        {
            byte n = Fetch();
            Registers.RegisterSet[L] = n;
            LogInstructionExec($"0x2E: LD L, 0x{n:X2}");
        }
        private void LD_A_N()
        {
            byte n = Fetch();
            Registers.RegisterSet[A] = n;
            LogInstructionExec($"0x3E: LD A, 0x{n:X2}");
        }

        private void LD_HLMEM_N()
        {
            byte n = Fetch();
            _memory.Write(Registers.HL, n);
            LogInstructionExec($"0x36: LD ({Registers.HL:X2}), 0x{n:X2}");
        }
        private void LD_B_HLMEM()
        {
            byte val = _memory.Read(Registers.HL);
            Registers.RegisterSet[B] = val;
            LogInstructionExec($"0x46: LD B, (0x{Registers.HL:X2}):0x{val:X2}");
        }
    }
}