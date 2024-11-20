using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using Z80Sharp.Helpers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        // Reference: http://www.z80.info/zip/z80-documented.pdf (Page 18)
        /*private void DAA()
        {
            byte regA = Registers.RegisterSet[A];
            byte adjustment = 0;
            bool flagN = Registers.IsFlagSet(FlagType.N);

            // Carry over from first nibble
            if ((!flagN && (regA & 0x0F) > 0x09) || Registers.IsFlagSet(FlagType.H))
            {
                adjustment |= 0x06;
                Registers.SetFlagConditionally(FlagType.H, ((regA & 0x0F) + 0x06) > 0x0F); // If our adjustment carries over into top 4 bits
            }

            // Full carry
            if ((regA > 0x99) && !flagN || Registers.IsFlagSet(FlagType.C))
            {
                adjustment |= 0x60;
                Registers.SetFlag(FlagType.C);
            }
            else
                Registers.ClearFlag(FlagType.C);

            Registers.RegisterSet[A] += flagN ? (byte)-adjustment : adjustment;
            regA = Registers.RegisterSet[A];

            Registers.SetFlagConditionally(FlagType.S, (regA & 0x80) != 0);             // (S)  (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, regA == 0);                      // (Z)  (Set if result is zero)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(regA));             // (PV) (Set if bit parity is even)
            Registers.SetFlagConditionally(FlagType.X, (regA & 0x20) > 0);              // (X)  (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (regA & 0x08) > 0);              // (Y)  (Undocumented flag)

            //LogInstructionExec("0x27: DAA");
        }*/
        // Reference: https://stackoverflow.com/questions/8119577/z80-daa-instruction
        private void DAA()
        {
            int t;

            t = 0;

            if (Registers.IsFlagSet(FlagType.H) || ((A & 0xF) > 9))
                t++;

            if (Registers.IsFlagSet(FlagType.C) || (A > 0x99))
            {
                t += 2;
                Registers.SetFlag(FlagType.C);
            }

            if (Registers.IsFlagSet(FlagType.N) && !Registers.IsFlagSet(FlagType.H))
                Registers.ClearFlag(FlagType.H);
            else
            {
                if (Registers.IsFlagSet(FlagType.N) && Registers.IsFlagSet(FlagType.H))
                    Registers.SetFlagConditionally(FlagType.H, (Registers.RegisterSet[A] & 0x0F) < 6);
                else
                    Registers.SetFlagConditionally(FlagType.H, (Registers.RegisterSet[A] & 0x0F) >= 0x0A);
            }

            switch (t)
            {
                case 1:
                    Registers.RegisterSet[A] += Registers.IsFlagSet(FlagType.N) ? (byte)0xFA : (byte)0x06; // -6:6
                    break;
                case 2:
                    Registers.RegisterSet[A] += Registers.IsFlagSet(FlagType.N) ? (byte)0xA0 : (byte)0x60; // -0x60:0x60
                    break;
                case 3:
                    Registers.RegisterSet[A] += Registers.IsFlagSet(FlagType.N) ? (byte)0x9A : (byte)0x66; // -0x66:0x66
                    break;
            }

            Registers.SetFlagConditionally(FlagType.S, (Registers.RegisterSet[A] & 0x80) != 0);             // (S)  (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, Registers.RegisterSet[A] == 0);                      // (Z)  (Set if result is zero)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(Registers.RegisterSet[A]));             // (PV) (Set if bit parity is even)
            Registers.SetFlagConditionally(FlagType.X, (Registers.RegisterSet[A] & 1 << 5) > 0);            // (X)  (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (Registers.RegisterSet[A] & 1 << 3) > 0);            // (Y)  (Undocumented flag)
        }

        private void NEG()
        {
            byte value = Registers.RegisterSet[A];
            int result = 0 - value;
            Registers.RegisterSet[A] = (byte)result;

            Registers.SetFlagConditionally(FlagType.S, (result & 0x80) != 0);           // (S)  (Set if result is negative)
            Registers.SetFlagConditionally(FlagType.Z, Registers.RegisterSet[A] == 0);  // (Z)  (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.H, (value & 0x0F) != 0);            // (H)  (Set if borrow from bit 4)
            Registers.SetFlagConditionally(FlagType.PV, value == 0x80);                 // (PV) (Set if A == -128 (sbyte.Min)
            Registers.SetFlagConditionally(FlagType.C, value != 0);                     // (C)  (Set if borrow occured)
            Registers.SetFlag(FlagType.N);                                              // (N)  (Unconditionally set)

            Registers.SetFlagConditionally(FlagType.X, (result & 0x20) > 0);            // (X)  (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x08) > 0);            // (Y)  (Undocumented flag)

            //LogInstructionExec("0x44: NEG");
        }

        #region INC instructions (RR, R, (HL), (IR + d))
        private void INC_RR([ConstantExpected] byte operatingRegister)
        {
            ushort value = (ushort)(Registers.GetR16FromHighIndexer(operatingRegister) + 1);
            Registers.RegisterSet[operatingRegister] = value.GetUpperByte();
            Registers.RegisterSet[operatingRegister + 1] = value.GetLowerByte();
            //LogInstructionExec($"0x{_currentInstruction:X2}: INC {Registers.RegisterName(operatingRegister, true)}");
        }
        private void INC_R([ConstantExpected] byte operatingRegister)
        {
            Registers.RegisterSet[operatingRegister] = INCAny(Registers.RegisterSet[operatingRegister]);
            //LogInstructionExec($"0x{_currentInstruction:X2}: INC {Registers.RegisterName(operatingRegister)}");
        }
        private void INC_HLMEM()
        {
            _memory.Write(Registers.HL, INCAny(_memory.Read(Registers.HL)));
            //LogInstructionExec($"0x34: INC (HL)");
        }
        private void INC_IRDMEM([ConstantExpected] byte indexAddressingMode)
        {
            ushort addr = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + (sbyte)Fetch());
            _memory.Write(addr, INCAny(_memory.Read(addr)));
            //LogInstructionExec($"0x34: INC ({Registers.RegisterName(indexAddressingMode, true)} + d)");
        }
        #endregion

        #region DEC instructions (RR, R, (HL), (IR + d))
        private void DEC_RR([ConstantExpected] byte operatingRegister)
        {
            ushort value = (ushort)(Registers.GetR16FromHighIndexer(operatingRegister) - 1);
            Registers.RegisterSet[operatingRegister] = value.GetUpperByte();
            Registers.RegisterSet[operatingRegister + 1] = value.GetLowerByte();
            //LogInstructionExec($"0x{_currentInstruction:X2}: DEC {Registers.RegisterName(operatingRegister, true)}");
        }
        private void DEC_R([ConstantExpected] byte operatingRegister)
        {
            Registers.RegisterSet[operatingRegister] = DECAny(Registers.RegisterSet[operatingRegister]);
            //LogInstructionExec($"0x{_currentInstruction:X2}: DEC {Registers.RegisterName(operatingRegister)}");
        }
        private void DEC_HLMEM()
        {
            _memory.Write(Registers.HL, DECAny(_memory.Read(Registers.HL)));
            //LogInstructionExec($"0x35: DEC (HL)");
        }
        private void DEC_IRDMEM([ConstantExpected] byte indexAddressingMode)
        {
            ushort addr = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + (sbyte)Fetch());
            _memory.Write(addr, DECAny(_memory.Read(addr)));
            //LogInstructionExec($"0x35: DEC ({Registers.RegisterName(indexAddressingMode, true)} + d)");
        }
        #endregion


        #region OR instructions (R, N, (RR), (IR + d))
        private void OR_R([ConstantExpected] byte operatingRegister)
        {
            ORAny(Registers.RegisterSet[operatingRegister]);
            //LogInstructionExec($"0x{_currentInstruction:X2}: OR {Registers.RegisterName(operatingRegister)}");
        }
        private void OR_N()
        {
            ORAny(Fetch());
            //LogInstructionExec($"0xF6: OR N:0x{FetchLast():X2}");
        }
        private void OR_RRMEM([ConstantExpected] byte operatingRegister)
        {
            ORAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            //LogInstructionExec($"0x{_currentInstruction:X2}: OR ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void OR_IRDMEM([ConstantExpected] byte indexAddressingMode)
        {
            ORAny(_memory.Read((ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: OR ({Registers.RegisterName(indexAddressingMode, true)})");
        }
        #endregion

        #region XOR instructions (R, N, (RR), (IR + d))
        private void XOR_R([ConstantExpected] byte operatingRegister)
        {
            XORAny(Registers.RegisterSet[operatingRegister]);
            //LogInstructionExec($"0x{_currentInstruction:X2}: XOR {Registers.RegisterName(operatingRegister)}");
        }
        private void XOR_N()
        {
            XORAny(Fetch());
            //LogInstructionExec($"0xEE: XOR N:0x{FetchLast():X2}");
        }
        private void XOR_RRMEM([ConstantExpected] byte operatingRegister)
        {
            XORAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            //LogInstructionExec($"0x{_currentInstruction:X2}: XOR ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void XOR_IRDMEM([ConstantExpected] byte indexAddressingMode)
        {
            XORAny(_memory.Read((ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: XOR ({Registers.RegisterName(indexAddressingMode, true)})");
        }
        #endregion


        #region AND instructions (R, N, (RR), (IR + d))
        private void AND_R([ConstantExpected] byte operatingRegister)
        {
            ANDAny(Registers.RegisterSet[operatingRegister]);
            //LogInstructionExec($"0x{_currentInstruction:X2}: AND {Registers.RegisterName(operatingRegister)}");
        }
        private void AND_N()
        {
            ANDAny(Fetch());
            //LogInstructionExec($"0xE6: XOR N:0x{FetchLast():X2}");
        }
        private void AND_RRMEM([ConstantExpected] byte operatingRegister)
        {
            ANDAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            //LogInstructionExec($"0x{_currentInstruction:X2}: AND ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void AND_IRDMEM([ConstantExpected] byte indexAddressingMode)
        {
            ANDAny(_memory.Read((ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: AND ({Registers.RegisterName(indexAddressingMode, true)})");
        }
        #endregion


        #region CP instructions (R, N, (RR), (IR + d))
        private void CMP_R([ConstantExpected] byte operatingRegister)
        {
            CMPAny(Registers.RegisterSet[operatingRegister]);
            //LogInstructionExec($"0x{_currentInstruction:X2}: CP {Registers.RegisterName(operatingRegister)}");
        }
        private void CMP_N()
        {
            CMPAny(Fetch());
            //LogInstructionExec($"0xFE: CP N:0x{FetchLast():X2}");
        }
        private void CMP_RRMEM([ConstantExpected] byte operatingRegister)
        {
            CMPAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            //LogInstructionExec($"0x{_currentInstruction:X2}: CP ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void CMP_IRDMEM([ConstantExpected] byte indexAddressingMode)
        {
            CMPAny(_memory.Read((ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: CP ({Registers.RegisterName(indexAddressingMode, true)})");
        }

        private void CPI()
        {
            byte hlMem = _memory.Read(Registers.HL);
            sbyte diff = (sbyte)(Registers.RegisterSet[A] - hlMem);

            Registers.HL++;
            Registers.BC--;

            Registers.SetFlagConditionally(FlagType.S, (byte)diff > 0x7f);
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);
            Registers.SetFlagConditionally(FlagType.H, (Registers.RegisterSet[A] & 0x0F) < (hlMem & 0x0F));
            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0);
            Registers.SetFlag(FlagType.N);

            int undoc = Registers.RegisterSet[A] - hlMem - (Registers.RegisterSet[F] & (byte)FlagType.H);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x02) > 0);  // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0);  // (Y) (Undocumented flag)

            //LogInstructionExec("0xA1: CPI");
        }
        private void CPIR()
        {
            byte hlMem = _memory.Read(Registers.HL);
            sbyte diff = (sbyte)(Registers.RegisterSet[A] - hlMem);

            Registers.HL++;
            Registers.BC--;

            Registers.SetFlagConditionally(FlagType.S, (byte)diff > 0x7f);
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);
            Registers.SetFlagConditionally(FlagType.H, (Registers.RegisterSet[A] & 0x0F) < (hlMem & 0x0F));
            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0);
            Registers.SetFlag(FlagType.N);

            int undoc = Registers.RegisterSet[A] - hlMem - (Registers.RegisterSet[F] & (byte)FlagType.H);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x02) > 0);  // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0);  // (Y) (Undocumented flag)

            if (Registers.BC != 0 && diff != 0)
            {
                Registers.PC -= 2;
            }

            //LogInstructionExec("0xB1: CPIR");
        }
        private void CPD()
        {
            byte hlMem = _memory.Read(Registers.HL);
            sbyte diff = (sbyte)(Registers.RegisterSet[A] - hlMem);

            Registers.HL--;
            Registers.BC--;

            Registers.SetFlagConditionally(FlagType.S, (byte)diff > 0x7f);
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);
            Registers.SetFlagConditionally(FlagType.H, (Registers.RegisterSet[A] & 0x0F) < (hlMem & 0x0F));
            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0);
            Registers.SetFlag(FlagType.N);

            int undoc = Registers.RegisterSet[A] - hlMem - (Registers.RegisterSet[F] & (byte)FlagType.H);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x02) > 0);  // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0);  // (Y) (Undocumented flag)

            //LogInstructionExec("0xA9: CPI");
        }
        private void CPDR()
        {
            byte hlMem = _memory.Read(Registers.HL);
            sbyte diff = (sbyte)(Registers.RegisterSet[A] - hlMem);

            Registers.HL--;
            Registers.BC--;

            Registers.SetFlagConditionally(FlagType.S, (byte)diff > 0x7f);
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);
            Registers.SetFlagConditionally(FlagType.H, (Registers.RegisterSet[A] & 0x0F) < (hlMem & 0x0F));
            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0);
            Registers.SetFlag(FlagType.N);

            int undoc = Registers.RegisterSet[A] - hlMem - (Registers.RegisterSet[F] & (byte)FlagType.H);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x02) > 0);  // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0);  // (Y) (Undocumented flag)

            if (Registers.BC != 0 && diff != 0)
            {
                Registers.PC -= 2;
            }

            //LogInstructionExec("0xB9: CPIR");
        }
        #endregion


        #region ADD instructions ((A, R), (A, N), (A, (RR)), (A, (IR + d)), (HL, RR), (IR, RR))
        private void ADD_A_R([ConstantExpected] byte operatingRegister)
        {
            ADDAny(Registers.RegisterSet[operatingRegister]);
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADD {Registers.RegisterName(operatingRegister)}");
        }
        private void ADD_A_N()
        {
            ADDAny(Fetch());
            //LogInstructionExec($"0xC6: ADD N:0x{FetchLast():X2}");
        }
        private void ADD_A_RRMEM([ConstantExpected] byte operatingRegister)
        {
            ADDAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADD ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void ADD_A_IRDMEM([ConstantExpected] byte indexAddressingMode)
        {
            ADDAny(_memory.Read((ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADD ({Registers.RegisterName(indexAddressingMode, true)} + d)");
        }
        private void ADD_HL_RR([ConstantExpected] byte operatingRegister)
        {
            Registers.HL = ADDWord(Registers.HL, Registers.GetR16FromHighIndexer(operatingRegister));
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADD HL, {Registers.RegisterName(operatingRegister, true)}");
        }
        private void ADD_IR_RR([ConstantExpected] byte mode, [ConstantExpected] byte operatingRegister)
        {
            ushort value = ADDWord(Registers.GetR16FromHighIndexer(mode), Registers.GetR16FromHighIndexer(operatingRegister));
            Registers.RegisterSet[mode] = value.GetUpperByte();
            Registers.RegisterSet[mode + 1] = value.GetLowerByte();
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADD {Registers.RegisterName(mode, true)}, {Registers.RegisterName(operatingRegister, true)}");
        }
        #endregion

        #region ADC instructions ((A, R), (A, N), (A, (RR)), (A, (IR + d)), (HL, RR))
        private void ADC_A_R(byte operatingRegister)
        {
            ADCAny(Registers.RegisterSet[operatingRegister]);
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADC {Registers.RegisterName(operatingRegister)}");
        }
        private void ADC_A_N()
        {
            ADCAny(Fetch());
            //LogInstructionExec($"0xCE: ADC N:0x{FetchLast():X2}");
        }
        private void ADC_A_RRMEM([ConstantExpected] byte operatingRegister)
        {
            ADCAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADC ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void ADC_A_IRDMEM([ConstantExpected] byte indexAddressingMode)
        {
            ADCAny(_memory.Read((ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADC ({Registers.RegisterName(indexAddressingMode, true)} + d)");
        }
        private void ADC_HL_RR([ConstantExpected] byte operatingRegister)
        {
            ADCHL(Registers.GetR16FromHighIndexer(operatingRegister));
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADC HL, {Registers.RegisterName(operatingRegister, true)}:0x{Registers.GetR16FromHighIndexer(operatingRegister):X4}");
        }
        #endregion


        #region SUB instructions (R, N, (RR), (IR + d))
        private void SUB_R([ConstantExpected] byte operatingRegister)
        {
            SUBAny(Registers.RegisterSet[operatingRegister]);
            //LogInstructionExec($"0x{_currentInstruction:X2}: SUB {Registers.RegisterName(operatingRegister)}");
        }
        private void SUB_N()
        {
            SUBAny(Fetch());
            //LogInstructionExec($"0xD6: SUB N:0x{FetchLast():X2}");
        }
        private void SUB_RRMEM([ConstantExpected] byte operatingRegister)
        {
            SUBAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            //LogInstructionExec($"0x{_currentInstruction:X2}: SUB ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void SUB_IRDMEM([ConstantExpected] byte indexAddressingMode)
        {
            SUBAny(_memory.Read((ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: SUB ({Registers.RegisterName(indexAddressingMode, true)})");
        }
        #endregion

        #region SBC instructions (A, R), (A, N), (A, (RR)), (A, (IR + d)), (HL, RR)
        private void SBC_A_R([ConstantExpected] byte operatingRegister)
        {
            SBCAny(Registers.RegisterSet[operatingRegister]);
            //LogInstructionExec($"0x{_currentInstruction:X2}: SBC {Registers.RegisterName(operatingRegister)}");
        }
        private void SBC_A_N()
        {
            SBCAny(Fetch());
            //LogInstructionExec($"0xDE: SBC N:0x{FetchLast():X2}");
        }
        private void SBC_A_RRMEM([ConstantExpected] byte operatingRegister)
        {
            SBCAny(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)));
            //LogInstructionExec($"0x{_currentInstruction:X2}: SBC ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void SBC_A_IRDMEM([ConstantExpected] byte indexAddressingMode)
        {
            SBCAny(_memory.Read((ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: SUB ({Registers.RegisterName(indexAddressingMode, true)})");
        }

        private void SBC_HL_RR([ConstantExpected] byte operatingRegister)
        {
            SBCHL(Registers.GetR16FromHighIndexer(operatingRegister));
            //LogInstructionExec($"0x{_currentInstruction:X2}: SBC HL, {Registers.RegisterName(operatingRegister, true)}:0x{Registers.GetR16FromHighIndexer(operatingRegister):X4}");
        }
        #endregion
    }
}