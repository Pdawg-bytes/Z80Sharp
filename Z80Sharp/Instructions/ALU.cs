﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using Z80Sharp.Helpers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        // Reference: https://stackoverflow.com/questions/8119577/z80-daa-instruction
        private void DAA()
        {
            int t = 0;
            byte regA = Registers.RegisterSet[A];

            if (Registers.IsFlagSet(FlagType.H) || (regA & 0xF) > 9)
                t++;

            if (Registers.IsFlagSet(FlagType.C) || regA > 0x99)
            {
                t += 2;
                Registers.SetFlag(FlagType.C);
            }

            if (Registers.IsFlagSet(FlagType.N))
            {
                Registers.SetFlagConditionally(FlagType.H, Registers.IsFlagSet(FlagType.H) && (regA & 0x0F) < 6);
            }
            else
            {
                Registers.SetFlagConditionally(FlagType.H, (regA & 0x0F) >= 0x0A);
            }

            Registers.RegisterSet[A] += (byte)(t == 1 ? (Registers.IsFlagSet(FlagType.N) ? 0xFA : 0x06) :
                                               t == 2 ? (Registers.IsFlagSet(FlagType.N) ? 0xA0 : 0x60) :
                                               t == 3 ? (Registers.IsFlagSet(FlagType.N) ? 0x9A : 0x66) : 0);

            regA = Registers.RegisterSet[A];

            Registers.SetFlagConditionally(FlagType.S, (regA & 0x80) != 0);   // (S) Sign flag
            Registers.SetFlagConditionally(FlagType.Z, regA == 0);            // (Z) Zero flag
            Registers.SetFlagConditionally(FlagType.PV, FlagHelpers.CheckParity(regA)); // (P/V) Parity flag
            Registers.SetFlagConditionally(FlagType.X, (regA & 0x20) != 0);   // (X) Undocumented flag
            Registers.SetFlagConditionally(FlagType.Y, (regA & 0x08) != 0);   // (Y) Undocumented flag
            LogInstructionExec("0x27: DAA");
        }

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

        private void ADC_A_R(byte operatingRegister)
        {
            ADCAny(Registers.RegisterSet[operatingRegister]);
            LogInstructionExec($"0x{_currentInstruction:X2}: ADC {Registers.RegisterName(operatingRegister)}");
        }
        private void ADC_A_N()
        {
            ADCAny(Fetch());
            LogInstructionExec($"0xCE: ADC N:0x{FetchLast():X2}");
        }
        private void ADC_A_RRMEM(byte operatingRegister)
        {
            ADCAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            LogInstructionExec($"0x{_currentInstruction:X2}: ADC ({Registers.RegisterName(operatingRegister, true)}:0x{Registers.GetR16FromHighIndexer(operatingRegister):X4})");
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

        private void SBC_A_R(byte operatingRegister)
        {
            SBCAny(Registers.RegisterSet[operatingRegister]);
            LogInstructionExec($"0x{_currentInstruction:X2}: SBC {Registers.RegisterName(operatingRegister)}");
        }
        private void SBC_A_N()
        {
            SBCAny(Fetch());
            LogInstructionExec($"0xDE: SBC N:0x{FetchLast():X2}");
        }
        private void SBC_A_RRMEM(byte operatingRegister)
        {
            SBCAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            LogInstructionExec($"0x{_currentInstruction:X2}: SBC ({Registers.RegisterName(operatingRegister, true)}:0x{Registers.GetR16FromHighIndexer(operatingRegister):X4})");
        }
    }
}