using Z80Sharp.Enums;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void ExecuteIndexRInstruction(AddressingMode indexAddressingMode)
        {
            LogInstructionDecode($"INDR decoded: 0x{_currentInstruction:X2}");
            byte instruction = Fetch();
            _currentInstruction = instruction;
            switch (instruction)
            {
                case 0xCB: ExecuteIndexRBitInstruction(indexAddressingMode); break; // IndexR Bit

                // AD(D/C) instructions: ADD register and operand (+ carry if ADC).
                case 0x09: ADD_IR_RR((byte)indexAddressingMode, B); break;   // ADD IR, BC
                case 0x19: ADD_IR_RR((byte)indexAddressingMode, D); break;   // ADD IR, DE
                case 0x29: ADD_IR_RR((byte)indexAddressingMode, H); break;   // ADD IR, HL
                case 0x39: ADD_IR_RR((byte)indexAddressingMode, SPi); break; // ADD IR, SP

                case 0x80: ADD_A_R(B); break;     // ADD A, B | UNDOCUMENTED
                case 0x81: ADD_A_R(C); break;     // ADD A, C | UNDOCUMENTED
                case 0x82: ADD_A_R(D); break;     // ADD A, D | UNDOCUMENTED
                case 0x83: ADD_A_R(E); break;     // ADD A, E | UNDOCUMENTED
                case 0x84: ADD_A_R((byte)indexAddressingMode); break;       // ADD A, IRh | UNDOCUMENTED
                case 0x85: ADD_A_R((byte)(indexAddressingMode + 1)); break; // ADD A, IRl | UNDOCUMENTED
                case 0x86: ADD_A_IRDMEM((byte)indexAddressingMode); break;  // ADD A, (IR + d)
                case 0x87: ADD_A_R(A); break;     // ADD A, A | UNDOCUMENTED

                case 0x88: ADC_A_R(B); break;     // ADC A, B | UNDOCUMENTED
                case 0x89: ADC_A_R(C); break;     // ADC A, C | UNDOCUMENTED
                case 0x8A: ADC_A_R(D); break;     // ADC A, D | UNDOCUMENTED
                case 0x8B: ADC_A_R(E); break;     // ADC A, E | UNDOCUMENTED
                case 0x8C: ADC_A_R((byte)indexAddressingMode); break;       // ADC A, H | UNDOCUMENTED
                case 0x8D: ADC_A_R((byte)(indexAddressingMode + 1)); break; // ADC A, L | UNDOCUMENTED
                case 0x8E: ADC_A_IRDMEM((byte)indexAddressingMode); break;  // ADC A, (IR + d)
                case 0x8F: ADC_A_R(A); break;     // ADC A, A | UNDOCUMENTED

                // SUB/SBC instructions: SUB operand from register (- carry if SBC).
                case 0x90: SUB_R(B); break;     // SUB B | UNDOCUMENTED
                case 0x91: SUB_R(C); break;     // SUB C | UNDOCUMENTED
                case 0x92: SUB_R(D); break;     // SUB D | UNDOCUMENTED
                case 0x93: SUB_R(E); break;     // SUB E | UNDOCUMENTED
                case 0x94: SUB_R((byte)indexAddressingMode); break;       // SUB IRh | UNDOCUMENTED
                case 0x95: SUB_R((byte)(indexAddressingMode + 1)); break; // SUB IRl | UNDOCUMENTED
                case 0x96: SUB_IRDMEM((byte)indexAddressingMode); break;  // SUB (IR + d)
                case 0x97: SUB_R(A); break;     // SUB A | UNDOCUMENTED

                case 0x98: SBC_A_R(B); break;     // SBC A, B | UNDOCUMENTED
                case 0x99: SBC_A_R(C); break;     // SBC A, C | UNDOCUMENTED
                case 0x9A: SBC_A_R(D); break;     // SBC A, D | UNDOCUMENTED
                case 0x9B: SBC_A_R(E); break;     // SBC A, E | UNDOCUMENTED
                case 0x9C: SBC_A_R((byte)indexAddressingMode); break;       // SBC A, IRh | UNDOCUMENTED
                case 0x9D: SBC_A_R((byte)(indexAddressingMode + 1)); break; // SBC A, IRl | UNDOCUMENTED
                case 0x9E: SBC_A_IRDMEM((byte)indexAddressingMode); break;  // SBC A, (IR + d)
                case 0x9F: SBC_A_R(A); break;     // SBC A, A | UNDOCUMENTED

                // OR instructions: OR accumulator with operand.
                case 0xB0: OR_R(B); break;     // OR B | UNDOCUMENTED
                case 0xB1: OR_R(C); break;     // OR C | UNDOCUMENTED
                case 0xB2: OR_R(D); break;     // OR D | UNDOCUMENTED
                case 0xB3: OR_R(E); break;     // OR E | UNDOCUMENTED
                case 0xB4: OR_R((byte)indexAddressingMode); break;       // OR IRh | UNDOCUMENTED
                case 0xB5: OR_R((byte)(indexAddressingMode + 1)); break; // OR IRh | UNDOCUMENTED
                case 0xB6: OR_IRDMEM((byte)indexAddressingMode); break;  // OR (IR + d)
                case 0xB7: OR_R(A); break;     // OR A | UNDOCUMENTED

                // XOR instructions: XOR accumulator with operand.
                case 0xA8: XOR_R(B); break;     // XOR B | UNDOCUMENTED
                case 0xA9: XOR_R(C); break;     // XOR C | UNDOCUMENTED
                case 0xAA: XOR_R(D); break;     // XOR D | UNDOCUMENTED
                case 0xAB: XOR_R(E); break;     // XOR E | UNDOCUMENTED
                case 0xAC: XOR_R((byte)indexAddressingMode); break;       // XOR IRh | UNDOCUMENTED
                case 0xAD: XOR_R((byte)(indexAddressingMode + 1)); break; // XOR IRl | UNDOCUMENTED
                case 0xAE: XOR_IRDMEM((byte)indexAddressingMode); break;  // XOR (IR + d)
                case 0xAF: XOR_R(A); break;     // XOR A | UNDOCUMENTED

                // AND instructions: AND accumulator with operand.
                case 0xA0: AND_R(B); break;     // AND B | UNDOCUMENTED
                case 0xA1: AND_R(C); break;     // AND C | UNDOCUMENTED
                case 0xA2: AND_R(D); break;     // AND D | UNDOCUMENTED
                case 0xA3: AND_R(E); break;     // AND E | UNDOCUMENTED
                case 0xA4: AND_R((byte)indexAddressingMode); break;       // AND IRh | UNDOCUMENTED
                case 0xA5: AND_R((byte)(indexAddressingMode + 1)); break; // AND IRl | UNDOCUMENTED
                case 0xA6: AND_IRDMEM((byte)indexAddressingMode); break;  // AND (IR + d)
                case 0xA7: AND_R(A); break;     // AND A | UNDOCUMENTED

                // CP instructions: Compare accumulator with operand diff.
                case 0xB8: CMP_R(B); break;     // CP B | UNDOCUMENTED
                case 0xB9: CMP_R(C); break;     // CP C | UNDOCUMENTED
                case 0xBA: CMP_R(D); break;     // CP D | UNDOCUMENTED
                case 0xBB: CMP_R(E); break;     // CP E | UNDOCUMENTED
                case 0xBC: CMP_R((byte)indexAddressingMode); break;       // CP IRh | UNDOCUMENTED
                case 0xBD: CMP_R((byte)(indexAddressingMode + 1)); break; // CP IRl | UNDOCUMENTED
                case 0xBE: CMP_IRDMEM((byte)indexAddressingMode); break;  // CP (IR + d)
                case 0xBF: CMP_R(A); break;     // CP A | UNDOCUMENTED


                // LD instructions: Load/store values between/from registers or memory
                case 0x21: LD_RR_NN((byte)indexAddressingMode); break;  // LD IR, NN
                case 0xF9: LD_RR_RR(SPi, (byte)indexAddressingMode); break; // LD SP, IR

                case 0x06: LD_R_N(B); break;    // LD B, N | UNDOCUMENTED
                case 0x0E: LD_R_N(C); break;    // LD C, N | UNDOCUMENTED
                case 0x16: LD_R_N(D); break;    // LD D, N | UNDOCUMENTED
                case 0x1E: LD_R_N(E); break;    // LD E, N | UNDOCUMENTED
                case 0x26: LD_R_N((byte)indexAddressingMode); break;        // LD IRh, N | UNDOCUMENTED
                case 0x2E: LD_R_N((byte)(indexAddressingMode + 1)); break;  // LD IRl, N | UNDOCUMENTED
                case 0x3E: LD_R_N(A); break;    // LD A, N | UNDOCUMENTED

                case 0x22: LD_NNMEM_RR((byte)indexAddressingMode); break;   // LD (NN), IR
                case 0x2A: LD_RR_NNMEM((byte)indexAddressingMode); break;   // LD IR, (NN)

                case 0x40: LD_R_R(B, B); break;  // LD B, B | UNDOCUMENTED
                case 0x41: LD_R_R(B, C); break;  // LD B, C | UNDOCUMENTED
                case 0x42: LD_R_R(B, D); break;  // LD B, D | UNDOCUMENTED
                case 0x43: LD_R_R(B, E); break;  // LD B, E | UNDOCUMENTED
                case 0x44: LD_R_R(B, (byte)indexAddressingMode); break;       // LD B, IRh | UNDOCUMENTED
                case 0x45: LD_R_R(B, (byte)(indexAddressingMode + 1)); break; // LD B, IRl | UNDOCUMENTED
                case 0x46: LD_R_IRDMEM(B, (byte)indexAddressingMode); break;  // LD B, (IR + d)
                case 0x47: LD_R_R(B, A); break;  // LD B, A | UNDOCUMENTED

                case 0x48: LD_R_R(C, B); break;  // LD C, B | UNDOCUMENTED
                case 0x49: LD_R_R(C, C); break;  // LD C, C | UNDOCUMENTED
                case 0x4A: LD_R_R(C, D); break;  // LD C, D | UNDOCUMENTED
                case 0x4B: LD_R_R(C, E); break;  // LD C, E | UNDOCUMENTED
                case 0x4C: LD_R_R(C, (byte)indexAddressingMode); break;       // LD C, IRh | UNDOCUMENTED
                case 0x4D: LD_R_R(C, (byte)(indexAddressingMode + 1)); break; // LD C, IRl | UNDOCUMENTED
                case 0x4E: LD_R_IRDMEM(C, (byte)indexAddressingMode); break;  // LD C, (IR + d)
                case 0x4F: LD_R_R(C, A); break;  // LD C, A | UNDOCUMENTED

                case 0x50: LD_R_R(D, B); break;  // LD D, B | UNDOCUMENTED
                case 0x51: LD_R_R(D, C); break;  // LD D, C | UNDOCUMENTED
                case 0x52: LD_R_R(D, D); break;  // LD D, D | UNDOCUMENTED
                case 0x53: LD_R_R(D, E); break;  // LD D, E | UNDOCUMENTED
                case 0x54: LD_R_R(D, (byte)indexAddressingMode); break;       // LD D, IRh | UNDOCUMENTED
                case 0x55: LD_R_R(D, (byte)(indexAddressingMode + 1)); break; // LD D, IRl | UNDOCUMENTED
                case 0x56: LD_R_IRDMEM(D, (byte)indexAddressingMode); break;  // LD D, (IR + d)
                case 0x57: LD_R_R(D, A); break;  // LD D, A | UNDOCUMENTED

                case 0x58: LD_R_R(E, B); break;  // LD E, B | UNDOCUMENTED
                case 0x59: LD_R_R(E, C); break;  // LD E, C | UNDOCUMENTED
                case 0x5A: LD_R_R(E, D); break;  // LD E, D | UNDOCUMENTED
                case 0x5B: LD_R_R(E, E); break;  // LD E, E | UNDOCUMENTED
                case 0x5C: LD_R_R(E, (byte)indexAddressingMode); break;       // LD E, IRh | UNDOCUMENTED
                case 0x5D: LD_R_R(E, (byte)(indexAddressingMode + 1)); break; // LD E, IRl | UNDOCUMENTED
                case 0x5E: LD_R_IRDMEM(E, (byte)indexAddressingMode); break;  // LD E, (IR + d)
                case 0x5F: LD_R_R(E, A); break;  // LD E, A | UNDOCUMENTED

                case 0x60: LD_R_R((byte)indexAddressingMode, B); break;  // LD IRh, B | UNDOCUMENTED
                case 0x61: LD_R_R((byte)indexAddressingMode, C); break;  // LD IRh, C | UNDOCUMENTED
                case 0x62: LD_R_R((byte)indexAddressingMode, D); break;  // LD IRh, D | UNDOCUMENTED
                case 0x63: LD_R_R((byte)indexAddressingMode, E); break;  // LD IRh, E | UNDOCUMENTED
                case 0x64: LD_R_R((byte)indexAddressingMode, H); break;  // LD IRh, H | UNDOCUMENTED
                case 0x65: LD_R_R((byte)indexAddressingMode, L); break;  // LD IRh, L | UNDOCUMENTED
                case 0x66: LD_R_IRDMEM(H, (byte)indexAddressingMode); break;  // LD H, (IR + d)
                case 0x67: LD_R_R((byte)indexAddressingMode, A); break;  // LD IRh, A | UNDOCUMENTED

                case 0x68: LD_R_R((byte)(indexAddressingMode + 1), B); break;  // LD IRl, B | UNDOCUMENTED
                case 0x69: LD_R_R((byte)(indexAddressingMode + 1), C); break;  // LD IRl, C | UNDOCUMENTED
                case 0x6A: LD_R_R((byte)(indexAddressingMode + 1), D); break;  // LD IRl, D | UNDOCUMENTED
                case 0x6B: LD_R_R((byte)(indexAddressingMode + 1), E); break;  // LD IRl, E | UNDOCUMENTED
                case 0x6C: LD_R_R((byte)(indexAddressingMode + 1), H); break;  // LD IRl, H | UNDOCUMENTED
                case 0x6D: LD_R_R((byte)(indexAddressingMode + 1), L); break;  // LD IRl, L | UNDOCUMENTED
                case 0x6E: LD_R_IRDMEM(L, (byte)indexAddressingMode); break;   // LD L, (IR + d)
                case 0x6F: LD_R_R((byte)(indexAddressingMode + 1), A); break;  // LD IRl, A | UNDOCUMENTED

                case 0x78: LD_R_R(A, B); break;  // LD A, B | UNDOCUMENTED
                case 0x79: LD_R_R(A, C); break;  // LD A, C | UNDOCUMENTED
                case 0x7A: LD_R_R(A, D); break;  // LD A, D | UNDOCUMENTED
                case 0x7B: LD_R_R(A, E); break;  // LD A, E | UNDOCUMENTED
                case 0x7C: LD_R_R(A, (byte)indexAddressingMode); break;       // LD A, IRh | UNDOCUMENTED
                case 0x7D: LD_R_R(A, (byte)(indexAddressingMode + 1)); break; // LD A, IRl | UNDOCUMENTED
                case 0x7E: LD_R_IRDMEM(A, (byte)indexAddressingMode); break;  // LD A, (IR + d)
                case 0x7F: LD_R_R(A, A); break;  // LD A, A | UNDOCUMENTED

                case 0x70: LD_IRDMEM_R((byte)indexAddressingMode, B); break;  // LD (IR + d), B
                case 0x71: LD_IRDMEM_R((byte)indexAddressingMode, C); break;  // LD (IR + d), C
                case 0x72: LD_IRDMEM_R((byte)indexAddressingMode, D); break;  // LD (IR + d), D
                case 0x73: LD_IRDMEM_R((byte)indexAddressingMode, E); break;  // LD (IR + d), E
                case 0x74: LD_IRDMEM_R((byte)indexAddressingMode, H); break;  // LD (IR + d), H
                case 0x75: LD_IRDMEM_R((byte)indexAddressingMode, L); break;  // LD (IR + d), L
                case 0x77: LD_IRDMEM_R((byte)indexAddressingMode, A); break;  // LD (IR + d), A

                case 0x36: LD_IRDMEM_N((byte)indexAddressingMode); break; // LD (IR + d), N


                // JP: Jump to absolute address
                case 0xE9: JP_RR((byte)indexAddressingMode); break; // JP (IR)


                // POP instructions: pop value at SP into RR and inc. SP
                case 0xE1: POP_RR((byte)indexAddressingMode); break;  // POP IR
                // PUSH instructions: push value in RR on to stack and dec. SP
                case 0xE5: PUSH_RR((byte)indexAddressingMode); break; // PUSH IR


                // INC instructions: increments RR/R by 1.
                case 0x23: INC_RR((byte)indexAddressingMode); break; // INC IR
                case 0x34: INC_HLMEM(); break; // INC (HL)
                case 0x04: INC_R(B); break;    // INC B | UNDOCUMENTED
                case 0x0C: INC_R(C); break;    // INC C | UNDOCUMENTED
                case 0x14: INC_R(D); break;    // INC D | UNDOCUMENTED
                case 0x1C: INC_R(E); break;    // INC E | UNDOCUMENTED
                case 0x24: INC_R((byte)indexAddressingMode); break;       // INC IRh | UNDOCUMENTED
                case 0x2C: INC_R((byte)(indexAddressingMode + 1)); break; // INC IRl | UNDOCUMENTED
                case 0x3C: INC_R(A); break;    // INC A | UNDOCUMENTED

                // DEC instructions: decrements RR/R by 1.
                case 0x2B: DEC_RR((byte)indexAddressingMode); break; // DEC IR
                case 0x35: DEC_HLMEM(); break; // DEC (HL)
                case 0x05: DEC_R(B); break;    // DEC B | UNDOCUMENTED
                case 0x0D: DEC_R(C); break;    // DEC C | UNDOCUMENTED
                case 0x15: DEC_R(D); break;    // DEC D | UNDOCUMENTED
                case 0x1D: DEC_R(E); break;    // DEC E | UNDOCUMENTED
                case 0x25: DEC_R((byte)indexAddressingMode); break;       // DEC IRh | UNDOCUMENTED
                case 0x2D: DEC_R((byte)(indexAddressingMode + 1)); break; // DEC IRl | UNDOCUMENTED
                case 0x3D: DEC_R(A); break;    // DEC A | UNDOCUMENTED

                case 0xE3: EX_SPMEM_IR((byte)indexAddressingMode); break; // EX (SP), IR

                default:
                    _logger.Log(LogSeverity.Fatal, $"Unrecognized INDR opcode: 0x{_currentInstruction:X2}");
                    Halted = true;
                    break;
            }
        }
    }
}