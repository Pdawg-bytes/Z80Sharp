﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Helpers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void NOP()
        {
            //LogInstructionExec("0x00: NOP");
        }

        private void EX_AF_AF_()
        {
            Registers.R8Exchange(A_, A);
            Registers.R8Exchange(F_, F);
            LogInstructionExec("0x08: EX AF, AF'");
        }
        private void EX_DE_HL()
        {
            Registers.R8Exchange(D, H);
            Registers.R8Exchange(E, L);
            LogInstructionExec("0x08: EX DE, HL");
        }
        private void EX_SPMEM_HL()
        {
            ushort sp = Registers.SP;
            ushort oldHl = Registers.GetR16FromHighIndexer(H);
            Registers.HL = _memory.ReadWord(sp);
            _memory.WriteWord(sp, oldHl);
            LogInstructionExec("0xE3: EX (SP), HL");
        }
        private void EXX()
        {
            Registers.R16Exchange(B, B_);
            Registers.R16Exchange(D, D_);
            Registers.R16Exchange(H, H_);
            LogInstructionExec("0xD9: EXX");
        }

        private void DI()
        {
            Registers.IFF1 = Registers.IFF2 = false;
            LogInstructionExec("0xF3: DI");
        }
        private void EI()
        {
            Registers.IFF1 = Registers.IFF2 = true;
            LogInstructionExec("0xFB: EI");
        }
    }
}
