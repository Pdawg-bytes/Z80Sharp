using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
    }
}
