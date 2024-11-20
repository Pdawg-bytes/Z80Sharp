using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;
using Z80Sharp.Enums;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExecuteIndexRBitInstruction([ConstantExpected] byte indexAddressingMode)
        {
            //LogInstructionDecode($"INRB decoded: 0x{_currentInstruction:X2}");
            sbyte displacement = (sbyte)Fetch(); // Displacement comes before opcode in index bit
            byte instruction = Fetch();          // So we fetch actual opcode after displacement
            _currentInstruction = instruction;
            switch (instruction)
            {
                // RL(C) instructions: Rotate (IR + d) left through/with carry
                case 0x00: RLC_IRDMEM_R(displacement, indexAddressingMode, B); break; // RLC (IR + d), B | UNDOCUMENTED
                case 0x01: RLC_IRDMEM_R(displacement, indexAddressingMode, C); break; // RLC (IR + d), C | UNDOCUMENTED
                case 0x02: RLC_IRDMEM_R(displacement, indexAddressingMode, D); break; // RLC (IR + d), D | UNDOCUMENTED
                case 0x03: RLC_IRDMEM_R(displacement, indexAddressingMode, E); break; // RLC (IR + d), E | UNDOCUMENTED
                case 0x04: RLC_IRDMEM_R(displacement, indexAddressingMode, H); break; // RLC (IR + d), H | UNDOCUMENTED
                case 0x05: RLC_IRDMEM_R(displacement, indexAddressingMode, L); break; // RLC (IR + d), L | UNDOCUMENTED
                case 0x07: RLC_IRDMEM_R(displacement, indexAddressingMode, A); break; // RLC (IR + d), A | UNDOCUMENTED
                case 0x06: RLC_IRDMEM(displacement, indexAddressingMode); break;      // RLC (IR + d)

                case 0x10: RL_IRDMEM_R(displacement, indexAddressingMode, B); break; // RL (IR + d), B | UNDOCUMENTED
                case 0x11: RL_IRDMEM_R(displacement, indexAddressingMode, C); break; // RL (IR + d), C | UNDOCUMENTED
                case 0x12: RL_IRDMEM_R(displacement, indexAddressingMode, D); break; // RL (IR + d), D | UNDOCUMENTED
                case 0x13: RL_IRDMEM_R(displacement, indexAddressingMode, E); break; // RL (IR + d), E | UNDOCUMENTED
                case 0x14: RL_IRDMEM_R(displacement, indexAddressingMode, H); break; // RL (IR + d), H | UNDOCUMENTED
                case 0x15: RL_IRDMEM_R(displacement, indexAddressingMode, L); break; // RL (IR + d), L | UNDOCUMENTED
                case 0x17: RL_IRDMEM_R(displacement, indexAddressingMode, A); break; // RL (IR + d), A | UNDOCUMENTED
                case 0x16: RL_IRDMEM(displacement, indexAddressingMode); break;      // RL (IR + d)


                // RR(C) instructions: Rotate (IR + d) right through/with carry
                case 0x08: RRC_IRDMEM_R(displacement, indexAddressingMode, B); break; // RRC (IR + d), B | UNDOCUMENTED
                case 0x09: RRC_IRDMEM_R(displacement, indexAddressingMode, C); break; // RRC (IR + d), C | UNDOCUMENTED
                case 0x0A: RRC_IRDMEM_R(displacement, indexAddressingMode, D); break; // RRC (IR + d), D | UNDOCUMENTED
                case 0x0B: RRC_IRDMEM_R(displacement, indexAddressingMode, E); break; // RRC (IR + d), E | UNDOCUMENTED
                case 0x0C: RRC_IRDMEM_R(displacement, indexAddressingMode, H); break; // RRC (IR + d), H | UNDOCUMENTED
                case 0x0D: RRC_IRDMEM_R(displacement, indexAddressingMode, L); break; // RRC (IR + d), L | UNDOCUMENTED
                case 0x0F: RRC_IRDMEM_R(displacement, indexAddressingMode, A); break; // RRC (IR + d), A | UNDOCUMENTED
                case 0x0E: RRC_IRDMEM(displacement, indexAddressingMode); break;      // RRC (IR + d)

                case 0x18: RR_IRDMEM_R(displacement, indexAddressingMode, B); break; // RR (IR + d), B | UNDOCUMENTED
                case 0x19: RR_IRDMEM_R(displacement, indexAddressingMode, C); break; // RR (IR + d), C | UNDOCUMENTED
                case 0x1A: RR_IRDMEM_R(displacement, indexAddressingMode, D); break; // RR (IR + d), D | UNDOCUMENTED
                case 0x1B: RR_IRDMEM_R(displacement, indexAddressingMode, E); break; // RR (IR + d), E | UNDOCUMENTED
                case 0x1C: RR_IRDMEM_R(displacement, indexAddressingMode, H); break; // RR (IR + d), H | UNDOCUMENTED
                case 0x1D: RR_IRDMEM_R(displacement, indexAddressingMode, L); break; // RR (IR + d), L | UNDOCUMENTED
                case 0x1F: RR_IRDMEM_R(displacement, indexAddressingMode, A); break; // RR (IR + d), A | UNDOCUMENTED
                case 0x1E: RR_IRDMEM(displacement, indexAddressingMode); break;      // RR (IR + d)


                // SL(A/L) instructions: Shift (IR + d) left
                case 0x20: SLA_IRDMEM_R(displacement, indexAddressingMode, B); break; // SLA (IR + d), B | UNDOCUMENTED
                case 0x21: SLA_IRDMEM_R(displacement, indexAddressingMode, C); break; // SLA (IR + d), C | UNDOCUMENTED
                case 0x22: SLA_IRDMEM_R(displacement, indexAddressingMode, D); break; // SLA (IR + d), D | UNDOCUMENTED
                case 0x23: SLA_IRDMEM_R(displacement, indexAddressingMode, E); break; // SLA (IR + d), E | UNDOCUMENTED
                case 0x24: SLA_IRDMEM_R(displacement, indexAddressingMode, H); break; // SLA (IR + d), H | UNDOCUMENTED
                case 0x25: SLA_IRDMEM_R(displacement, indexAddressingMode, L); break; // SLA (IR + d), L | UNDOCUMENTED
                case 0x27: SLA_IRDMEM_R(displacement, indexAddressingMode, A); break; // SLA (IR + d), A | UNDOCUMENTED
                case 0x26: SLA_IRDMEM(displacement, indexAddressingMode); break;      // SLA (IR + d)

                case 0x30: SLL_IRDMEM_R(displacement, indexAddressingMode, B); break; // SLL (IR + d), B | UNDOCUMENTED
                case 0x31: SLL_IRDMEM_R(displacement, indexAddressingMode, C); break; // SLL (IR + d), C | UNDOCUMENTED
                case 0x32: SLL_IRDMEM_R(displacement, indexAddressingMode, D); break; // SLL (IR + d), D | UNDOCUMENTED
                case 0x33: SLL_IRDMEM_R(displacement, indexAddressingMode, E); break; // SLL (IR + d), E | UNDOCUMENTED
                case 0x34: SLL_IRDMEM_R(displacement, indexAddressingMode, H); break; // SLL (IR + d), H | UNDOCUMENTED
                case 0x35: SLL_IRDMEM_R(displacement, indexAddressingMode, L); break; // SLL (IR + d), L | UNDOCUMENTED
                case 0x37: SLL_IRDMEM_R(displacement, indexAddressingMode, A); break; // SLL (IR + d), A | UNDOCUMENTED
                case 0x36: SLL_IRDMEM(displacement, indexAddressingMode); break;      // SLL (IR + d)


                // SR(A/L) instructions: Shift (IR + d) right
                case 0x28: SRA_IRDMEM_R(displacement, indexAddressingMode, B); break; // SRA (IR + d), B | UNDOCUMENTED
                case 0x29: SRA_IRDMEM_R(displacement, indexAddressingMode, C); break; // SRA (IR + d), C | UNDOCUMENTED
                case 0x2A: SRA_IRDMEM_R(displacement, indexAddressingMode, D); break; // SRA (IR + d), D | UNDOCUMENTED
                case 0x2B: SRA_IRDMEM_R(displacement, indexAddressingMode, E); break; // SRA (IR + d), E | UNDOCUMENTED
                case 0x2C: SRA_IRDMEM_R(displacement, indexAddressingMode, H); break; // SRA (IR + d), H | UNDOCUMENTED
                case 0x2D: SRA_IRDMEM_R(displacement, indexAddressingMode, L); break; // SRA (IR + d), L | UNDOCUMENTED
                case 0x2E: SRA_IRDMEM_R(displacement, indexAddressingMode, A); break; // SRA (IR + d), A | UNDOCUMENTED
                case 0x2F: SRA_IRDMEM(displacement, indexAddressingMode); break;      // SRA (IR + d)

                case 0x38: SRL_IRDMEM_R(displacement, indexAddressingMode, B); break; // SRL (IR + d), B | UNDOCUMENTED
                case 0x39: SRL_IRDMEM_R(displacement, indexAddressingMode, C); break; // SRL (IR + d), C | UNDOCUMENTED
                case 0x3A: SRL_IRDMEM_R(displacement, indexAddressingMode, D); break; // SRL (IR + d), D | UNDOCUMENTED
                case 0x3B: SRL_IRDMEM_R(displacement, indexAddressingMode, E); break; // SRL (IR + d), E | UNDOCUMENTED
                case 0x3C: SRL_IRDMEM_R(displacement, indexAddressingMode, H); break; // SRL (IR + d), H | UNDOCUMENTED
                case 0x3D: SRL_IRDMEM_R(displacement, indexAddressingMode, L); break; // SRL (IR + d), L | UNDOCUMENTED
                case 0x3E: SRL_IRDMEM_R(displacement, indexAddressingMode, A); break; // SRL (IR + d), A | UNDOCUMENTED
                case 0x3F: SRL_IRDMEM(displacement, indexAddressingMode); break;      // SRL (IR + d)


                // BIT instructions: Tests bit B of register R, sets Z if tested bit is zero
                case 0x40:  // BIT 0, (IR + d) | UNDOCUMENTED
                case 0x41:  // BIT 0, (IR + d) | UNDOCUMENTED
                case 0x42:  // BIT 0, (IR + d) | UNDOCUMENTED
                case 0x43:  // BIT 0, (IR + d) | UNDOCUMENTED
                case 0x44:  // BIT 0, (IR + d) | UNDOCUMENTED
                case 0x45:  // BIT 0, (IR + d) | UNDOCUMENTED
                case 0x46:  // BIT 0, (IR + d)
                case 0x47:  // BIT 0, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(0, displacement, indexAddressingMode); break;

                case 0x48:  // BIT 1, (IR + d) | UNDOCUMENTED
                case 0x49:  // BIT 1, (IR + d) | UNDOCUMENTED
                case 0x4A:  // BIT 1, (IR + d) | UNDOCUMENTED
                case 0x4B:  // BIT 1, (IR + d) | UNDOCUMENTED
                case 0x4C:  // BIT 1, (IR + d) | UNDOCUMENTED
                case 0x4D:  // BIT 1, (IR + d) | UNDOCUMENTED
                case 0x4E:  // BIT 1, (IR + d)
                case 0x4F:  // BIT 1, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(1, displacement, indexAddressingMode); break;

                case 0x50:  // BIT 2, (IR + d) | UNDOCUMENTED
                case 0x51:  // BIT 2, (IR + d) | UNDOCUMENTED
                case 0x52:  // BIT 2, (IR + d) | UNDOCUMENTED
                case 0x53:  // BIT 2, (IR + d) | UNDOCUMENTED
                case 0x54:  // BIT 2, (IR + d) | UNDOCUMENTED
                case 0x55:  // BIT 2, (IR + d) | UNDOCUMENTED
                case 0x56:  // BIT 2, (IR + d)
                case 0x57:  // BIT 2, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(2, displacement, indexAddressingMode); break;

                case 0x58:  // BIT 3, (IR + d) | UNDOCUMENTED
                case 0x59:  // BIT 3, (IR + d) | UNDOCUMENTED
                case 0x5A:  // BIT 3, (IR + d) | UNDOCUMENTED
                case 0x5B:  // BIT 3, (IR + d) | UNDOCUMENTED
                case 0x5C:  // BIT 3, (IR + d) | UNDOCUMENTED
                case 0x5D:  // BIT 3, (IR + d) | UNDOCUMENTED
                case 0x5E:  // BIT 3, (IR + d)
                case 0x5F:  // BIT 3, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(3, displacement, indexAddressingMode); break;

                case 0x60:  // BIT 4, (IR + d) | UNDOCUMENTED
                case 0x61:  // BIT 4, (IR + d) | UNDOCUMENTED
                case 0x62:  // BIT 4, (IR + d) | UNDOCUMENTED
                case 0x63:  // BIT 4, (IR + d) | UNDOCUMENTED
                case 0x64:  // BIT 4, (IR + d) | UNDOCUMENTED
                case 0x65:  // BIT 4, (IR + d) | UNDOCUMENTED
                case 0x66:  // BIT 4, (IR + d)
                case 0x67:  // BIT 4, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(4, displacement, indexAddressingMode); break;

                case 0x68:  // BIT 5, (IR + d) | UNDOCUMENTED
                case 0x69:  // BIT 5, (IR + d) | UNDOCUMENTED
                case 0x6A:  // BIT 5, (IR + d) | UNDOCUMENTED
                case 0x6B:  // BIT 5, (IR + d) | UNDOCUMENTED
                case 0x6C:  // BIT 5, (IR + d) | UNDOCUMENTED
                case 0x6D:  // BIT 5, (IR + d) | UNDOCUMENTED
                case 0x6E:  // BIT 5, (IR + d)
                case 0x6F:  // BIT 5, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(5, displacement, indexAddressingMode); break;

                case 0x70:  // BIT 6, (IR + d) | UNDOCUMENTED
                case 0x71:  // BIT 6, (IR + d) | UNDOCUMENTED
                case 0x72:  // BIT 6, (IR + d) | UNDOCUMENTED
                case 0x73:  // BIT 6, (IR + d) | UNDOCUMENTED
                case 0x74:  // BIT 6, (IR + d) | UNDOCUMENTED
                case 0x75:  // BIT 6, (IR + d) | UNDOCUMENTED
                case 0x76:  // BIT 6, (IR + d)
                case 0x77:  // BIT 6, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(6, displacement, indexAddressingMode); break;

                case 0x78:  // BIT 7, (IR + d) | UNDOCUMENTED
                case 0x79:  // BIT 7, (IR + d) | UNDOCUMENTED
                case 0x7A:  // BIT 7, (IR + d) | UNDOCUMENTED
                case 0x7B:  // BIT 7, (IR + d) | UNDOCUMENTED
                case 0x7C:  // BIT 7, (IR + d) | UNDOCUMENTED
                case 0x7D:  // BIT 7, (IR + d) | UNDOCUMENTED
                case 0x7E:  // BIT 7, (IR + d)
                case 0x7F:  // BIT 7, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(7, displacement, indexAddressingMode); break;


                // RES instructions: Reset bit B of (IR + d)
                case 0x80: RES_B_IRDMEM_R(0, displacement, indexAddressingMode, B); break; // RES 0, (IX + d), B | UNDOCUMENTED
                case 0x81: RES_B_IRDMEM_R(0, displacement, indexAddressingMode, C); break; // RES 0, (IX + d), C | UNDOCUMENTED
                case 0x82: RES_B_IRDMEM_R(0, displacement, indexAddressingMode, D); break; // RES 0, (IX + d), D | UNDOCUMENTED
                case 0x83: RES_B_IRDMEM_R(0, displacement, indexAddressingMode, E); break; // RES 0, (IX + d), E | UNDOCUMENTED
                case 0x84: RES_B_IRDMEM_R(0, displacement, indexAddressingMode, H); break; // RES 0, (IX + d), H | UNDOCUMENTED
                case 0x85: RES_B_IRDMEM_R(0, displacement, indexAddressingMode, L); break; // RES 0, (IX + d), L | UNDOCUMENTED
                case 0x87: RES_B_IRDMEM_R(0, displacement, indexAddressingMode, A); break; // RES 0, (IX + d), A | UNDOCUMENTED
                case 0x86: RES_B_IRDMEM(0, displacement, indexAddressingMode); break;      // RES 0, (IX + d)

                case 0x88: RES_B_IRDMEM_R(1, displacement, indexAddressingMode, B); break; // RES 1, (IX + d), B | UNDOCUMENTED
                case 0x89: RES_B_IRDMEM_R(1, displacement, indexAddressingMode, C); break; // RES 1, (IX + d), C | UNDOCUMENTED
                case 0x8A: RES_B_IRDMEM_R(1, displacement, indexAddressingMode, D); break; // RES 1, (IX + d), D | UNDOCUMENTED
                case 0x8B: RES_B_IRDMEM_R(1, displacement, indexAddressingMode, E); break; // RES 1, (IX + d), E | UNDOCUMENTED
                case 0x8C: RES_B_IRDMEM_R(1, displacement, indexAddressingMode, H); break; // RES 1, (IX + d), H | UNDOCUMENTED
                case 0x8D: RES_B_IRDMEM_R(1, displacement, indexAddressingMode, L); break; // RES 1, (IX + d), L | UNDOCUMENTED
                case 0x8F: RES_B_IRDMEM_R(1, displacement, indexAddressingMode, A); break; // RES 1, (IX + d), A | UNDOCUMENTED
                case 0x8E: RES_B_IRDMEM(1, displacement, indexAddressingMode); break;      // RES 1, (IX + d)

                case 0x90: RES_B_IRDMEM_R(2, displacement, indexAddressingMode, B); break; // RES 2, (IX + d), B | UNDOCUMENTED
                case 0x91: RES_B_IRDMEM_R(2, displacement, indexAddressingMode, C); break; // RES 2, (IX + d), C | UNDOCUMENTED
                case 0x92: RES_B_IRDMEM_R(2, displacement, indexAddressingMode, D); break; // RES 2, (IX + d), D | UNDOCUMENTED
                case 0x93: RES_B_IRDMEM_R(2, displacement, indexAddressingMode, E); break; // RES 2, (IX + d), E | UNDOCUMENTED
                case 0x94: RES_B_IRDMEM_R(2, displacement, indexAddressingMode, H); break; // RES 2, (IX + d), H | UNDOCUMENTED
                case 0x95: RES_B_IRDMEM_R(2, displacement, indexAddressingMode, L); break; // RES 2, (IX + d), L | UNDOCUMENTED
                case 0x97: RES_B_IRDMEM_R(2, displacement, indexAddressingMode, A); break; // RES 2, (IX + d), A | UNDOCUMENTED
                case 0x96: RES_B_IRDMEM(2, displacement, indexAddressingMode); break;      // RES 2, (IX + d)

                case 0x98: RES_B_IRDMEM_R(3, displacement, indexAddressingMode, B); break; // RES 3, (IX + d), B | UNDOCUMENTED
                case 0x99: RES_B_IRDMEM_R(3, displacement, indexAddressingMode, C); break; // RES 3, (IX + d), C | UNDOCUMENTED
                case 0x9A: RES_B_IRDMEM_R(3, displacement, indexAddressingMode, D); break; // RES 3, (IX + d), D | UNDOCUMENTED
                case 0x9B: RES_B_IRDMEM_R(3, displacement, indexAddressingMode, E); break; // RES 3, (IX + d), E | UNDOCUMENTED
                case 0x9C: RES_B_IRDMEM_R(3, displacement, indexAddressingMode, H); break; // RES 3, (IX + d), H | UNDOCUMENTED
                case 0x9D: RES_B_IRDMEM_R(3, displacement, indexAddressingMode, L); break; // RES 3, (IX + d), L | UNDOCUMENTED
                case 0x9F: RES_B_IRDMEM_R(3, displacement, indexAddressingMode, A); break; // RES 3, (IX + d), A | UNDOCUMENTED
                case 0x9E: RES_B_IRDMEM(3, displacement, indexAddressingMode); break;      // RES 3, (IX + d)

                case 0xA0: RES_B_IRDMEM_R(4, displacement, indexAddressingMode, B); break; // RES 4, (IX + d), B | UNDOCUMENTED
                case 0xA1: RES_B_IRDMEM_R(4, displacement, indexAddressingMode, C); break; // RES 4, (IX + d), C | UNDOCUMENTED
                case 0xA2: RES_B_IRDMEM_R(4, displacement, indexAddressingMode, D); break; // RES 4, (IX + d), D | UNDOCUMENTED
                case 0xA3: RES_B_IRDMEM_R(4, displacement, indexAddressingMode, E); break; // RES 4, (IX + d), E | UNDOCUMENTED
                case 0xA4: RES_B_IRDMEM_R(4, displacement, indexAddressingMode, H); break; // RES 4, (IX + d), H | UNDOCUMENTED
                case 0xA5: RES_B_IRDMEM_R(4, displacement, indexAddressingMode, L); break; // RES 4, (IX + d), L | UNDOCUMENTED
                case 0xA7: RES_B_IRDMEM_R(4, displacement, indexAddressingMode, A); break; // RES 4, (IX + d), A | UNDOCUMENTED
                case 0xA6: RES_B_IRDMEM(4, displacement, indexAddressingMode); break;      // RES 4, (IX + d)

                case 0xA8: RES_B_IRDMEM_R(5, displacement, indexAddressingMode, B); break; // RES 5, (IX + d), B | UNDOCUMENTED
                case 0xA9: RES_B_IRDMEM_R(5, displacement, indexAddressingMode, C); break; // RES 5, (IX + d), C | UNDOCUMENTED
                case 0xAA: RES_B_IRDMEM_R(5, displacement, indexAddressingMode, D); break; // RES 5, (IX + d), D | UNDOCUMENTED
                case 0xAB: RES_B_IRDMEM_R(5, displacement, indexAddressingMode, E); break; // RES 5, (IX + d), E | UNDOCUMENTED
                case 0xAC: RES_B_IRDMEM_R(5, displacement, indexAddressingMode, H); break; // RES 5, (IX + d), H | UNDOCUMENTED
                case 0xAD: RES_B_IRDMEM_R(5, displacement, indexAddressingMode, L); break; // RES 5, (IX + d), L | UNDOCUMENTED
                case 0xAF: RES_B_IRDMEM_R(5, displacement, indexAddressingMode, A); break; // RES 5, (IX + d), A | UNDOCUMENTED
                case 0xAE: RES_B_IRDMEM(5, displacement, indexAddressingMode); break;      // RES 5, (IX + d)

                case 0xB0: RES_B_IRDMEM_R(6, displacement, indexAddressingMode, B); break; // RES 6, (IX + d), B | UNDOCUMENTED
                case 0xB1: RES_B_IRDMEM_R(6, displacement, indexAddressingMode, C); break; // RES 6, (IX + d), C | UNDOCUMENTED
                case 0xB2: RES_B_IRDMEM_R(6, displacement, indexAddressingMode, D); break; // RES 6, (IX + d), D | UNDOCUMENTED
                case 0xB3: RES_B_IRDMEM_R(6, displacement, indexAddressingMode, E); break; // RES 6, (IX + d), E | UNDOCUMENTED
                case 0xB4: RES_B_IRDMEM_R(6, displacement, indexAddressingMode, H); break; // RES 6, (IX + d), H | UNDOCUMENTED
                case 0xB5: RES_B_IRDMEM_R(6, displacement, indexAddressingMode, L); break; // RES 6, (IX + d), L | UNDOCUMENTED
                case 0xB7: RES_B_IRDMEM_R(6, displacement, indexAddressingMode, A); break; // RES 6, (IX + d), A | UNDOCUMENTED
                case 0xB6: RES_B_IRDMEM(6, displacement, indexAddressingMode); break;      // RES 6, (IX + d)

                case 0xB8: RES_B_IRDMEM_R(7, displacement, indexAddressingMode, B); break; // RES 7, (IX + d), B | UNDOCUMENTED
                case 0xB9: RES_B_IRDMEM_R(7, displacement, indexAddressingMode, C); break; // RES 7, (IX + d), C | UNDOCUMENTED
                case 0xBA: RES_B_IRDMEM_R(7, displacement, indexAddressingMode, D); break; // RES 7, (IX + d), D | UNDOCUMENTED
                case 0xBB: RES_B_IRDMEM_R(7, displacement, indexAddressingMode, E); break; // RES 7, (IX + d), E | UNDOCUMENTED
                case 0xBC: RES_B_IRDMEM_R(7, displacement, indexAddressingMode, H); break; // RES 7, (IX + d), H | UNDOCUMENTED
                case 0xBD: RES_B_IRDMEM_R(7, displacement, indexAddressingMode, L); break; // RES 7, (IX + d), L | UNDOCUMENTED
                case 0xBF: RES_B_IRDMEM_R(7, displacement, indexAddressingMode, A); break; // RES 7, (IX + d), A | UNDOCUMENTED
                case 0xBE: RES_B_IRDMEM(7, displacement, indexAddressingMode); break;      // RES 7, (IX + d)


                // SET instructions: Set bit B of (IR + d)
                case 0xC0: SET_B_IRDMEM_R(0, displacement, indexAddressingMode, B); break; // SET 0, (IX + d), B | UNDOCUMENTED
                case 0xC1: SET_B_IRDMEM_R(0, displacement, indexAddressingMode, C); break; // SET 0, (IX + d), C | UNDOCUMENTED
                case 0xC2: SET_B_IRDMEM_R(0, displacement, indexAddressingMode, D); break; // SET 0, (IX + d), D | UNDOCUMENTED
                case 0xC3: SET_B_IRDMEM_R(0, displacement, indexAddressingMode, E); break; // SET 0, (IX + d), E | UNDOCUMENTED
                case 0xC4: SET_B_IRDMEM_R(0, displacement, indexAddressingMode, H); break; // SET 0, (IX + d), H | UNDOCUMENTED
                case 0xC5: SET_B_IRDMEM_R(0, displacement, indexAddressingMode, L); break; // SET 0, (IX + d), L | UNDOCUMENTED
                case 0xC7: SET_B_IRDMEM_R(0, displacement, indexAddressingMode, A); break; // SET 0, (IX + d), A | UNDOCUMENTED
                case 0xC6: SET_B_IRDMEM(0, displacement, indexAddressingMode); break;      // SET 0, (IX + d)

                case 0xC8: SET_B_IRDMEM_R(1, displacement, indexAddressingMode, B); break; // SET 1, (IX + d), B | UNDOCUMENTED
                case 0xC9: SET_B_IRDMEM_R(1, displacement, indexAddressingMode, C); break; // SET 1, (IX + d), C | UNDOCUMENTED
                case 0xCA: SET_B_IRDMEM_R(1, displacement, indexAddressingMode, D); break; // SET 1, (IX + d), D | UNDOCUMENTED
                case 0xCB: SET_B_IRDMEM_R(1, displacement, indexAddressingMode, E); break; // SET 1, (IX + d), E | UNDOCUMENTED
                case 0xCC: SET_B_IRDMEM_R(1, displacement, indexAddressingMode, H); break; // SET 1, (IX + d), H | UNDOCUMENTED
                case 0xCD: SET_B_IRDMEM_R(1, displacement, indexAddressingMode, L); break; // SET 1, (IX + d), L | UNDOCUMENTED
                case 0xCF: SET_B_IRDMEM_R(1, displacement, indexAddressingMode, A); break; // SET 1, (IX + d), A | UNDOCUMENTED
                case 0xCE: SET_B_IRDMEM(1, displacement, indexAddressingMode); break;      // SET 1, (IX + d)

                case 0xD0: SET_B_IRDMEM_R(2, displacement, indexAddressingMode, B); break; // SET 2, (IX + d), B | UNDOCUMENTED
                case 0xD1: SET_B_IRDMEM_R(2, displacement, indexAddressingMode, C); break; // SET 2, (IX + d), C | UNDOCUMENTED
                case 0xD2: SET_B_IRDMEM_R(2, displacement, indexAddressingMode, D); break; // SET 2, (IX + d), D | UNDOCUMENTED
                case 0xD3: SET_B_IRDMEM_R(2, displacement, indexAddressingMode, E); break; // SET 2, (IX + d), E | UNDOCUMENTED
                case 0xD4: SET_B_IRDMEM_R(2, displacement, indexAddressingMode, H); break; // SET 2, (IX + d), H | UNDOCUMENTED
                case 0xD5: SET_B_IRDMEM_R(2, displacement, indexAddressingMode, L); break; // SET 2, (IX + d), L | UNDOCUMENTED
                case 0xD7: SET_B_IRDMEM_R(2, displacement, indexAddressingMode, A); break; // SET 2, (IX + d), A | UNDOCUMENTED
                case 0xD6: SET_B_IRDMEM(2, displacement, indexAddressingMode); break;      // SET 2, (IX + d)

                case 0xD8: SET_B_IRDMEM_R(3, displacement, indexAddressingMode, B); break; // SET 3, (IX + d), B | UNDOCUMENTED
                case 0xD9: SET_B_IRDMEM_R(3, displacement, indexAddressingMode, C); break; // SET 3, (IX + d), C | UNDOCUMENTED
                case 0xDA: SET_B_IRDMEM_R(3, displacement, indexAddressingMode, D); break; // SET 3, (IX + d), D | UNDOCUMENTED
                case 0xDB: SET_B_IRDMEM_R(3, displacement, indexAddressingMode, E); break; // SET 3, (IX + d), E | UNDOCUMENTED
                case 0xDC: SET_B_IRDMEM_R(3, displacement, indexAddressingMode, H); break; // SET 3, (IX + d), H | UNDOCUMENTED
                case 0xDD: SET_B_IRDMEM_R(3, displacement, indexAddressingMode, L); break; // SET 3, (IX + d), L | UNDOCUMENTED
                case 0xDF: SET_B_IRDMEM_R(3, displacement, indexAddressingMode, A); break; // SET 3, (IX + d), A | UNDOCUMENTED
                case 0xDE: SET_B_IRDMEM(3, displacement, indexAddressingMode); break;      // SET 3, (IX + d)

                case 0xE0: SET_B_IRDMEM_R(4, displacement, indexAddressingMode, B); break; // SET 4, (IX + d), B | UNDOCUMENTED
                case 0xE1: SET_B_IRDMEM_R(4, displacement, indexAddressingMode, C); break; // SET 4, (IX + d), C | UNDOCUMENTED
                case 0xE2: SET_B_IRDMEM_R(4, displacement, indexAddressingMode, D); break; // SET 4, (IX + d), D | UNDOCUMENTED
                case 0xE3: SET_B_IRDMEM_R(4, displacement, indexAddressingMode, E); break; // SET 4, (IX + d), E | UNDOCUMENTED
                case 0xE4: SET_B_IRDMEM_R(4, displacement, indexAddressingMode, H); break; // SET 4, (IX + d), H | UNDOCUMENTED
                case 0xE5: SET_B_IRDMEM_R(4, displacement, indexAddressingMode, L); break; // SET 4, (IX + d), L | UNDOCUMENTED
                case 0xE7: SET_B_IRDMEM_R(4, displacement, indexAddressingMode, A); break; // SET 4, (IX + d), A | UNDOCUMENTED
                case 0xE6: SET_B_IRDMEM(4, displacement, indexAddressingMode); break;      // SET 4, (IX + d)

                case 0xE8: SET_B_IRDMEM_R(5, displacement, indexAddressingMode, B); break; // SET 5, (IX + d), B | UNDOCUMENTED
                case 0xE9: SET_B_IRDMEM_R(5, displacement, indexAddressingMode, C); break; // SET 5, (IX + d), C | UNDOCUMENTED
                case 0xEA: SET_B_IRDMEM_R(5, displacement, indexAddressingMode, D); break; // SET 5, (IX + d), D | UNDOCUMENTED
                case 0xEB: SET_B_IRDMEM_R(5, displacement, indexAddressingMode, E); break; // SET 5, (IX + d), E | UNDOCUMENTED
                case 0xEC: SET_B_IRDMEM_R(5, displacement, indexAddressingMode, H); break; // SET 5, (IX + d), H | UNDOCUMENTED
                case 0xED: SET_B_IRDMEM_R(5, displacement, indexAddressingMode, L); break; // SET 5, (IX + d), L | UNDOCUMENTED
                case 0xEF: SET_B_IRDMEM_R(5, displacement, indexAddressingMode, A); break; // SET 5, (IX + d), A | UNDOCUMENTED
                case 0xEE: SET_B_IRDMEM(5, displacement, indexAddressingMode); break;      // SET 5, (IX + d)

                case 0xF0: SET_B_IRDMEM_R(6, displacement, indexAddressingMode, B); break; // SET 6, (IX + d), B | UNDOCUMENTED
                case 0xF1: SET_B_IRDMEM_R(6, displacement, indexAddressingMode, C); break; // SET 6, (IX + d), C | UNDOCUMENTED
                case 0xF2: SET_B_IRDMEM_R(6, displacement, indexAddressingMode, D); break; // SET 6, (IX + d), D | UNDOCUMENTED
                case 0xF3: SET_B_IRDMEM_R(6, displacement, indexAddressingMode, E); break; // SET 6, (IX + d), E | UNDOCUMENTED
                case 0xF4: SET_B_IRDMEM_R(6, displacement, indexAddressingMode, H); break; // SET 6, (IX + d), H | UNDOCUMENTED
                case 0xF5: SET_B_IRDMEM_R(6, displacement, indexAddressingMode, L); break; // SET 6, (IX + d), L | UNDOCUMENTED
                case 0xF7: SET_B_IRDMEM_R(6, displacement, indexAddressingMode, A); break; // SET 6, (IX + d), A | UNDOCUMENTED
                case 0xF6: SET_B_IRDMEM(6, displacement, indexAddressingMode); break;      // SET 6, (IX + d)

                case 0xF8: SET_B_IRDMEM_R(7, displacement, indexAddressingMode, B); break; // SET 7, (IX + d), B | UNDOCUMENTED
                case 0xF9: SET_B_IRDMEM_R(7, displacement, indexAddressingMode, C); break; // SET 7, (IX + d), C | UNDOCUMENTED
                case 0xFA: SET_B_IRDMEM_R(7, displacement, indexAddressingMode, D); break; // SET 7, (IX + d), D | UNDOCUMENTED
                case 0xFB: SET_B_IRDMEM_R(7, displacement, indexAddressingMode, E); break; // SET 7, (IX + d), E | UNDOCUMENTED
                case 0xFC: SET_B_IRDMEM_R(7, displacement, indexAddressingMode, H); break; // SET 7, (IX + d), H | UNDOCUMENTED
                case 0xFD: SET_B_IRDMEM_R(7, displacement, indexAddressingMode, L); break; // SET 7, (IX + d), L | UNDOCUMENTED
                case 0xFF: SET_B_IRDMEM_R(7, displacement, indexAddressingMode, A); break; // SET 7, (IX + d), A | UNDOCUMENTED
                case 0xFE: SET_B_IRDMEM(7, displacement, indexAddressingMode); break;      // SET 7, (IX + d)
            }
        }
    }
}
