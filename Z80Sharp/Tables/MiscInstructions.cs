using System.Runtime.CompilerServices;
using Z80Sharp.Enums;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void ExecuteMiscInstruction()
        {
            _clock.Add(8);
            Registers.IncrementRefresh();

            byte instruction = Fetch();
            _currentInstruction = instruction;
            switch (instruction)
            {
                case 0x00: NOP(); CQ(); break; // NOP | UNDOCUMENTED

                case 0x44: NEG(); SQ(); break; // NEG

                case 0x45: RETX(); CQ(); _clock.Add(6); break; // RETN
                case 0x4D: RETX(); CQ(); _clock.Add(6); break; // RETI

                case 0x67: RRD(); SQ(); _clock.Add(10); break; // RRD
                case 0x6F: RLD(); SQ(); _clock.Add(10); break; // RLD

                case 0xA0: LDI(); SQ(); _clock.Add(8); break;      // LDI
                case 0xB0: LDIR(); SQ(); _clock.Add(13, 8); break; // LDIR
                case 0xA8: LDD(); SQ(); _clock.Add(8); break;      // LDD
                case 0xB8: LDDR(); SQ(); _clock.Add(13, 8); break; // LDDR

                case 0xA1: CPI(); SQ(); _clock.Add(8); break;      // CPI
                case 0xB1: CPIR(); SQ(); _clock.Add(13, 8); break; // CPIR
                case 0xA9: CPD(); SQ(); _clock.Add(8); break;      // CPD
                case 0xB9: CPDR(); SQ(); _clock.Add(13, 8); break; // CPDR

                case 0xA2: INI(); SQ(); _clock.Add(8); break;      // INI
                case 0xB2: INIR(); SQ(); _clock.Add(13, 8); break; // INIR
                case 0xAA: IND(); SQ(); _clock.Add(8); break;      // IND
                case 0xBA: INDR(); SQ(); _clock.Add(13, 8); break; // INDR

                case 0xA3: OUTI(); SQ(); _clock.Add(8); break;     // OUTI
                case 0xB3: OTIR(); SQ(); _clock.Add(13, 8); break; // OTIR
                case 0xAB: OUTD(); SQ(); _clock.Add(8); break;     // OUTD
                case 0xBB: OTDR(); SQ(); _clock.Add(13, 8); break; // OTDR


                case 0x42: SBC_HL_RR(ref Registers.BC); SQ(); _clock.Add(7); break; // SBC HL, BC
                case 0x52: SBC_HL_RR(ref Registers.DE); SQ(); _clock.Add(7); break; // SBC HL, DE
                case 0x62: SBC_HL_RR(ref Registers.HL); SQ(); _clock.Add(7); break; // SBC HL, HL
                case 0x72: SBC_HL_RR(ref Registers.SP); SQ(); _clock.Add(7); break; // SBC HL, SP

                case 0x4A: ADC_HL_RR(ref Registers.BC); SQ(); _clock.Add(7); break; // ADC HL, BC
                case 0x5A: ADC_HL_RR(ref Registers.DE); SQ(); _clock.Add(7); break; // ADC HL, DE
                case 0x6A: ADC_HL_RR(ref Registers.HL); SQ(); _clock.Add(7); break; // ADC HL, HL
                case 0x7A: ADC_HL_RR(ref Registers.SP); SQ(); _clock.Add(7); break; // ADC HL, SP


                case 0x43: LD_NNMEM_RR(ref Registers.BC); CQ(); _clock.Add(12); break; // LD (NN), BC
                case 0x53: LD_NNMEM_RR(ref Registers.DE); CQ(); _clock.Add(12); break; // LD (NN), DE
                case 0x63: LD_NNMEM_RR(ref Registers.HL); CQ(); _clock.Add(12); break; // LD (NN), HL | UNDOCUMENTED
                case 0x73: LD_NNMEM_RR(ref Registers.SP); CQ(); _clock.Add(12); break; // LD (NN), SP

                case 0x4B: LD_RR_NNMEM_F(ref Registers.BC); CQ(); _clock.Add(12); break; // LD BC, (NN)
                case 0x5B: LD_RR_NNMEM_F(ref Registers.DE); CQ(); _clock.Add(12); break; // LD DE, (NN)
                case 0x6B: LD_RR_NNMEM_F(ref Registers.HL); CQ(); _clock.Add(12); break; // LD HL, (NN) | UNDOCUMENTED
                case 0x7B: LD_RR_NNMEM_F(ref Registers.SP); CQ(); _clock.Add(12); break; // LD SP, (NN)

                case 0x47: LD_R_R(ref Registers.I, ref Registers.A); CQ(); _clock.Add(1); break; // LD I, A
                case 0x4F: LD_R_R(ref Registers.R, ref Registers.A); CQ(); _clock.Add(1); break; // LD R, A
                case 0x57: LD_A_R(ref Registers.I); SQ(); _clock.Add(1); break; // LD A, I
                case 0x5F: LD_A_R(ref Registers.R); SQ(); _clock.Add(1); break; // LD A, R


                case 0x46: IM_M(InterruptMode.IM0); CQ(); break; // IM0
                case 0x56: IM_M(InterruptMode.IM1); CQ(); break; // IM1
                case 0x5E: IM_M(InterruptMode.IM2); CQ(); break; // IM2


                case 0x40: IN_R_CPORT(ref Registers.B); SQ(); _clock.Add(4); break; // IN B, (C)
                case 0x48: IN_R_CPORT(ref Registers.C); SQ(); _clock.Add(4); break; // IN C, (C)
                case 0x50: IN_R_CPORT(ref Registers.D); SQ(); _clock.Add(4); break; // IN D, (C)
                case 0x58: IN_R_CPORT(ref Registers.E); SQ(); _clock.Add(4); break; // IN E, (C)
                case 0x60: IN_R_CPORT(ref Registers.H); SQ(); _clock.Add(4); break; // IN H, (C)
                case 0x68: IN_R_CPORT(ref Registers.L); SQ(); _clock.Add(4); break; // IN L, (C)
                case 0x78: IN_R_CPORT(ref Registers.A); SQ(); _clock.Add(4); break; // IN A, (C)

                case 0x70: IN_CPORT(); SQ(); _clock.Add(4); break; // IN (C) | UNDOCUMENTED


                case 0x41: OUT_CPORT_R(ref Registers.B); CQ(); _clock.Add(4); break; // OUT (C), B
                case 0x49: OUT_CPORT_R(ref Registers.C); CQ(); _clock.Add(4); break; // OUT (C), C
                case 0x51: OUT_CPORT_R(ref Registers.D); CQ(); _clock.Add(4); break; // OUT (C), D
                case 0x59: OUT_CPORT_R(ref Registers.E); CQ(); _clock.Add(4); break; // OUT (C), E
                case 0x61: OUT_CPORT_R(ref Registers.H); CQ(); _clock.Add(4); break; // OUT (C), H
                case 0x69: OUT_CPORT_R(ref Registers.L); CQ(); _clock.Add(4); break; // OUT (C), L
                case 0x79: OUT_CPORT_R(ref Registers.A); CQ(); _clock.Add(4); break; // OUT (C), A

                case 0x71: OUT_CPORT_0(); CQ(); _clock.Add(4); break;  // OUT (C), 0 | UNDOCUMENTED


                // These instructions are quite weird, but are expected by FUSE.
                case 0x4E: IM_M(InterruptMode.IM0); CQ(); break; // IM 0 | UNDOCUMENTED
                case 0x4C: NEG(); SQ(); break;                   // NEG  | UNDOCUMENTED

                case 0x54: NEG(); SQ(); break;                   // NEG  | UNDOCUMENTED
                case 0x55: RETX(); CQ(); _clock.Add(6); break;   // RETN | UNDOCUMENTED
                case 0x5C: NEG(); SQ(); break;                   // NEG  | UNDOCUMENTED
                case 0x5D: RETX(); CQ(); _clock.Add(6); break;   // RETN | UNDOCUMENTED

                case 0x64: NEG(); SQ(); break;                   // NEG  | UNDOCUMENTED
                case 0x65: RETX(); CQ(); _clock.Add(6); break;   // RETN | UNDOCUMENTED
                case 0x66: IM_M(InterruptMode.IM0); CQ(); break; // IM 0 | UNDOCUMENTED
                case 0x6C: NEG(); SQ(); break;                   // NEG  | UNDOCUMENTED
                case 0x6D: RETX(); CQ(); _clock.Add(6); break;   // RETN | UNDOCUMENTED

                case 0x74: NEG(); SQ(); break;                   // NEG  | UNDOCUMENTED
                case 0x75: RETX(); CQ(); _clock.Add(6); break;   // RETN | UNDOCUMENTED
                case 0x76: IM_M(InterruptMode.IM1); CQ(); break; // IM 1 | UNDOCUMENTED
                case 0x7C: NEG(); SQ(); break;                   // NEG  | UNDOCUMENTED
                case 0x7D: RETX(); CQ(); _clock.Add(6); break;   // RETN | UNDOCUMENTED
                case 0x7E: IM_M(InterruptMode.IM2); CQ(); break; // IM 2 | UNDOCUMENTED
                case 0x7F: NOP(); CQ(); break;                   // NOP  | UNDOCUMENTED

                default:
                    _logger.Log(Enums.LogSeverity.Fatal, $"Unrecognized MISC opcode: 0x{_currentInstruction:X2}");
                    //Halted = true;
                    break;
            }
        }
    }
}