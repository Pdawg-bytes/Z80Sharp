using Z80Sharp.Enums;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void ExecuteIndexRInstruction(ref ushort indexAddressingMode, ref byte irH, ref byte irL)
        {
            Registers.IncrementRefresh();

            byte instruction = Fetch();
            _currentInstruction = instruction;

            if (instruction == 0xCB) { ExecuteIndexRBitInstruction(ref indexAddressingMode); return; }

            _clock.Add(8);
            switch (instruction)
            {
                // TODO: discard prefix if illegal opcode
                case 0x00: NOP(); CQ(); break; // NOP | UNDOCUMENTED
                case 0xDD: NOP(); CQ(); break; // NOP | UNDOCUMENTED
                case 0xFD: NOP(); CQ(); break; // NOP | UNDOCUMENTED

                case 0x09: ADD_IR_RR(ref indexAddressingMode, ref Registers.BC); SQ(); _clock.Add(7); break;        // ADD IR, BC
                case 0x19: ADD_IR_RR(ref indexAddressingMode, ref Registers.DE); SQ(); _clock.Add(7); break;        // ADD IR, DE
                case 0x29: ADD_IR_RR(ref indexAddressingMode, ref indexAddressingMode); SQ(); _clock.Add(7); break; // ADD IR, IR
                case 0x39: ADD_IR_RR(ref indexAddressingMode, ref Registers.SP); SQ(); _clock.Add(7); break;        // ADD IR, SP

                case 0x80: ADD_A_R(ref Registers.B); SQ(); break;                               // ADD A, B | UNDOCUMENTED
                case 0x81: ADD_A_R(ref Registers.C); SQ(); break;                               // ADD A, C | UNDOCUMENTED
                case 0x82: ADD_A_R(ref Registers.D); SQ(); break;                               // ADD A, D | UNDOCUMENTED
                case 0x83: ADD_A_R(ref Registers.E); SQ(); break;                               // ADD A, E | UNDOCUMENTED
                case 0x84: ADD_A_R(ref irH); SQ(); break;                                       // ADD A, IRh | UNDOCUMENTED
                case 0x85: ADD_A_R(ref irL); SQ(); break;                                       // ADD A, IRl | UNDOCUMENTED
                case 0x86: ADD_A_IRDMEM(ref indexAddressingMode); SQ(); _clock.Add(11); break;  // ADD A, (IR + d)
                case 0x87: ADD_A_R(ref Registers.A); SQ(); break;                               // ADD A, A | UNDOCUMENTED

                case 0x88: ADC_A_R(ref Registers.B); SQ(); break;                               // ADC A, B | UNDOCUMENTED
                case 0x89: ADC_A_R(ref Registers.C); SQ(); break;                               // ADC A, C | UNDOCUMENTED
                case 0x8A: ADC_A_R(ref Registers.D); SQ(); break;                               // ADC A, D | UNDOCUMENTED
                case 0x8B: ADC_A_R(ref Registers.E); SQ(); break;                               // ADC A, E | UNDOCUMENTED
                case 0x8C: ADC_A_R(ref irH); SQ(); break;                                       // ADC A, H | UNDOCUMENTED
                case 0x8D: ADC_A_R(ref irL); SQ(); break;                                       // ADC A, L | UNDOCUMENTED
                case 0x8E: ADC_A_IRDMEM(ref indexAddressingMode); SQ(); _clock.Add(11); break;  // ADC A, (IR + d)
                case 0x8F: ADC_A_R(ref Registers.A); SQ(); break;                               // ADC A, A | UNDOCUMENTED

                case 0x90: SUB_A_R(ref Registers.B); SQ(); break;                               // SUB B | UNDOCUMENTED
                case 0x91: SUB_A_R(ref Registers.C); SQ(); break;                               // SUB C | UNDOCUMENTED
                case 0x92: SUB_A_R(ref Registers.D); SQ(); break;                               // SUB D | UNDOCUMENTED
                case 0x93: SUB_A_R(ref Registers.E); SQ(); break;                               // SUB E | UNDOCUMENTED
                case 0x94: SUB_A_R(ref irH); SQ(); break;                                       // SUB IRh | UNDOCUMENTED
                case 0x95: SUB_A_R(ref irL); SQ(); break;                                       // SUB IRl | UNDOCUMENTED
                case 0x96: SUB_A_IRDMEM(ref indexAddressingMode); SQ(); _clock.Add(11); break;  // SUB (IR + d)
                case 0x97: SUB_A_R(ref Registers.A); SQ(); break;                               // SUB A | UNDOCUMENTED

                case 0x98: SBC_A_R(ref Registers.B); SQ(); break;                               // SBC A, B | UNDOCUMENTED
                case 0x99: SBC_A_R(ref Registers.C); SQ(); break;                               // SBC A, C | UNDOCUMENTED
                case 0x9A: SBC_A_R(ref Registers.D); SQ(); break;                               // SBC A, D | UNDOCUMENTED
                case 0x9B: SBC_A_R(ref Registers.E); SQ(); break;                               // SBC A, E | UNDOCUMENTED
                case 0x9C: SBC_A_R(ref irH); SQ(); break;                                       // SBC A, IRh | UNDOCUMENTED
                case 0x9D: SBC_A_R(ref irL); SQ(); break;                                       // SBC A, IRl | UNDOCUMENTED
                case 0x9E: SBC_A_IRDMEM(ref indexAddressingMode); SQ(); _clock.Add(11); break;  // SBC A, (IR + d)
                case 0x9F: SBC_A_R(ref Registers.A); SQ(); break;                               // SBC A, A | UNDOCUMENTED

                case 0xB0: OR_R(ref Registers.B); SQ(); break;                               // OR B | UNDOCUMENTED
                case 0xB1: OR_R(ref Registers.C); SQ(); break;                               // OR C | UNDOCUMENTED
                case 0xB2: OR_R(ref Registers.D); SQ(); break;                               // OR D | UNDOCUMENTED
                case 0xB3: OR_R(ref Registers.E); SQ(); break;                               // OR E | UNDOCUMENTED
                case 0xB4: OR_R(ref irH); SQ(); break;                                       // OR IRh | UNDOCUMENTED
                case 0xB5: OR_R(ref irL); SQ(); break;                                       // OR IRh | UNDOCUMENTED
                case 0xB6: OR_IRDMEM(ref indexAddressingMode); SQ(); _clock.Add(11); break;  // OR (IR + d)
                case 0xB7: OR_R(ref Registers.A); SQ(); break;                               // OR A | UNDOCUMENTED

                case 0xA8: XOR_R(ref Registers.B); SQ(); break;                               // XOR B | UNDOCUMENTED
                case 0xA9: XOR_R(ref Registers.C); SQ(); break;                               // XOR C | UNDOCUMENTED
                case 0xAA: XOR_R(ref Registers.D); SQ(); break;                               // XOR D | UNDOCUMENTED
                case 0xAB: XOR_R(ref Registers.E); SQ(); break;                               // XOR E | UNDOCUMENTED
                case 0xAC: XOR_R(ref irH); SQ(); break;                                       // XOR IRh | UNDOCUMENTED
                case 0xAD: XOR_R(ref irL); SQ(); break;                                       // XOR IRl | UNDOCUMENTED
                case 0xAE: XOR_IRDMEM(ref indexAddressingMode); SQ(); _clock.Add(11); break;  // XOR (IR + d)
                case 0xAF: XOR_R(ref Registers.A); SQ(); break;                               // XOR A | UNDOCUMENTED

                case 0xA0: AND_R(ref Registers.B); SQ(); break;                               // AND B | UNDOCUMENTED
                case 0xA1: AND_R(ref Registers.C); SQ(); break;                               // AND C | UNDOCUMENTED
                case 0xA2: AND_R(ref Registers.D); SQ(); break;                               // AND D | UNDOCUMENTED
                case 0xA3: AND_R(ref Registers.E); SQ(); break;                               // AND E | UNDOCUMENTED
                case 0xA4: AND_R(ref irH); SQ(); break;                                       // AND IRh | UNDOCUMENTED
                case 0xA5: AND_R(ref irL); SQ(); break;                                       // AND IRl | UNDOCUMENTED
                case 0xA6: AND_IRDMEM(ref indexAddressingMode); SQ(); _clock.Add(11); break;  // AND (IR + d)
                case 0xA7: AND_R(ref Registers.A); SQ(); break;                               // AND A | UNDOCUMENTED

                case 0xB8: CMP_R(ref Registers.B); SQ(); break;                               // CP B | UNDOCUMENTED
                case 0xB9: CMP_R(ref Registers.C); SQ(); break;                               // CP C | UNDOCUMENTED
                case 0xBA: CMP_R(ref Registers.D); SQ(); break;                               // CP D | UNDOCUMENTED
                case 0xBB: CMP_R(ref Registers.E); SQ(); break;                               // CP E | UNDOCUMENTED
                case 0xBC: CMP_R(ref irH); SQ(); break;                                       // CP IRh | UNDOCUMENTED
                case 0xBD: CMP_R(ref irL); SQ(); break;                                       // CP IRl | UNDOCUMENTED
                case 0xBE: CMP_IRDMEM(ref indexAddressingMode); SQ(); _clock.Add(11); break;  // CP (IR + d)
                case 0xBF: CMP_R(ref Registers.A); SQ(); break;                               // CP A | UNDOCUMENTED


                case 0x21: LD_RR_NN(ref indexAddressingMode); CQ(); _clock.Add(6); break;                   // LD IR, NN
                case 0xF9: LD_RR_RR(ref Registers.SP, ref indexAddressingMode); CQ(); _clock.Add(2); break; // LD SP, IR

                case 0x06: LD_R_N(ref Registers.B); CQ(); _clock.Add(3); break;    // LD B, N | UNDOCUMENTED
                case 0x0E: LD_R_N(ref Registers.C); CQ(); _clock.Add(3); break;    // LD C, N | UNDOCUMENTED
                case 0x16: LD_R_N(ref Registers.D); CQ(); _clock.Add(3); break;    // LD D, N | UNDOCUMENTED
                case 0x1E: LD_R_N(ref Registers.E); CQ(); _clock.Add(3); break;    // LD E, N | UNDOCUMENTED
                case 0x26: LD_R_N(ref irH); CQ(); _clock.Add(3); break;            // LD IRh, N | UNDOCUMENTED
                case 0x2E: LD_R_N(ref irL); CQ(); _clock.Add(3); break;            // LD IRl, N | UNDOCUMENTED
                case 0x3E: LD_R_N(ref Registers.A); CQ(); _clock.Add(3); break;    // LD A, N | UNDOCUMENTED

                case 0x22: LD_NNMEM_RR(ref indexAddressingMode); CQ(); _clock.Add(12); break;   // LD (NN), IR
                case 0x2A: LD_IR_NNMEM(ref indexAddressingMode); CQ(); _clock.Add(12); break;   // LD IR, (NN)

                case 0x40: LD_R_R(ref Registers.B, ref Registers.B); CQ(); break;                               // LD B, B | UNDOCUMENTED
                case 0x41: LD_R_R(ref Registers.B, ref Registers.C); CQ(); break;                               // LD B, C | UNDOCUMENTED
                case 0x42: LD_R_R(ref Registers.B, ref Registers.D); CQ(); break;                               // LD B, D | UNDOCUMENTED
                case 0x43: LD_R_R(ref Registers.B, ref Registers.E); CQ(); break;                               // LD B, E | UNDOCUMENTED
                case 0x44: LD_R_R(ref Registers.B, ref irH); CQ(); break;                                       // LD B, IRh | UNDOCUMENTED
                case 0x45: LD_R_R(ref Registers.B, ref irL); CQ(); break;                                       // LD B, IRl | UNDOCUMENTED
                case 0x46: LD_R_IRDMEM(ref Registers.B, ref indexAddressingMode); CQ(); _clock.Add(11); break;  // LD B, (IR + d)
                case 0x47: LD_R_R(ref Registers.B, ref Registers.A); CQ(); break;                               // LD B, A | UNDOCUMENTED

                case 0x48: LD_R_R(ref Registers.C, ref Registers.B); CQ(); break;                               // LD C, B | UNDOCUMENTED
                case 0x49: LD_R_R(ref Registers.C, ref Registers.C); CQ(); break;                               // LD C, C | UNDOCUMENTED
                case 0x4A: LD_R_R(ref Registers.C, ref Registers.D); CQ(); break;                               // LD C, D | UNDOCUMENTED
                case 0x4B: LD_R_R(ref Registers.C, ref Registers.E); CQ(); break;                               // LD C, E | UNDOCUMENTED
                case 0x4C: LD_R_R(ref Registers.C, ref irH); CQ(); break;                                       // LD C, IRh | UNDOCUMENTED
                case 0x4D: LD_R_R(ref Registers.C, ref irL); CQ(); break;                                       // LD C, IRl | UNDOCUMENTED
                case 0x4E: LD_R_IRDMEM(ref Registers.C, ref indexAddressingMode); CQ(); _clock.Add(11); break;  // LD C, (IR + d)
                case 0x4F: LD_R_R(ref Registers.C, ref Registers.A); CQ(); break;                               // LD C, A | UNDOCUMENTED

                case 0x50: LD_R_R(ref Registers.D, ref Registers.B); CQ(); break;                               // LD D, B | UNDOCUMENTED
                case 0x51: LD_R_R(ref Registers.D, ref Registers.C); CQ(); break;                               // LD D, C | UNDOCUMENTED
                case 0x52: LD_R_R(ref Registers.D, ref Registers.D); CQ(); break;                               // LD D, D | UNDOCUMENTED
                case 0x53: LD_R_R(ref Registers.D, ref Registers.E); CQ(); break;                               // LD D, E | UNDOCUMENTED
                case 0x54: LD_R_R(ref Registers.D, ref irH); CQ(); break;                                       // LD D, IRh | UNDOCUMENTED
                case 0x55: LD_R_R(ref Registers.D, ref irL); CQ(); break;                                       // LD D, IRl | UNDOCUMENTED
                case 0x56: LD_R_IRDMEM(ref Registers.D, ref indexAddressingMode); CQ(); _clock.Add(11); break;  // LD D, (IR + d)
                case 0x57: LD_R_R(ref Registers.D, ref Registers.A); CQ(); break;                               // LD D, A | UNDOCUMENTED

                case 0x58: LD_R_R(ref Registers.E, ref Registers.B); CQ(); break;                               // LD E, B | UNDOCUMENTED
                case 0x59: LD_R_R(ref Registers.E, ref Registers.C); CQ(); break;                               // LD E, C | UNDOCUMENTED
                case 0x5A: LD_R_R(ref Registers.E, ref Registers.D); CQ(); break;                               // LD E, D | UNDOCUMENTED
                case 0x5B: LD_R_R(ref Registers.E, ref Registers.E); CQ(); break;                               // LD E, E | UNDOCUMENTED
                case 0x5C: LD_R_R(ref Registers.E, ref irH); CQ(); break;                                       // LD E, IRh | UNDOCUMENTED
                case 0x5D: LD_R_R(ref Registers.E, ref irL); CQ(); break;                                       // LD E, IRl | UNDOCUMENTED
                case 0x5E: LD_R_IRDMEM(ref Registers.E, ref indexAddressingMode); CQ(); _clock.Add(11); break;  // LD E, (IR + d)
                case 0x5F: LD_R_R(ref Registers.E, ref Registers.A); CQ(); break;                               // LD E, A | UNDOCUMENTED

                case 0x60: LD_R_R(ref irH, ref Registers.B); CQ(); break;                                       // LD IRh, B | UNDOCUMENTED
                case 0x61: LD_R_R(ref irH, ref Registers.C); CQ(); break;                                       // LD IRh, C | UNDOCUMENTED
                case 0x62: LD_R_R(ref irH, ref Registers.D); CQ(); break;                                       // LD IRh, D | UNDOCUMENTED
                case 0x63: LD_R_R(ref irH, ref Registers.E); CQ(); break;                                       // LD IRh, E | UNDOCUMENTED
                case 0x64: LD_R_R(ref irH, ref irH); CQ(); break;                                               // LD IRh, IRh | UNDOCUMENTED
                case 0x65: LD_R_R(ref irH, ref irL); CQ(); break;                                               // LD IRh, IRl | UNDOCUMENTED
                case 0x66: LD_R_IRDMEM(ref Registers.H, ref indexAddressingMode); CQ(); _clock.Add(11); break;  // LD H, (IR + d)
                case 0x67: LD_R_R(ref irH, ref Registers.A); CQ(); break;                                       // LD IRh, A | UNDOCUMENTED

                case 0x68: LD_R_R(ref irL, ref Registers.B); CQ(); break;                                        // LD IRl, B | UNDOCUMENTED
                case 0x69: LD_R_R(ref irL, ref Registers.C); CQ(); break;                                        // LD IRl, C | UNDOCUMENTED
                case 0x6A: LD_R_R(ref irL, ref Registers.D); CQ(); break;                                        // LD IRl, D | UNDOCUMENTED
                case 0x6B: LD_R_R(ref irL, ref Registers.E); CQ(); break;                                        // LD IRl, E | UNDOCUMENTED
                case 0x6C: LD_R_R(ref irL, ref irH); CQ(); break;                                                // LD IRl, IRh | UNDOCUMENTED
                case 0x6D: LD_R_R(ref irL, ref irL); CQ(); break;                                                // LD IRl, IRl | UNDOCUMENTED
                case 0x6E: LD_R_IRDMEM(ref Registers.L, ref indexAddressingMode); CQ(); _clock.Add(11); break;   // LD L, (IR + d)
                case 0x6F: LD_R_R(ref irL, ref Registers.A); CQ(); break;                                        // LD IRl, A | UNDOCUMENTED

                case 0x78: LD_R_R(ref Registers.A, ref Registers.B); CQ(); break;                               // LD A, B | UNDOCUMENTED
                case 0x79: LD_R_R(ref Registers.A, ref Registers.C); CQ(); break;                               // LD A, C | UNDOCUMENTED
                case 0x7A: LD_R_R(ref Registers.A, ref Registers.D); CQ(); break;                               // LD A, D | UNDOCUMENTED
                case 0x7B: LD_R_R(ref Registers.A, ref Registers.E); CQ(); break;                               // LD A, E | UNDOCUMENTED
                case 0x7C: LD_R_R(ref Registers.A, ref irH); CQ(); break;                                       // LD A, IRh | UNDOCUMENTED
                case 0x7D: LD_R_R(ref Registers.A, ref irL); CQ(); break;                                       // LD A, IRl | UNDOCUMENTED
                case 0x7E: LD_R_IRDMEM(ref Registers.A, ref indexAddressingMode); CQ(); _clock.Add(11); break;  // LD A, (IR + d)
                case 0x7F: LD_R_R(ref Registers.A, ref Registers.A); CQ(); break;                               // LD A, A | UNDOCUMENTED

                case 0x70: LD_IRDMEM_R(ref indexAddressingMode, ref Registers.B); CQ(); _clock.Add(11); break;  // LD (IR + d), B
                case 0x71: LD_IRDMEM_R(ref indexAddressingMode, ref Registers.C); CQ(); _clock.Add(11); break;  // LD (IR + d), C
                case 0x72: LD_IRDMEM_R(ref indexAddressingMode, ref Registers.D); CQ(); _clock.Add(11); break;  // LD (IR + d), D
                case 0x73: LD_IRDMEM_R(ref indexAddressingMode, ref Registers.E); CQ(); _clock.Add(11); break;  // LD (IR + d), E
                case 0x74: LD_IRDMEM_R(ref indexAddressingMode, ref Registers.H); CQ(); _clock.Add(11); break;  // LD (IR + d), H
                case 0x75: LD_IRDMEM_R(ref indexAddressingMode, ref Registers.L); CQ(); _clock.Add(11); break;  // LD (IR + d), L
                case 0x77: LD_IRDMEM_R(ref indexAddressingMode, ref Registers.A); CQ(); _clock.Add(11); break;  // LD (IR + d), A
                case 0x36: LD_IRDMEM_N(ref indexAddressingMode); CQ(); _clock.Add(11); break;                   // LD (IR + d), N


                case 0xE9: JP_RR(ref indexAddressingMode); CQ(); break; // JP (IR)


                case 0xE1: POP_RR(ref indexAddressingMode); CQ();  _clock.Add(6); break;  // POP IR
                case 0xE5: PUSH_RR(ref indexAddressingMode); CQ(); _clock.Add(7); break; // PUSH IR


                case 0x23: INC_RR(ref indexAddressingMode); CQ(); _clock.Add(2); break;      // INC IR
                case 0x34: INC_IRDMEM(ref indexAddressingMode); SQ(); _clock.Add(15); break; // INC (IR + d)
                case 0x04: INC_R(ref Registers.B); SQ(); break;                              // INC B | UNDOCUMENTED
                case 0x0C: INC_R(ref Registers.C); SQ(); break;                              // INC C | UNDOCUMENTED
                case 0x14: INC_R(ref Registers.D); SQ(); break;                              // INC D | UNDOCUMENTED
                case 0x1C: INC_R(ref Registers.E); SQ(); break;                              // INC E | UNDOCUMENTED
                case 0x24: INC_R(ref irH); SQ(); break;                                      // INC IRh | UNDOCUMENTED
                case 0x2C: INC_R(ref irL); SQ(); break;                                      // INC IRl | UNDOCUMENTED
                case 0x3C: INC_R(ref Registers.A); SQ(); break;                              // INC A | UNDOCUMENTED


                case 0x2B: DEC_RR(ref indexAddressingMode); CQ(); _clock.Add(2); break;      // DEC IR
                case 0x35: DEC_IRDMEM(ref indexAddressingMode); SQ(); _clock.Add(15); break; // DEC (IR + d)
                case 0x05: DEC_R(ref Registers.B); SQ(); break;                              // DEC B | UNDOCUMENTED
                case 0x0D: DEC_R(ref Registers.C); SQ(); break;                              // DEC C | UNDOCUMENTED
                case 0x15: DEC_R(ref Registers.D); SQ(); break;                              // DEC D | UNDOCUMENTED
                case 0x1D: DEC_R(ref Registers.E); SQ(); break;                              // DEC E | UNDOCUMENTED
                case 0x25: DEC_R(ref irH); SQ(); break;                                      // DEC IRh | UNDOCUMENTED
                case 0x2D: DEC_R(ref irL); SQ(); break;                                      // DEC IRl | UNDOCUMENTED
                case 0x3D: DEC_R(ref Registers.A); SQ(); break;                              // DEC A | UNDOCUMENTED

                case 0xE3: EX_SPMEM_IR(ref indexAddressingMode); CQ(); _clock.Add(15); break; // EX (SP), IR

                default:
                    _logger.Log(LogSeverity.Fatal, $"Unrecognized INDR opcode: 0x{_currentInstruction:X2}");
                    Halted = true;
                    break;
            }
        }
    }
}