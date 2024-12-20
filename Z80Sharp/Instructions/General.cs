using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using Z80Sharp.Helpers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        private void NOP()
        {
            //LogInstructionExec("0x00: NOP");
        }
        private void HALT()
        {
            //LogInstructionExec("0x76: HALT");
            if (!Halted)
            {
                Halted = true;
                Registers.PC--;
            }
        }

        private void CPL()
        {
            Registers.A ^= 0b11111111;
            Registers.SetFlagBits((byte)(FlagType.N | FlagType.H));
            //LogInstructionExec("0x2F: CPL");
        }

        private void CCF()
        {
            Registers.ClearFlag(FlagType.N);
            // H is cleared to allow it to be 0. SetFlagBits uses |= which won't overwrite any 1s.
            Registers.ClearFlag(FlagType.H);
            Registers.SetFlagBits((byte)((Registers.F << 4) & 0b00010000)); // Set H to C pre-inversion
            Registers.InvertFlag(FlagType.C);
            //LogInstructionExec("0x3F: CCF");
        }
        private void SCF()
        {
            Registers.SetFlag(FlagType.C);
            Registers.ClearFlag(FlagType.N);
            Registers.ClearFlag(FlagType.H);
            //LogInstructionExec("0x37: SCF");
        }

        private void EX_AF_AF_()
        {
            Registers.R16Exchange(ref Registers.AF, ref Registers.AF_);
            //LogInstructionExec("0x08: EX AF, AF'");
        }
        private void EX_DE_HL()
        {
            Registers.R8Exchange(ref Registers.D, ref Registers.H);
            Registers.R8Exchange(ref Registers.E, ref Registers.L);
            //LogInstructionExec("0xEB: EX DE, HL");
        }
        private void EX_SPMEM_HL()
        {
            ushort sp = Registers.SP;
            ushort oldHl = Registers.HL;
            Registers.HL = _memory.ReadWord(sp);
            _memory.WriteWord(sp, oldHl);
            //LogInstructionExec("0xE3: EX (SP), HL");
        }
        private void EX_SPMEM_IR(ref ushort indexAddressingMode)
        {
            ushort sp = Registers.SP;
            ushort oldIr = indexAddressingMode;
            indexAddressingMode = _memory.Read(sp++);
            _memory.WriteWord(sp, oldIr);
            //LogInstructionExec("0xE3: EX (SP), HL");
        }
        private void EXX()
        {
            Registers.R16Exchange(ref Registers.BC, ref Registers.BC);
            Registers.R16Exchange(ref Registers.DE, ref Registers.DE_);
            Registers.R16Exchange(ref Registers.HL, ref Registers.HL_);
            //LogInstructionExec("0xD9: EXX");
        }

        private void DI()
        {
            Registers.IFF1 = Registers.IFF2 = false;
            //LogInstructionExec("0xF3: DI");
        }
        private void EI()
        {
            Registers.IFF1 = Registers.IFF2 = true;
            //LogInstructionExec("0xFB: EI");
        }

        private void IM_M(InterruptMode mode)
        {
            Registers.InterruptMode = mode;
            //LogInstructionExec($"0x{_currentInstruction:X2}: {Registers.InterruptModeName(mode)}");
        }
    }
}
