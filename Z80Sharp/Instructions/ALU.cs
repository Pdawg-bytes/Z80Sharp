using System;
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
        private void INC_RR(byte operatingRegister)
        {
            ushort value = (ushort)(Registers.GetR16FromHighIndexer(operatingRegister) + 1);
            Registers.RegisterSet[operatingRegister] = value.GetUpperByte();
            Registers.RegisterSet[operatingRegister + 1] = value.GetLowerByte();
            LogInstructionExec($"0x{_currentInstruction}: INC {Registers.RegisterName(operatingRegister, true)}");
        }
        private void INC_R(byte operatingRegister)
        {
            Registers.RegisterSet[operatingRegister] = INCWithFlags(Registers.RegisterSet[operatingRegister]);
            LogInstructionExec($"0x{_currentInstruction}: INC {Registers.RegisterName(operatingRegister)}");
        }
        private void INC_HLMEM()
        {
            _memory.Write(Registers.HL, INCWithFlags(_memory.Read(Registers.HL)));
            LogInstructionExec($"0x34: INC (HL:0x{Registers.HL:X4})");
        }

        private void DEC_RR(byte operatingRegister)
        {
            ushort value = (ushort)(Registers.GetR16FromHighIndexer(operatingRegister) - 1);
            Registers.RegisterSet[operatingRegister] = value.GetUpperByte();
            Registers.RegisterSet[operatingRegister + 1] = value.GetLowerByte();
            LogInstructionExec($"0x{_currentInstruction}: DEC {Registers.RegisterName(operatingRegister, true)}");
        }
        private void DEC_R(byte operatingRegister)
        {
            Registers.RegisterSet[operatingRegister] = DECWithFlags(Registers.RegisterSet[operatingRegister]);
            LogInstructionExec($"0x{_currentInstruction}: DEC {Registers.RegisterName(operatingRegister)}");
        }
        private void DEC_HLMEM()
        {
            _memory.Write(Registers.HL, DECWithFlags(_memory.Read(Registers.HL)));
            LogInstructionExec($"0x35: DEC (HL:0x{Registers.HL:X4})");
        }
    }
}