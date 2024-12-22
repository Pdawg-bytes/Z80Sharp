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
        // Reference: https://stackoverflow.com/questions/8119577/z80-daa-instruction
        // I copied this code...I wish there was a better way to do this while keeping it 100% accurate.
        private void DAA()
        {
            int t = 0;

            if (Registers.IsFlagSet(FlagType.H) || ((Registers.A & 0xF) > 9)) t++;
            if (Registers.IsFlagSet(FlagType.C) || (Registers.A > 0x99)) { t += 2; Registers.SetFlag(FlagType.C); }

            if (Registers.IsFlagSet(FlagType.N) && !Registers.IsFlagSet(FlagType.H)) Registers.ClearFlag(FlagType.H);
            else
            {
                if (Registers.IsFlagSet(FlagType.N) && Registers.IsFlagSet(FlagType.H))
                    Registers.SetFlagConditionally(FlagType.H, (Registers.A & 0x0F) < 6);
                else
                    Registers.SetFlagConditionally(FlagType.H, (Registers.A & 0x0F) >= 0x0A);
            }

            switch (t)
            {
                case 1:
                    Registers.A += Registers.IsFlagSet(FlagType.N) ? (byte)0xFA : (byte)0x06; // -6:6
                    break;
                case 2:
                    Registers.A += Registers.IsFlagSet(FlagType.N) ? (byte)0xA0 : (byte)0x60; // -0x60:0x60
                    break;
                case 3:
                    Registers.A += Registers.IsFlagSet(FlagType.N) ? (byte)0x9A : (byte)0x66; // -0x66:0x66
                    break;
            }

            Registers.SetFlagConditionally(FlagType.S, (Registers.A & 0x80) != 0);  // (S)  (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, Registers.A == 0);           // (Z)  (Set if result is zero)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(Registers.A));  // (PV) (Set if bit parity is even)
            Registers.SetFlagConditionally(FlagType.X, (Registers.A & 1 << 5) > 0); // (X)  (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (Registers.A & 1 << 3) > 0); // (Y)  (Undocumented flag)
        }

        private void NEG()
        {
            byte value = Registers.A;
            int result = 0 - value;
            Registers.A = (byte)result;

            Registers.SetFlagConditionally(FlagType.S, (result & 0x80) != 0); // (S)  (Set if result is negative)
            Registers.SetFlagConditionally(FlagType.Z, Registers.A == 0);     // (Z)  (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.H, (value & 0x0F) != 0);  // (H)  (Set if borrow from bit 4)
            Registers.SetFlagConditionally(FlagType.PV, value == 0x80);       // (PV) (Set if A == -128 (sbyte.Min)
            Registers.SetFlagConditionally(FlagType.C, value != 0);           // (C)  (Set if borrow occured)
            Registers.SetFlag(FlagType.N);                                    // (N)  (Unconditionally set)

            Registers.SetFlagConditionally(FlagType.X, (result & 0x20) > 0);  // (X)  (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x08) > 0);  // (Y)  (Undocumented flag)

            //LogInstructionExec("0x44: NEG");
        }

        #region INC instructions (RR, R, (HL), (IR + d))
        private void INC_RR(ref ushort operatingRegister)
        {
            ushort value = (ushort)(operatingRegister + 1);
            operatingRegister = value;
            //LogInstructionExec($"0x{_currentInstruction:X2}: INC RR");
        }
        private void INC_R(ref byte operatingRegister)
        {
            operatingRegister = INCAny(operatingRegister);
            //LogInstructionExec($"0x{_currentInstruction:X2}: INC R");
        }
        private void INC_HLMEM()
        {
            _memory.Write(Registers.HL, INCAny(_memory.Read(Registers.HL)));
            //LogInstructionExec($"0x34: INC (HL)");
        }
        private void INC_IRDMEM(ref ushort indexAddressingMode)
        {
            ushort addr = (ushort)(indexAddressingMode + (sbyte)Fetch());
            _memory.Write(addr, INCAny(_memory.Read(addr)));
            //LogInstructionExec($"0x34: INC (IR + d)");
        }
        #endregion

        #region DEC instructions (RR, R, (HL), (IR + d))
        private void DEC_RR(ref ushort operatingRegister)
        {
            operatingRegister = (ushort)(operatingRegister - 1);
            //LogInstructionExec($"0x{_currentInstruction:X2}: DEC RR");
        }
        private void DEC_R(ref byte operatingRegister)
        {
            operatingRegister = DECAny(operatingRegister);
            //LogInstructionExec($"0x{_currentInstruction:X2}: DEC R");
        }
        private void DEC_HLMEM()
        {
            _memory.Write(Registers.HL, DECAny(_memory.Read(Registers.HL)));
            //LogInstructionExec($"0x35: DEC (HL)");
        }
        private void DEC_IRDMEM(ref ushort indexAddressingMode)
        {
            ushort addr = (ushort)(indexAddressingMode + (sbyte)Fetch());
            _memory.Write(addr, DECAny(_memory.Read(addr)));
            //LogInstructionExec($"0x35: DEC (IR + d)");
        }
        #endregion


        #region OR instructions (R, N, (RR), (IR + d))
        private void OR_R(ref byte operatingRegister)
        {
            ORAny(operatingRegister);
            //LogInstructionExec($"0x{_currentInstruction:X2}: OR R");
        }
        private void OR_N()
        {
            ORAny(Fetch());
            //LogInstructionExec($"0xF6: OR N:0x{FetchLast():X2}");
        }
        private void OR_RRMEM(ref ushort operatingRegister)
        {
            ORAny(_memory.Read(operatingRegister));
            //LogInstructionExec($"0x{_currentInstruction:X2}: OR (RR)");
        }
        private void OR_IRDMEM(ref ushort indexAddressingMode)
        {
            ORAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: OR (IR + d)");
        }
        #endregion

        #region XOR instructions (R, N, (RR), (IR + d))
        private void XOR_R(ref byte operatingRegister)
        {
            XORAny(operatingRegister);
            //LogInstructionExec($"0x{_currentInstruction:X2}: XOR R");
        }
        private void XOR_N()
        {
            XORAny(Fetch());
            //LogInstructionExec($"0xEE: XOR N:0x{FetchLast():X2}");
        }
        private void XOR_RRMEM(ref ushort operatingRegister)
        {
            XORAny(_memory.Read(operatingRegister));
            //LogInstructionExec($"0x{_currentInstruction:X2}: XOR (RR)");
        }
        private void XOR_IRDMEM(ref ushort indexAddressingMode)
        {
            XORAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: XOR (IR + d)");
        }
        #endregion


        #region AND instructions (R, N, (RR), (IR + d))
        private void AND_R(ref byte operatingRegister)
        {
            ANDAny(operatingRegister);
            //LogInstructionExec($"0x{_currentInstruction:X2}: AND R");
        }
        private void AND_N()
        {
            ANDAny(Fetch());
            //LogInstructionExec($"0xE6: XOR N:0x{FetchLast():X2}");
        }
        private void AND_RRMEM(ref ushort operatingRegister)
        {
            ANDAny(_memory.Read(operatingRegister));
            //LogInstructionExec($"0x{_currentInstruction:X2}: AND (RR)");
        }
        private void AND_IRDMEM(ref ushort indexAddressingMode)
        {
            ANDAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: AND (IR + d)");
        }
        #endregion


        #region CP instructions (R, N, (RR), (IR + d))
        private void CMP_R(ref byte operatingRegister)
        {
            CMPAny(operatingRegister);
            //LogInstructionExec($"0x{_currentInstruction:X2}: CP R");
        }
        private void CMP_N()
        {
            CMPAny(Fetch());
            //LogInstructionExec($"0xFE: CP N:0x{FetchLast():X2}");
        }
        private void CMP_RRMEM(ref ushort operatingRegister)
        {
            CMPAny(_memory.Read(operatingRegister));
            //LogInstructionExec($"0x{_currentInstruction:X2}: CP (RR)");
        }
        private void CMP_IRDMEM(ref ushort indexAddressingMode)
        {
            CMPAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: CP (IR + d)");
        }

        private void CPI()
        {
            byte hlMem = _memory.Read(Registers.HL);
            sbyte diff = (sbyte)(Registers.A - hlMem);

            Registers.HL++;
            Registers.BC--;

            Registers.SetFlagConditionally(FlagType.S, (byte)diff > 0x7f);
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);
            Registers.SetFlagConditionally(FlagType.H, (Registers.A & 0x0F) < (hlMem & 0x0F));
            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0);
            Registers.SetFlag(FlagType.N);

            int undoc = Registers.A - hlMem - (Registers.F & (byte)FlagType.H);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x02) > 0);  // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0);  // (Y) (Undocumented flag)

            //LogInstructionExec("0xA1: CPI");
        }
        private void CPIR()
        {
            byte hlMem = _memory.Read(Registers.HL);
            sbyte diff = (sbyte)(Registers.A - hlMem);

            Registers.HL++;
            Registers.BC--;

            Registers.SetFlagConditionally(FlagType.S, (byte)diff > 0x7f);
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);
            Registers.SetFlagConditionally(FlagType.H, (Registers.A & 0x0F) < (hlMem & 0x0F));
            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0);
            Registers.SetFlag(FlagType.N);

            int undoc = Registers.A - hlMem - (Registers.F & (byte)FlagType.H);
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
            sbyte diff = (sbyte)(Registers.A - hlMem);

            Registers.HL--;
            Registers.BC--;

            Registers.SetFlagConditionally(FlagType.S, (byte)diff > 0x7f);
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);
            Registers.SetFlagConditionally(FlagType.H, (Registers.A & 0x0F) < (hlMem & 0x0F));
            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0);
            Registers.SetFlag(FlagType.N);

            int undoc = Registers.A - hlMem - (Registers.F & (byte)FlagType.H);
            Registers.SetFlagConditionally(FlagType.X, (undoc & 0x02) > 0);  // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (undoc & 0x08) > 0);  // (Y) (Undocumented flag)

            //LogInstructionExec("0xA9: CPI");
        }
        private void CPDR()
        {
            byte hlMem = _memory.Read(Registers.HL);
            sbyte diff = (sbyte)(Registers.A - hlMem);

            Registers.HL--;
            Registers.BC--;

            Registers.SetFlagConditionally(FlagType.S, (byte)diff > 0x7f);
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);
            Registers.SetFlagConditionally(FlagType.H, (Registers.A & 0x0F) < (hlMem & 0x0F));
            Registers.SetFlagConditionally(FlagType.PV, Registers.BC != 0);
            Registers.SetFlag(FlagType.N);

            int undoc = Registers.A - hlMem - (Registers.F & (byte)FlagType.H);
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
        private void ADD_A_R(ref byte operatingRegister)
        {
            ADDAny(operatingRegister);
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADD R");
        }
        private void ADD_A_N()
        {
            ADDAny(Fetch());
            //LogInstructionExec($"0xC6: ADD N:0x{FetchLast():X2}");
        }
        private void ADD_A_RRMEM(ref ushort operatingRegister)
        {
            ADDAny(_memory.Read(operatingRegister));
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADD (RR)");
        }
        private void ADD_A_IRDMEM(ref ushort indexAddressingMode)
        {
            ADDAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADD (IR + d)");
        }
        private void ADD_HL_RR(ref ushort operatingRegister)
        {
            Registers.HL = ADDWord(Registers.HL, operatingRegister);
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADD HL, RR");
        }
        private void ADD_IR_RR(ref ushort mode, ref ushort operatingRegister)
        {
            mode = ADDWord(mode, operatingRegister);
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADD IR, RR");
        }
        #endregion

        #region ADC instructions ((A, R), (A, N), (A, (RR)), (A, (IR + d)), (HL, RR))
        private void ADC_A_R(ref byte operatingRegister)
        {
            ADCAny(operatingRegister);
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADC R");
        }
        private void ADC_A_N()
        {
            ADCAny(Fetch());
            //LogInstructionExec($"0xCE: ADC N:0x{FetchLast():X2}");
        }
        private void ADC_A_RRMEM(ref ushort operatingRegister)
        {
            ADCAny(_memory.Read(operatingRegister));
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADC (RR)");
        }
        private void ADC_A_IRDMEM(ref ushort indexAddressingMode)
        {
            ADCAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADC (IR + d)");
        }
        private void ADC_HL_RR(ref ushort operatingRegister)
        {
            ADCHL(operatingRegister);
            //LogInstructionExec($"0x{_currentInstruction:X2}: ADC HL, RR");
        }
        #endregion


        #region SUB instructions (R, N, (RR), (IR + d))
        private void SUB_R(ref byte operatingRegister)
        {
            SUBAny(operatingRegister);
            //LogInstructionExec($"0x{_currentInstruction:X2}: SUB R");
        }
        private void SUB_N()
        {
            SUBAny(Fetch());
            //LogInstructionExec($"0xD6: SUB N:0x{FetchLast():X2}");
        }
        private void SUB_RRMEM(ref ushort operatingRegister)
        {
            SUBAny(_memory.Read(operatingRegister));
            //LogInstructionExec($"0x{_currentInstruction:X2}: SUB (RR)");
        }
        private void SUB_IRDMEM(ref ushort indexAddressingMode)
        {
            SUBAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: SUB (IR + d)");
        }
        #endregion

        #region SBC instructions (A, R), (A, N), (A, (RR)), (A, (IR + d)), (HL, RR)
        private void SBC_A_R(ref byte operatingRegister)
        {
            SBCAny(operatingRegister);
            //LogInstructionExec($"0x{_currentInstruction:X2}: SBC R");
        }
        private void SBC_A_N()
        {
            SBCAny(Fetch());
            //LogInstructionExec($"0xDE: SBC N:0x{FetchLast():X2}");
        }
        private void SBC_A_RRMEM(ref ushort operatingRegister)
        {
            SBCAny(_memory.Read(operatingRegister));
            //LogInstructionExec($"0x{_currentInstruction:X2}: SBC (RR)");
        }
        private void SBC_A_IRDMEM(ref ushort indexAddressingMode)
        {
            SBCAny(_memory.Read((ushort)(indexAddressingMode + (sbyte)Fetch())));
            //LogInstructionExec($"0x{_currentInstruction:X2}: SUB (IR + d)");
        }

        private void SBC_HL_RR(ref ushort operatingRegister)
        {
            SBCHL(operatingRegister);
            //LogInstructionExec($"0x{_currentInstruction:X2}: SBC HL, RR");
        }
        #endregion
    }
}