namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void ExecuteBitInstruction()
        {
           _clock.Add(8);
            Registers.IncrementRefresh();

            byte instruction = Fetch();
            _currentInstruction = instruction;
            _pendingInstruction.Opcode2 = _currentInstruction;

            switch (instruction)
            {
                // R(L/R)(C) instructions: Rotate register/memory value through/from carry flag
                case 0x00: RLC_R(ref Registers.B); break;                     // RLC B
                case 0x01: RLC_R(ref Registers.C); break;                     // RLC C
                case 0x02: RLC_R(ref Registers.D); break;                     // RLC D
                case 0x03: RLC_R(ref Registers.E); break;                     // RLC E
                case 0x04: RLC_R(ref Registers.H); break;                     // RLC H
                case 0x05: RLC_R(ref Registers.L); break;                     // RLC L
                case 0x06: RLC_RRMEM(ref Registers.HL); _clock.Add(7); break; // RLC (HL)
                case 0x07: RLC_R(ref Registers.A); break;                     // RLC A

                case 0x10: RL_R(ref Registers.B); break;                     // RL B
                case 0x11: RL_R(ref Registers.C); break;                     // RL C
                case 0x12: RL_R(ref Registers.D); break;                     // RL D
                case 0x13: RL_R(ref Registers.E); break;                     // RL E
                case 0x14: RL_R(ref Registers.H); break;                     // RL H
                case 0x15: RL_R(ref Registers.L); break;                     // RL L
                case 0x16: RL_RRMEM(ref Registers.HL); _clock.Add(7); break; // RL (HL)
                case 0x17: RL_R(ref Registers.A); break;                     // RL A

                case 0x08: RRC_R(ref Registers.B); break;                     // RRC B
                case 0x09: RRC_R(ref Registers.C); break;                     // RRC C
                case 0x0A: RRC_R(ref Registers.D); break;                     // RRC D
                case 0x0B: RRC_R(ref Registers.E); break;                     // RRC E
                case 0x0C: RRC_R(ref Registers.H); break;                     // RRC H
                case 0x0D: RRC_R(ref Registers.L); break;                     // RRC L
                case 0x0E: RRC_RRMEM(ref Registers.HL); _clock.Add(7); break; // RRC (HL)
                case 0x0F: RRC_R(ref Registers.A); break;                     // RRC A

                case 0x18: RR_R(ref Registers.B); break;                     // RR B
                case 0x19: RR_R(ref Registers.C); break;                     // RR C
                case 0x1A: RR_R(ref Registers.D); break;                     // RR D
                case 0x1B: RR_R(ref Registers.E); break;                     // RR E
                case 0x1C: RR_R(ref Registers.H); break;                     // RR H
                case 0x1D: RR_R(ref Registers.L); break;                     // RR L
                case 0x1E: RR_RRMEM(ref Registers.HL); _clock.Add(7); break; // RR (HL)
                case 0x1F: RR_R(ref Registers.A); break;                     // RR A


                // S(L/R)(L/A) instructions: Logical shift register/memory value through/from carry flag
                case 0x20: SLA_R(ref Registers.B); break;                     // SLA B
                case 0x21: SLA_R(ref Registers.C); break;                     // SLA C
                case 0x22: SLA_R(ref Registers.D); break;                     // SLA D
                case 0x23: SLA_R(ref Registers.E); break;                     // SLA E
                case 0x24: SLA_R(ref Registers.H); break;                     // SLA H
                case 0x25: SLA_R(ref Registers.L); break;                     // SLA L
                case 0x26: SLA_RRMEM(ref Registers.HL); _clock.Add(7); break; // SLA (HL)
                case 0x27: SLA_R(ref Registers.A); break;                     // SLA A

                case 0x30: SLL_R(ref Registers.B); break;                     // SLL B | UNDOCUMENTED
                case 0x31: SLL_R(ref Registers.C); break;                     // SLL C | UNDOCUMENTED
                case 0x32: SLL_R(ref Registers.D); break;                     // SLL D | UNDOCUMENTED
                case 0x33: SLL_R(ref Registers.E); break;                     // SLL E | UNDOCUMENTED
                case 0x34: SLL_R(ref Registers.H); break;                     // SLL H | UNDOCUMENTED
                case 0x35: SLL_R(ref Registers.L); break;                     // SLL L | UNDOCUMENTED
                case 0x36: SLL_RRMEM(ref Registers.HL); _clock.Add(7); break; // SLL (HL) | UNDOCUMENTED
                case 0x37: SLL_R(ref Registers.A); break;                     // SLL A | UNDOCUMENTED

                case 0x28: SRA_R(ref Registers.B); break;                     // SRA B
                case 0x29: SRA_R(ref Registers.C); break;                     // SRA C
                case 0x2A: SRA_R(ref Registers.D); break;                     // SRA D
                case 0x2B: SRA_R(ref Registers.E); break;                     // SRA E
                case 0x2C: SRA_R(ref Registers.H); break;                     // SRA H
                case 0x2D: SRA_R(ref Registers.L); break;                     // SRA L
                case 0x2E: SRA_RRMEM(ref Registers.HL); _clock.Add(7); break; // SRA (HL)
                case 0x2F: SRA_R(ref Registers.A); break;                     // SRA A

                case 0x38: SRL_R(ref Registers.B); break;                     // SRL B
                case 0x39: SRL_R(ref Registers.C); break;                     // SRL C
                case 0x3A: SRL_R(ref Registers.D); break;                     // SRL D
                case 0x3B: SRL_R(ref Registers.E); break;                     // SRL E
                case 0x3C: SRL_R(ref Registers.H); break;                     // SRL H
                case 0x3D: SRL_R(ref Registers.L); break;                     // SRL L
                case 0x3E: SRL_RRMEM(ref Registers.HL); _clock.Add(7); break; // SRL (HL)
                case 0x3F: SRL_R(ref Registers.A); break;                     // SRL A

                // BIT instructions: Tests bit B of register R, sets Z if tested bit is zero
                case 0x40: BIT_B_R(0, ref Registers.B); break;                     // BIT 0, B
                case 0x41: BIT_B_R(0, ref Registers.C); break;                     // BIT 0, C
                case 0x42: BIT_B_R(0, ref Registers.D); break;                     // BIT 0, D
                case 0x43: BIT_B_R(0, ref Registers.E); break;                     // BIT 0, E
                case 0x44: BIT_B_R(0, ref Registers.H); break;                     // BIT 0, H
                case 0x45: BIT_B_R(0, ref Registers.L); break;                     // BIT 0, L
                case 0x46: BIT_B_RRMEM(0, ref Registers.HL); _clock.Add(4); break; // BIT 0, (HL)
                case 0x47: BIT_B_R(0, ref Registers.A); break;                     // BIT 0, A

                case 0x48: BIT_B_R(1, ref Registers.B); break;                     // BIT 1, B
                case 0x49: BIT_B_R(1, ref Registers.C); break;                     // BIT 1, C
                case 0x4A: BIT_B_R(1, ref Registers.D); break;                     // BIT 1, D
                case 0x4B: BIT_B_R(1, ref Registers.E); break;                     // BIT 1, E
                case 0x4C: BIT_B_R(1, ref Registers.H); break;                     // BIT 1, H
                case 0x4D: BIT_B_R(1, ref Registers.L); break;                     // BIT 1, L
                case 0x4E: BIT_B_RRMEM(1, ref Registers.HL); _clock.Add(4); break; // BIT 1, (HL)
                case 0x4F: BIT_B_R(1, ref Registers.A); break;                     // BIT 1, A

                case 0x50: BIT_B_R(2, ref Registers.B); break;                     // BIT 2, B
                case 0x51: BIT_B_R(2, ref Registers.C); break;                     // BIT 2, C
                case 0x52: BIT_B_R(2, ref Registers.D); break;                     // BIT 2, D
                case 0x53: BIT_B_R(2, ref Registers.E); break;                     // BIT 2, E
                case 0x54: BIT_B_R(2, ref Registers.H);  break;                     // BIT 2, H
                case 0x55: BIT_B_R(2, ref Registers.L); break;                     // BIT 2, L
                case 0x56: BIT_B_RRMEM(2, ref Registers.HL); _clock.Add(4); break; // BIT 2, (HL)
                case 0x57: BIT_B_R(2, ref Registers.A); break;                     // BIT 2, A

                case 0x58: BIT_B_R(3, ref Registers.B); break;                     // BIT 3, B
                case 0x59: BIT_B_R(3, ref Registers.C); break;                     // BIT 3, C
                case 0x5A: BIT_B_R(3, ref Registers.D); break;                     // BIT 3, D
                case 0x5B: BIT_B_R(3, ref Registers.E); break;                     // BIT 3, E
                case 0x5C: BIT_B_R(3, ref Registers.H); break;                     // BIT 3, H
                case 0x5D: BIT_B_R(3, ref Registers.L); break;                     // BIT 3, L
                case 0x5E: BIT_B_RRMEM(3, ref Registers.HL); _clock.Add(4); break; // BIT 3, (HL)
                case 0x5F: BIT_B_R(3, ref Registers.A); break;                     // BIT 3, A

                case 0x60: BIT_B_R(4, ref Registers.B); break;                     // BIT 4, B
                case 0x61: BIT_B_R(4, ref Registers.C); break;                     // BIT 4, C
                case 0x62: BIT_B_R(4, ref Registers.D); break;                     // BIT 4, D
                case 0x63: BIT_B_R(4, ref Registers.E); break;                     // BIT 4, E
                case 0x64: BIT_B_R(4, ref Registers.H); break;                     // BIT 4, H
                case 0x65: BIT_B_R(4, ref Registers.L); break;                     // BIT 4, L
                case 0x66: BIT_B_RRMEM(4, ref Registers.HL); _clock.Add(4); break; // BIT 4, (HL)
                case 0x67: BIT_B_R(4, ref Registers.A); break;                     // BIT 4, A

                case 0x68: BIT_B_R(5, ref Registers.B); break;                     // BIT 5, B
                case 0x69: BIT_B_R(5, ref Registers.C); break;                     // BIT 5, C
                case 0x6A: BIT_B_R(5, ref Registers.D); break;                     // BIT 5, D
                case 0x6B: BIT_B_R(5, ref Registers.E); break;                     // BIT 5, E
                case 0x6C: BIT_B_R(5, ref Registers.H); break;                     // BIT 5, H
                case 0x6D: BIT_B_R(5, ref Registers.L); break;                     // BIT 5, L
                case 0x6E: BIT_B_RRMEM(5, ref Registers.HL); _clock.Add(4); break; // BIT 5, (HL)
                case 0x6F: BIT_B_R(5, ref Registers.A); break;                     // BIT 5, A

                case 0x70: BIT_B_R(6, ref Registers.B); break;                     // BIT 6, B
                case 0x71: BIT_B_R(6, ref Registers.C); break;                     // BIT 6, C
                case 0x72: BIT_B_R(6, ref Registers.D); break;                     // BIT 6, D
                case 0x73: BIT_B_R(6, ref Registers.E); break;                     // BIT 6, E
                case 0x74: BIT_B_R(6, ref Registers.H); break;                     // BIT 6, H
                case 0x75: BIT_B_R(6, ref Registers.L); break;                     // BIT 6, L
                case 0x76: BIT_B_RRMEM(6, ref Registers.HL); _clock.Add(4); break; // BIT 6, (HL)
                case 0x77: BIT_B_R(6, ref Registers.A); break;                     // BIT 6, A

                case 0x78: BIT_B_R(7, ref Registers.B); break;                     // BIT 7, B
                case 0x79: BIT_B_R(7, ref Registers.C); break;                     // BIT 7, C
                case 0x7A: BIT_B_R(7, ref Registers.D); break;                     // BIT 7, D
                case 0x7B: BIT_B_R(7, ref Registers.E); break;                     // BIT 7, E
                case 0x7C: BIT_B_R(7, ref Registers.H); break;                     // BIT 7, H
                case 0x7D: BIT_B_R(7, ref Registers.L); break;                     // BIT 7, L
                case 0x7E: BIT_B_RRMEM(7, ref Registers.HL); _clock.Add(4); break; // BIT 7, (HL)
                case 0x7F: BIT_B_R(7, ref Registers.A); break;                     // BIT 7, A


                // RES instructions: Reset bit B of R or (RR)
                case 0x80: RES_B_R(0, ref Registers.B); break;                     // RES 0, B
                case 0x81: RES_B_R(0, ref Registers.C); break;                     // RES 0, C
                case 0x82: RES_B_R(0, ref Registers.D); break;                     // RES 0, D
                case 0x83: RES_B_R(0, ref Registers.E); break;                     // RES 0, E
                case 0x84: RES_B_R(0, ref Registers.H); break;                     // RES 0, H
                case 0x85: RES_B_R(0, ref Registers.L); break;                     // RES 0, L
                case 0x86: RES_B_RRMEM(0, ref Registers.HL); _clock.Add(7); break; // RES 0, (HL)
                case 0x87: RES_B_R(0, ref Registers.A); break;                     // RES 0, A

                case 0x88: RES_B_R(1, ref Registers.B); break;                     // RES 1, B
                case 0x89: RES_B_R(1, ref Registers.C); break;                     // RES 1, C
                case 0x8A: RES_B_R(1, ref Registers.D); break;                     // RES 1, D
                case 0x8B: RES_B_R(1, ref Registers.E); break;                     // RES 1, E
                case 0x8C: RES_B_R(1, ref Registers.H); break;                     // RES 1, H
                case 0x8D: RES_B_R(1, ref Registers.L); break;                     // RES 1, L
                case 0x8E: RES_B_RRMEM(1, ref Registers.HL); _clock.Add(7); break; // RES 1, (HL)
                case 0x8F: RES_B_R(1, ref Registers.A); break;                     // RES 1, A

                case 0x90: RES_B_R(2, ref Registers.B); break;                     // RES 2, B
                case 0x91: RES_B_R(2, ref Registers.C); break;                     // RES 2, C
                case 0x92: RES_B_R(2, ref Registers.D); break;                     // RES 2, D
                case 0x93: RES_B_R(2, ref Registers.E); break;                     // RES 2, E
                case 0x94: RES_B_R(2, ref Registers.H); break;                     // RES 2, H
                case 0x95: RES_B_R(2, ref Registers.L); break;                     // RES 2, L
                case 0x96: RES_B_RRMEM(2, ref Registers.HL); _clock.Add(7); break; // RES 2, (HL)
                case 0x97: RES_B_R(2, ref Registers.A); break;                     // RES 2, A

                case 0x98: RES_B_R(3, ref Registers.B); break;                     // RES 3, B
                case 0x99: RES_B_R(3, ref Registers.C); break;                     // RES 3, C
                case 0x9A: RES_B_R(3, ref Registers.D); break;                     // RES 3, D
                case 0x9B: RES_B_R(3, ref Registers.E); break;                     // RES 3, E
                case 0x9C: RES_B_R(3, ref Registers.H); break;                     // RES 3, H
                case 0x9D: RES_B_R(3, ref Registers.L); break;                     // RES 3, L
                case 0x9E: RES_B_RRMEM(3, ref Registers.HL); _clock.Add(7); break; // RES 3, (HL)
                case 0x9F: RES_B_R(3, ref Registers.A); break;                     // RES 3, A

                case 0xA0: RES_B_R(4, ref Registers.B); break;                     // RES 4, B
                case 0xA1: RES_B_R(4, ref Registers.C); break;                     // RES 4, C
                case 0xA2: RES_B_R(4, ref Registers.D); break;                     // RES 4, D
                case 0xA3: RES_B_R(4, ref Registers.E); break;                     // RES 4, E
                case 0xA4: RES_B_R(4, ref Registers.H); break;                     // RES 4, H
                case 0xA5: RES_B_R(4, ref Registers.L); break;                     // RES 4, L
                case 0xA6: RES_B_RRMEM(4, ref Registers.HL); _clock.Add(7); break; // RES 4, (HL)
                case 0xA7: RES_B_R(4, ref Registers.A); break;                     // RES 4, A

                case 0xA8: RES_B_R(5, ref Registers.B); break;                     // RES 5, B
                case 0xA9: RES_B_R(5, ref Registers.C); break;                     // RES 5, C
                case 0xAA: RES_B_R(5, ref Registers.D); break;                     // RES 5, D
                case 0xAB: RES_B_R(5, ref Registers.E); break;                     // RES 5, E
                case 0xAC: RES_B_R(5, ref Registers.H); break;                     // RES 5, H
                case 0xAD: RES_B_R(5, ref Registers.L); break;                     // RES 5, L
                case 0xAE: RES_B_RRMEM(5, ref Registers.HL); _clock.Add(7); break; // RES 5, (HL)
                case 0xAF: RES_B_R(5, ref Registers.A); break;                     // RES 5, A

                case 0xB0: RES_B_R(6, ref Registers.B); break;                     // RES 6, B
                case 0xB1: RES_B_R(6, ref Registers.C); break;                     // RES 6, C
                case 0xB2: RES_B_R(6, ref Registers.D); break;                     // RES 6, D
                case 0xB3: RES_B_R(6, ref Registers.E); break;                     // RES 6, E
                case 0xB4: RES_B_R(6, ref Registers.H); break;                     // RES 6, H
                case 0xB5: RES_B_R(6, ref Registers.L); break;                     // RES 6, L
                case 0xB6: RES_B_RRMEM(6, ref Registers.HL); _clock.Add(7); break; // RES 6, (HL)
                case 0xB7: RES_B_R(6, ref Registers.A); break;                     // RES 6, A

                case 0xB8: RES_B_R(7, ref Registers.B); break;                     // RES 7, B
                case 0xB9: RES_B_R(7, ref Registers.C); break;                     // RES 7, C
                case 0xBA: RES_B_R(7, ref Registers.D); break;                     // RES 7, D
                case 0xBB: RES_B_R(7, ref Registers.E); break;                     // RES 7, E
                case 0xBC: RES_B_R(7, ref Registers.H); break;                     // RES 7, H
                case 0xBD: RES_B_R(7, ref Registers.L); break;                     // RES 7, L
                case 0xBE: RES_B_RRMEM(7, ref Registers.HL); _clock.Add(7); break; // RES 7, (HL)
                case 0xBF: RES_B_R(7, ref Registers.A); break;                     // RES 7, A


                // SET instructions: Set bit B of R or (RR)
                case 0xC0: SET_B_R(0, ref Registers.B); break;                     // SET 0, B
                case 0xC1: SET_B_R(0, ref Registers.C); break;                     // SET 0, C
                case 0xC2: SET_B_R(0, ref Registers.D); break;                     // SET 0, D
                case 0xC3: SET_B_R(0, ref Registers.E); break;                     // SET 0, E
                case 0xC4: SET_B_R(0, ref Registers.H); break;                     // SET 0, H
                case 0xC5: SET_B_R(0, ref Registers.L); break;                     // SET 0, L
                case 0xC6: SET_B_RRMEM(0, ref Registers.HL); _clock.Add(7); break; // SET 0, (HL)
                case 0xC7: SET_B_R(0, ref Registers.A); break;                     // SET 0, A

                case 0xC8: SET_B_R(1, ref Registers.B); break;                     // SET 1, B
                case 0xC9: SET_B_R(1, ref Registers.C); break;                     // SET 1, C
                case 0xCA: SET_B_R(1, ref Registers.D); break;                     // SET 1, D
                case 0xCB: SET_B_R(1, ref Registers.E); break;                     // SET 1, E
                case 0xCC: SET_B_R(1, ref Registers.H); break;                     // SET 1, H
                case 0xCD: SET_B_R(1, ref Registers.L); break;                     // SET 1, L
                case 0xCE: SET_B_RRMEM(1, ref Registers.HL); _clock.Add(7); break; // SET 1, (HL)
                case 0xCF: SET_B_R(1, ref Registers.A); break;                     // SET 1, A

                case 0xD0: SET_B_R(2, ref Registers.B); break;                     // SET 2, B
                case 0xD1: SET_B_R(2, ref Registers.C); break;                     // SET 2, C
                case 0xD2: SET_B_R(2, ref Registers.D); break;                     // SET 2, D
                case 0xD3: SET_B_R(2, ref Registers.E); break;                     // SET 2, E
                case 0xD4: SET_B_R(2, ref Registers.H); break;                     // SET 2, H
                case 0xD5: SET_B_R(2, ref Registers.L); break;                     // SET 2, L
                case 0xD6: SET_B_RRMEM(2, ref Registers.HL); _clock.Add(7); break; // SET 2, (HL)
                case 0xD7: SET_B_R(2, ref Registers.A); break;                     // SET 2, A

                case 0xD8: SET_B_R(3, ref Registers.B); break;                     // SET 3, B
                case 0xD9: SET_B_R(3, ref Registers.C); break;                     // SET 3, C
                case 0xDA: SET_B_R(3, ref Registers.D); break;                     // SET 3, D
                case 0xDB: SET_B_R(3, ref Registers.E); break;                     // SET 3, E
                case 0xDC: SET_B_R(3, ref Registers.H); break;                     // SET 3, H
                case 0xDD: SET_B_R(3, ref Registers.L); break;                     // SET 3, L
                case 0xDE: SET_B_RRMEM(3, ref Registers.HL); _clock.Add(7); break; // SET 3, (HL)
                case 0xDF: SET_B_R(3, ref Registers.A); break;                     // SET 3, A

                case 0xE0: SET_B_R(4, ref Registers.B); break;                     // SET 4, B
                case 0xE1: SET_B_R(4, ref Registers.C); break;                     // SET 4, C
                case 0xE2: SET_B_R(4, ref Registers.D); break;                     // SET 4, D
                case 0xE3: SET_B_R(4, ref Registers.E); break;                     // SET 4, E
                case 0xE4: SET_B_R(4, ref Registers.H); break;                     // SET 4, H
                case 0xE5: SET_B_R(4, ref Registers.L); break;                     // SET 4, L
                case 0xE6: SET_B_RRMEM(4, ref Registers.HL); _clock.Add(7); break; // SET 4, (HL)
                case 0xE7: SET_B_R(4, ref Registers.A); break;                     // SET 4, A

                case 0xE8: SET_B_R(5, ref Registers.B); break;                     // SET 5, B
                case 0xE9: SET_B_R(5, ref Registers.C); break;                     // SET 5, C
                case 0xEA: SET_B_R(5, ref Registers.D); break;                     // SET 5, D
                case 0xEB: SET_B_R(5, ref Registers.E); break;                     // SET 5, E
                case 0xEC: SET_B_R(5, ref Registers.H); break;                     // SET 5, H
                case 0xED: SET_B_R(5, ref Registers.L); break;                     // SET 5, L
                case 0xEE: SET_B_RRMEM(5, ref Registers.HL); _clock.Add(7); break; // SET 5, (HL)
                case 0xEF: SET_B_R(5, ref Registers.A); break;                     // SET 5, A

                case 0xF0: SET_B_R(6, ref Registers.B); break;                     // SET 6, B
                case 0xF1: SET_B_R(6, ref Registers.C); break;                     // SET 6, C
                case 0xF2: SET_B_R(6, ref Registers.D); break;                     // SET 6, D
                case 0xF3: SET_B_R(6, ref Registers.E); break;                     // SET 6, E
                case 0xF4: SET_B_R(6, ref Registers.H); break;                     // SET 6, H
                case 0xF5: SET_B_R(6, ref Registers.L); break;                     // SET 6, L
                case 0xF6: SET_B_RRMEM(6, ref Registers.HL); _clock.Add(7); break; // SET 6, (HL)
                case 0xF7: SET_B_R(6, ref Registers.A); break;                     // SET 6, A

                case 0xF8: SET_B_R(7, ref Registers.B); break;                     // SET 7, B
                case 0xF9: SET_B_R(7, ref Registers.C); break;                     // SET 7, C
                case 0xFA: SET_B_R(7, ref Registers.D); break;                     // SET 7, D
                case 0xFB: SET_B_R(7, ref Registers.E); break;                     // SET 7, E
                case 0xFC: SET_B_R(7, ref Registers.H); break;                     // SET 7, H
                case 0xFD: SET_B_R(7, ref Registers.L); break;                     // SET 7, L
                case 0xFE: SET_B_RRMEM(7, ref Registers.HL); _clock.Add(7); break; // SET 7, (HL)
                case 0xFF: SET_B_R(7, ref Registers.A); break;                     // SET 7, A
            }
        }
    }
}