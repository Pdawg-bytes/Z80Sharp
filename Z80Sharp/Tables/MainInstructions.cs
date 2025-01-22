using static Z80Sharp.Constants.ConditionCodes;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        private void ExecuteMainInstruction()
        {
            // Most instructions in the main table only take 4 cycles,
            // so we can just add them here. Any instructions that take
            // longer will add extra cycles if needed.
            _clock.Add(4);
            switch (_currentInstruction)
            {
                case 0x00: NOP(); break;  // NOP
                case 0x76: HALT(); break; // HALT

                case 0x2F: CPL(); break; // CPL

                case 0x27: DAA(); break; // DAA


                case 0xD3: OUT_NPORT_A(); _clock.Add(7); break; // OUT (N), A
                case 0xDB: IN_A_NPORT();  _clock.Add(7); break; // IN A, (N)


                case 0x07: RLCA(); break; // RLCA
                case 0x17: RLA(); break;  // RLA
                case 0x0F: RRCA(); break; // RRCA
                case 0x1F: RRA(); break;  // RRA


                case 0x09: ADD_HL_RR(ref Registers.BC); _clock.Add(7); break; // ADD HL, BC
                case 0x19: ADD_HL_RR(ref Registers.DE); _clock.Add(7); break; // ADD HL, DE
                case 0x29: ADD_HL_RR(ref Registers.HL); _clock.Add(7); break; // ADD HL, HL
                case 0x39: ADD_HL_RR(ref Registers.SP); _clock.Add(7); break; // ADD HL, SP

                case 0x80: ADD_A_R(ref Registers.B); break;                     // ADD A, B
                case 0x81: ADD_A_R(ref Registers.C); break;                     // ADD A, C
                case 0x82: ADD_A_R(ref Registers.D); break;                     // ADD A, D
                case 0x83: ADD_A_R(ref Registers.E); break;                     // ADD A, E
                case 0x84: ADD_A_R(ref Registers.H); break;                     // ADD A, H
                case 0x85: ADD_A_R(ref Registers.L); break;                     // ADD A, L
                case 0x86: ADD_A_RRMEM(ref Registers.HL); _clock.Add(3); break; // ADD A, (HL)
                case 0x87: ADD_A_R(ref Registers.A); break;                     // ADD A, A
                case 0xC6: ADD_A_N(); _clock.Add(3); break;                     // ADD A, N

                case 0x88: ADC_A_R(ref Registers.B); break;                     // ADC A, B
                case 0x89: ADC_A_R(ref Registers.C); break;                     // ADC A, C
                case 0x8A: ADC_A_R(ref Registers.D); break;                     // ADC A, D
                case 0x8B: ADC_A_R(ref Registers.E); break;                     // ADC A, E
                case 0x8C: ADC_A_R(ref Registers.H); break;                     // ADC A, H
                case 0x8D: ADC_A_R(ref Registers.L); break;                     // ADC A, L
                case 0x8E: ADC_A_RRMEM(ref Registers.HL); _clock.Add(3); break; // ADC A, (HL)
                case 0x8F: ADC_A_R(ref Registers.A); break;                     // ADC A, A
                case 0xCE: ADC_A_N(); _clock.Add(3); break;                     // ADC A, N


                case 0x90: SUB_A_R(ref Registers.B); break;                     // SUB B
                case 0x91: SUB_A_R(ref Registers.C); break;                     // SUB C
                case 0x92: SUB_A_R(ref Registers.D); break;                     // SUB D
                case 0x93: SUB_A_R(ref Registers.E); break;                     // SUB E
                case 0x94: SUB_A_R(ref Registers.H); break;                     // SUB H
                case 0x95: SUB_A_R(ref Registers.L); break;                     // SUB L
                case 0x96: SUB_A_RRMEM(ref Registers.HL); _clock.Add(3); break; // SUB (HL)
                case 0x97: SUB_A_R(ref Registers.A); break;                     // SUB A
                case 0xD6: SUB_A_N(); _clock.Add(3); break;                     // SUB N

                case 0x98: SBC_A_R(ref Registers.B); break;                     // SBC A, B
                case 0x99: SBC_A_R(ref Registers.C); break;                     // SBC A, C
                case 0x9A: SBC_A_R(ref Registers.D); break;                     // SBC A, D
                case 0x9B: SBC_A_R(ref Registers.E); break;                     // SBC A, E
                case 0x9C: SBC_A_R(ref Registers.H); break;                     // SBC A, H
                case 0x9D: SBC_A_R(ref Registers.L); break;                     // SBC A, L
                case 0x9E: SBC_A_RRMEM(ref Registers.HL); _clock.Add(3); break; // SBC A, (HL)
                case 0x9F: SBC_A_R(ref Registers.A); break;                     // SBC A, A
                case 0xDE: SBC_A_N(); _clock.Add(3); break;                     // SBC A, N


                case 0xB0: OR_R(ref Registers.B); break;                     // OR B
                case 0xB1: OR_R(ref Registers.C); break;                     // OR C
                case 0xB2: OR_R(ref Registers.D); break;                     // OR D
                case 0xB3: OR_R(ref Registers.E); break;                     // OR E
                case 0xB4: OR_R(ref Registers.H); break;                     // OR H
                case 0xB5: OR_R(ref Registers.L); break;                     // OR L
                case 0xB6: OR_RRMEM(ref Registers.HL); _clock.Add(3); break; // OR (HL)
                case 0xB7: OR_R(ref Registers.A); break;                     // OR A
                case 0xF6: OR_N(); _clock.Add(3); break;                     // OR N


                case 0xA8: XOR_R(ref Registers.B); break;                     // XOR B
                case 0xA9: XOR_R(ref Registers.C); break;                     // XOR C
                case 0xAA: XOR_R(ref Registers.D); break;                     // XOR D
                case 0xAB: XOR_R(ref Registers.E); break;                     // XOR E
                case 0xAC: XOR_R(ref Registers.H); break;                     // XOR H
                case 0xAD: XOR_R(ref Registers.L); break;                     // XOR L
                case 0xAE: XOR_RRMEM(ref Registers.HL); _clock.Add(3); break; // XOR (HL)
                case 0xAF: XOR_R(ref Registers.A); break;                     // XOR A
                case 0xEE: XOR_N(); _clock.Add(3); break;                     // XOR N


                case 0xA0: AND_R(ref Registers.B); break;                     // AND B
                case 0xA1: AND_R(ref Registers.C); break;                     // AND C
                case 0xA2: AND_R(ref Registers.D); break;                     // AND D
                case 0xA3: AND_R(ref Registers.E); break;                     // AND E
                case 0xA4: AND_R(ref Registers.H); break;                     // AND H
                case 0xA5: AND_R(ref Registers.L); break;                     // AND L
                case 0xA6: AND_RRMEM(ref Registers.HL); _clock.Add(3); break; // AND (HL)
                case 0xA7: AND_R(ref Registers.A); break;                     // AND A
                case 0xE6: AND_N(); _clock.Add(3); break;                     // AND N


                case 0xB8: CMP_R(ref Registers.B); break;                     // CP B
                case 0xB9: CMP_R(ref Registers.C); break;                     // CP C
                case 0xBA: CMP_R(ref Registers.D); break;                     // CP D
                case 0xBB: CMP_R(ref Registers.E); break;                     // CP E
                case 0xBC: CMP_R(ref Registers.H); break;                     // CP H
                case 0xBD: CMP_R(ref Registers.L); break;                     // CP L
                case 0xBE: CMP_RRMEM(ref Registers.HL); _clock.Add(3); break; // CP (HL)
                case 0xBF: CMP_R(ref Registers.A); break;                     // CP A
                case 0xFE: CMP_N(); _clock.Add(3); break;                     // CP N


                case 0x3F: CCF(); break; // CCF
                case 0x37: SCF(); break; // SCF


                case 0x01: LD_RR_NN(ref Registers.BC); _clock.Add(6); break;   // LD BC, NN
                case 0x11: LD_RR_NN(ref Registers.DE); _clock.Add(6); break;   // LD DE, NN
                case 0x21: LD_RR_NN(ref Registers.HL); _clock.Add(6); break;   // LD HL, NN
                case 0x31: LD_RR_NN(ref Registers.SP); _clock.Add(6); break;   // LD SP, NN

                case 0xF9: LD_RR_RR(ref Registers.SP, ref Registers.HL); _clock.Add(2); break; // LD SP, HL

                case 0x06: LD_R_N(ref Registers.B); _clock.Add(3); break; // LD B, N
                case 0x0E: LD_R_N(ref Registers.C); _clock.Add(3); break; // LD C, N
                case 0x16: LD_R_N(ref Registers.D); _clock.Add(3); break; // LD D, N
                case 0x1E: LD_R_N(ref Registers.E); _clock.Add(3); break; // LD E, N
                case 0x26: LD_R_N(ref Registers.H); _clock.Add(3); break; // LD H, N
                case 0x2E: LD_R_N(ref Registers.L); _clock.Add(3); break; // LD L, N
                case 0x3E: LD_R_N(ref Registers.A); _clock.Add(3); break; // LD A, N

                case 0x36: LD_HLMEM_N(); _clock.Add(6); break; // LD (HL), N
                case 0x02: LD_RRMEM_R(ref Registers.BC, ref Registers.A); _clock.Add(3); break;  // LD (BC), A
                case 0x12: LD_RRMEM_R(ref Registers.DE, ref Registers.A); _clock.Add(3); break;  // LD (DE), A
                case 0x0A: LD_R_RRMEM(ref Registers.A, ref Registers.BC); _clock.Add(3); break;  // LD A, (BC)
                case 0x1A: LD_R_RRMEM(ref Registers.A, ref Registers.DE); _clock.Add(3); break;  // LD A, (DE)

                case 0x32: LD_NNMEM_R(ref Registers.A);   _clock.Add(9);  break; // LD (NN), A
                case 0x22: LD_NNMEM_RR(ref Registers.HL); _clock.Add(12); break; // LD (NN), HL
                case 0x2A: LD_RR_NNMEM(ref Registers.HL); _clock.Add(12); break; // LD HL, (NN)
                case 0x3A: LD_R_NNMEM(ref Registers.A);   _clock.Add(9);  break; // LD A, (NN)

                case 0x40: LD_R_R(ref Registers.B, ref Registers.B); break;                     // LD B, B
                case 0x41: LD_R_R(ref Registers.B, ref Registers.C); break;                     // LD B, C
                case 0x42: LD_R_R(ref Registers.B, ref Registers.D); break;                     // LD B, D
                case 0x43: LD_R_R(ref Registers.B, ref Registers.E); break;                     // LD B, E
                case 0x44: LD_R_R(ref Registers.B, ref Registers.H); break;                     // LD B, H
                case 0x45: LD_R_R(ref Registers.B, ref Registers.L); break;                     // LD B, L
                case 0x46: LD_R_RRMEM(ref Registers.B, ref Registers.HL); _clock.Add(3); break; // LD B, (HL)
                case 0x47: LD_R_R(ref Registers.B, ref Registers.A); break;                     // LD B, A

                case 0x48: LD_R_R(ref Registers.C, ref Registers.B); break;                     // LD C, B
                case 0x49: LD_R_R(ref Registers.C, ref Registers.C); break;                     // LD C, C
                case 0x4A: LD_R_R(ref Registers.C, ref Registers.D); break;                     // LD C, D
                case 0x4B: LD_R_R(ref Registers.C, ref Registers.E); break;                     // LD C, E
                case 0x4C: LD_R_R(ref Registers.C, ref Registers.H); break;                     // LD C, H
                case 0x4D: LD_R_R(ref Registers.C, ref Registers.L); break;                     // LD C, L
                case 0x4E: LD_R_RRMEM(ref Registers.C, ref Registers.HL); _clock.Add(3); break; // LD C, (HL)
                case 0x4F: LD_R_R(ref Registers.C, ref Registers.A); break;                     // LD C, A

                case 0x50: LD_R_R(ref Registers.D, ref Registers.B); break;                     // LD D, B
                case 0x51: LD_R_R(ref Registers.D, ref Registers.C); break;                     // LD D, C
                case 0x52: LD_R_R(ref Registers.D, ref Registers.D); break;                     // LD D, D
                case 0x53: LD_R_R(ref Registers.D, ref Registers.E); break;                     // LD D, E
                case 0x54: LD_R_R(ref Registers.D, ref Registers.H); break;                     // LD D, H
                case 0x55: LD_R_R(ref Registers.D, ref Registers.L); break;                     // LD D, L
                case 0x56: LD_R_RRMEM(ref Registers.D, ref Registers.HL); _clock.Add(3); break; // LD D, (HL)
                case 0x57: LD_R_R(ref Registers.D, ref Registers.A); break;                     // LD D, A

                case 0x58: LD_R_R(ref Registers.E, ref Registers.B); break;                     // LD E, B
                case 0x59: LD_R_R(ref Registers.E, ref Registers.C); break;                     // LD E, C
                case 0x5A: LD_R_R(ref Registers.E, ref Registers.D); break;                     // LD E, D
                case 0x5B: LD_R_R(ref Registers.E, ref Registers.E); break;                     // LD E, E
                case 0x5C: LD_R_R(ref Registers.E, ref Registers.H); break;                     // LD E, H
                case 0x5D: LD_R_R(ref Registers.E, ref Registers.L); break;                     // LD E, L
                case 0x5E: LD_R_RRMEM(ref Registers.E, ref Registers.HL); _clock.Add(3); break; // LD E, (HL)
                case 0x5F: LD_R_R(ref Registers.E, ref Registers.A); break;                     // LD E, A

                case 0x60: LD_R_R(ref Registers.H, ref Registers.B); break;                     // LD H, B
                case 0x61: LD_R_R(ref Registers.H, ref Registers.C); break;                     // LD H, C
                case 0x62: LD_R_R(ref Registers.H, ref Registers.D); break;                     // LD H, D
                case 0x63: LD_R_R(ref Registers.H, ref Registers.E); break;                     // LD H, E
                case 0x64: LD_R_R(ref Registers.H, ref Registers.H); break;                     // LD H, H
                case 0x65: LD_R_R(ref Registers.H, ref Registers.L); break;                     // LD H, L
                case 0x66: LD_R_RRMEM(ref Registers.H, ref Registers.HL); _clock.Add(3); break; // LD H, (HL)
                case 0x67: LD_R_R(ref Registers.H, ref Registers.A); break;                     // LD H, A

                case 0x68: LD_R_R(ref Registers.L, ref Registers.B); break;                     // LD L, B
                case 0x69: LD_R_R(ref Registers.L, ref Registers.C); break;                     // LD L, C
                case 0x6A: LD_R_R(ref Registers.L, ref Registers.D); break;                     // LD L, D
                case 0x6B: LD_R_R(ref Registers.L, ref Registers.E); break;                     // LD L, E
                case 0x6C: LD_R_R(ref Registers.L, ref Registers.H); break;                     // LD L, H
                case 0x6D: LD_R_R(ref Registers.L, ref Registers.L); break;                     // LD L, L
                case 0x6E: LD_R_RRMEM(ref Registers.L, ref Registers.HL); _clock.Add(3); break; // LD L, (HL)
                case 0x6F: LD_R_R(ref Registers.L, ref Registers.A); break;                     // LD L, A

                case 0x78: LD_R_R(ref Registers.A, ref Registers.B); break;                     // LD A, B
                case 0x79: LD_R_R(ref Registers.A, ref Registers.C); break;                     // LD A, C
                case 0x7A: LD_R_R(ref Registers.A, ref Registers.D); break;                     // LD A, D
                case 0x7B: LD_R_R(ref Registers.A, ref Registers.E); break;                     // LD A, E
                case 0x7C: LD_R_R(ref Registers.A, ref Registers.H); break;                     // LD A, H
                case 0x7D: LD_R_R(ref Registers.A, ref Registers.L); break;                     // LD A, L
                case 0x7E: LD_R_RRMEM(ref Registers.A, ref Registers.HL); _clock.Add(3); break; // LD A, (HL)
                case 0x7F: LD_R_R(ref Registers.A, ref Registers.A); break;                     // LD A, A

                case 0x70: LD_RRMEM_R(ref Registers.HL, ref Registers.B); _clock.Add(3); break;  // LD (HL), B
                case 0x71: LD_RRMEM_R(ref Registers.HL, ref Registers.C); _clock.Add(3); break;  // LD (HL), C
                case 0x72: LD_RRMEM_R(ref Registers.HL, ref Registers.D); _clock.Add(3); break;  // LD (HL), D
                case 0x73: LD_RRMEM_R(ref Registers.HL, ref Registers.E); _clock.Add(3); break;  // LD (HL), E
                case 0x74: LD_RRMEM_R(ref Registers.HL, ref Registers.H); _clock.Add(3); break;  // LD (HL), H
                case 0x75: LD_RRMEM_R(ref Registers.HL, ref Registers.L); _clock.Add(3); break;  // LD (HL), L
                case 0x77: LD_RRMEM_R(ref Registers.HL, ref Registers.A); _clock.Add(3); break;  // LD (HL), A


                case 0x10: DJNZ_D(); break; // DJNZ D
                case 0xC3: JP_NN(); _clock.Add(6); break;  // JP NN
                case 0xE9: JP_RR(ref Registers.HL); break; // JP (HL)
                case 0xC2: JP_NN_C(NZ_C); _clock.Add(6); break; // JP NZ, NN
                case 0xCA: JP_NN_C(Z_C);  _clock.Add(6); break; // JP Z, NN
                case 0xD2: JP_NN_C(NC_C); _clock.Add(6); break; // JP NC, NN
                case 0xDA: JP_NN_C(C_C);  _clock.Add(6); break; // JP C, NN
                case 0xE2: JP_NN_C(PO_C); _clock.Add(6); break; // JP PO, NN
                case 0xEA: JP_NN_C(PE_C); _clock.Add(6); break; // JP PE, NN
                case 0xF2: JP_NN_C(P_C);  _clock.Add(6); break; // JP P, NN
                case 0xFA: JP_NN_C(M_C);  _clock.Add(6); break; // JP M, NN

                case 0x18: JR_D(); _clock.Add(8); break;           // JR D
                case 0x20: JR_CC_D(NZ_C); _clock.Add(8, 3); break; // JR NZ, D
                case 0x28: JR_CC_D(Z_C); _clock.Add(8, 3);  break; // JR Z, D
                case 0x30: JR_CC_D(NC_C); _clock.Add(8, 3); break; // JR NC, D
                case 0x38: JR_CC_D(C_C); _clock.Add(8, 3);  break; // JR C, D


                case 0xC9: RET(); _clock.Add(6); break;           // RET
                case 0xC0: RET_CC(NZ_C); _clock.Add(7, 1); break; // RET NZ
                case 0xC8: RET_CC(Z_C); _clock.Add(7, 1);  break; // RET Z
                case 0xD0: RET_CC(NC_C); _clock.Add(7, 1); break; // RET NC
                case 0xD8: RET_CC(C_C); _clock.Add(7, 1);  break; // RET C
                case 0xE0: RET_CC(PO_C); _clock.Add(7, 1); break; // RET PO
                case 0xE8: RET_CC(PE_C); _clock.Add(7, 1); break; // RET PE
                case 0xF0: RET_CC(P_C); _clock.Add(7, 1);  break; // RET P
                case 0xF8: RET_CC(M_C); _clock.Add(7, 1);  break; // RET M


                case 0xC7: RST_HH(0x00); _clock.Add(7); break; // RST 00h
                case 0xD7: RST_HH(0x10); _clock.Add(7); break; // RST 10h
                case 0xE7: RST_HH(0x20); _clock.Add(7); break; // RST 20h
                case 0xF7: RST_HH(0x30); _clock.Add(7); break; // RST 30h
                case 0xCF: RST_HH(0x08); _clock.Add(7); break; // RST 08h
                case 0xDF: RST_HH(0x18); _clock.Add(7); break; // RST 18h
                case 0xEF: RST_HH(0x28); _clock.Add(7); break; // RST 28h
                case 0xFF: RST_HH(0x38); _clock.Add(7); break; // RST 38h


                case 0xCD: CALL_NN(); _clock.Add(13); break;           // CALL NN
                case 0xC4: CALL_CC_NN(NZ_C); _clock.Add(13, 6); break; // CALL NZ, NN
                case 0xCC: CALL_CC_NN(Z_C); _clock.Add(13, 6); break;  // CALL Z, NN
                case 0xD4: CALL_CC_NN(NC_C); _clock.Add(13, 6); break; // CALL NC, NN
                case 0xDC: CALL_CC_NN(C_C); _clock.Add(13, 6); break;  // CALL C, NN
                case 0xE4: CALL_CC_NN(PO_C); _clock.Add(13, 6); break; // CALL PO, NN
                case 0xEC: CALL_CC_NN(PE_C); _clock.Add(13, 6); break; // CALL PE, NN
                case 0xF4: CALL_CC_NN(P_C); _clock.Add(13, 6); break;  // CALL P, NN
                case 0xFC: CALL_CC_NN(M_C); _clock.Add(13, 6); break;  // CALL M, NN


                case 0xC1: POP_RR(ref Registers.BC); _clock.Add(6); break; // POP BC
                case 0xD1: POP_RR(ref Registers.DE); _clock.Add(6); break; // POP DE
                case 0xE1: POP_RR(ref Registers.HL); _clock.Add(6); break; // POP HL
                case 0xF1: POP_AF(); _clock.Add(6); break;                 // POP AF

                case 0xC5: PUSH_RR(ref Registers.BC); _clock.Add(7); break; // PUSH BC
                case 0xD5: PUSH_RR(ref Registers.DE); _clock.Add(7); break; // PUSH DE
                case 0xE5: PUSH_RR(ref Registers.HL); _clock.Add(7); break; // PUSH HL
                case 0xF5: PUSH_AF(); _clock.Add(7); break;                 // PUSH AF


                case 0x03: INC_RR(ref Registers.BC); _clock.Add(2); break; // INC BC
                case 0x13: INC_RR(ref Registers.DE); _clock.Add(2); break; // INC DE
                case 0x23: INC_RR(ref Registers.HL); _clock.Add(2); break; // INC HL
                case 0x33: INC_RR(ref Registers.SP); _clock.Add(2); break; // INC SP
                case 0x34: INC_HLMEM(); _clock.Add(7); break;              // INC (HL)
                case 0x04: INC_R(ref Registers.B); break;   // INC B
                case 0x0C: INC_R(ref Registers.C); break;   // INC C
                case 0x14: INC_R(ref Registers.D); break;   // INC D
                case 0x1C: INC_R(ref Registers.E); break;   // INC E
                case 0x24: INC_R(ref Registers.H); break;   // INC H
                case 0x2C: INC_R(ref Registers.L); break;   // INC L
                case 0x3C: INC_R(ref Registers.A); break;   // INC A


                case 0x0B: DEC_RR(ref Registers.BC); _clock.Add(2); break; // DEC BC
                case 0x1B: DEC_RR(ref Registers.DE); _clock.Add(2); break; // DEC DE
                case 0x2B: DEC_RR(ref Registers.HL); _clock.Add(2); break; // DEC HL
                case 0x3B: DEC_RR(ref Registers.SP); _clock.Add(2); break; // DEC SP
                case 0x35: DEC_HLMEM(); _clock.Add(7); break;              // DEC (HL)
                case 0x05: DEC_R(ref Registers.B); break;   // DEC B
                case 0x0D: DEC_R(ref Registers.C); break;   // DEC C
                case 0x15: DEC_R(ref Registers.D); break;   // DEC D
                case 0x1D: DEC_R(ref Registers.E); break;   // DEC E
                case 0x25: DEC_R(ref Registers.H); break;   // DEC H
                case 0x2D: DEC_R(ref Registers.L); break;   // DEC L
                case 0x3D: DEC_R(ref Registers.A); break;   // DEC A

                case 0x08: EX_AF_AF_(); break;                   // EX AF, AF'
                case 0xE3: EX_SPMEM_HL(); _clock.Add(15); break; // EX (SP), HL
                case 0xEB: EX_DE_HL(); break;                    // EX DE, HL
                case 0xD9: EXX(); break;                         // EXX

                case 0xF3: DI(); break; // Disable Interrupts
                case 0xFB: EI(); break; // Enable Interrupts

                default:
                    _logger.Log(Enums.LogSeverity.Fatal, $"Unrecognized MAIN opcode: 0x{_currentInstruction:X2}");
                    Halted = true;
                    break;
            }
        }
    }
}