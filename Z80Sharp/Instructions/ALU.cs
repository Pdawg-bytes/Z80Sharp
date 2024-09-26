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
            LogInstructionExec($"0x{_currentInstruction:X2}: INC {Registers.RegisterName(operatingRegister, true)}");
        }
        private void INC_R(byte operatingRegister)
        {
            Registers.RegisterSet[operatingRegister] = INCWithFlags(Registers.RegisterSet[operatingRegister]);
            LogInstructionExec($"0x{_currentInstruction:X2}: INC {Registers.RegisterName(operatingRegister)}");
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
            LogInstructionExec($"0x{_currentInstruction:X2}: DEC {Registers.RegisterName(operatingRegister, true)}");
        }
        private void DEC_R(byte operatingRegister)
        {
            Registers.RegisterSet[operatingRegister] = DECWithFlags(Registers.RegisterSet[operatingRegister]);
            LogInstructionExec($"0x{_currentInstruction:X2}: DEC {Registers.RegisterName(operatingRegister)}");
        }
        private void DEC_HLMEM()
        {
            _memory.Write(Registers.HL, DECWithFlags(_memory.Read(Registers.HL)));
            LogInstructionExec($"0x35: DEC (HL:0x{Registers.HL:X4})");
        }

        private void OR_R(byte operatingRegister)
        {
            ORAny(Registers.RegisterSet[operatingRegister]);
            LogInstructionExec($"0x{_currentInstruction:X2}: OR {Registers.RegisterName(operatingRegister)}");
        }
        private void OR_N()
        {
            ORAny(Fetch());
            LogInstructionExec($"0xF6: OR N:0x{FetchLast():X2}");
        }
        private void OR_RRMEM(byte operatingRegister)
        {
            ORAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            LogInstructionExec($"0x{_currentInstruction:X2}: OR ({Registers.RegisterName(operatingRegister, true)}:0x{Registers.GetR16FromHighIndexer(operatingRegister):X4})");
        }

        private void XOR_R(byte operatingRegister)
        {
            XORAny(Registers.RegisterSet[operatingRegister]);
            LogInstructionExec($"0x{_currentInstruction:X2}: XOR {Registers.RegisterName(operatingRegister)}");
        }
        private void XOR_N()
        {
            XORAny(Fetch());
            LogInstructionExec($"0xEE: XOR N:0x{FetchLast():X2}");
        }
        private void XOR_RRMEM(byte operatingRegister)
        {
            XORAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            LogInstructionExec($"0x{_currentInstruction:X2}: XOR ({Registers.RegisterName(operatingRegister, true)}:0x{Registers.GetR16FromHighIndexer(operatingRegister):X4})");
        }

        private void AND_R(byte operatingRegister)
        {
            ANDAny(Registers.RegisterSet[operatingRegister]);
            LogInstructionExec($"0x{_currentInstruction:X2}: AND {Registers.RegisterName(operatingRegister)}");
        }
        private void AND_N()
        {
            ANDAny(Fetch());
            LogInstructionExec($"0xE6: XOR N:0x{FetchLast():X2}");
        }
        private void AND_RRMEM(byte operatingRegister)
        {
            ANDAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            LogInstructionExec($"0x{_currentInstruction:X2}: AND ({Registers.RegisterName(operatingRegister, true)}:0x{Registers.GetR16FromHighIndexer(operatingRegister):X4})");
        }

        private void CMP_R(byte operatingRegister)
        {
            CMPAny(Registers.RegisterSet[operatingRegister]);
            LogInstructionExec($"0x{_currentInstruction:X2}: CP {Registers.RegisterName(operatingRegister)}");
        }
        private void CMP_N()
        {
            CMPAny(Fetch());
            LogInstructionExec($"0xFE: CP N:0x{FetchLast():X2}");
        }
        private void CMP_RRMEM(byte operatingRegister)
        {
            CMPAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            LogInstructionExec($"0x{_currentInstruction:X2}: CP ({Registers.RegisterName(operatingRegister, true)}:0x{Registers.GetR16FromHighIndexer(operatingRegister):X4})");
        }

        private void ADD_A_R(byte operatingRegister)
        {
            ADDAny(Registers.RegisterSet[operatingRegister]);
            LogInstructionExec($"0x{_currentInstruction:X2}: ADD {Registers.RegisterName(operatingRegister)}");
        }
        private void ADD_A_N()
        {
            ADDAny(Fetch());
            LogInstructionExec($"0xC6: ADD N:0x{FetchLast():X2}");
        }
        private void ADD_A_RRMEM(byte operatingRegister)
        {
            ADDAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            LogInstructionExec($"0x{_currentInstruction:X2}: ADD ({Registers.RegisterName(operatingRegister, true)}:0x{Registers.GetR16FromHighIndexer(operatingRegister):X4})");
        }
        private void ADD_HL_RR(byte operatingRegister)
        {
            Registers.HL = ADDWord(Registers.HL, Registers.GetR16FromHighIndexer(operatingRegister));
            LogInstructionExec($"0x{_currentInstruction}: ADD HL, {Registers.RegisterName(operatingRegister, true)}");
        }

        private void SUB_R(byte operatingRegister)
        {
            SUBAny(Registers.RegisterSet[operatingRegister]);
            LogInstructionExec($"0x{_currentInstruction:X2}: SUB {Registers.RegisterName(operatingRegister)}");
        }
        private void SUB_N()
        {
            SUBAny(Fetch());
            LogInstructionExec($"0xD6: SUB N:0x{FetchLast():X2}");
        }
        private void SUB_RRMEM(byte operatingRegister)
        {
            SUBAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            LogInstructionExec($"0x{_currentInstruction:X2}: SUB ({Registers.RegisterName(operatingRegister, true)}:0x{Registers.GetR16FromHighIndexer(operatingRegister):X4})");
        }
    }
}