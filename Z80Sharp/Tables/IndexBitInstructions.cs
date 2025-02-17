namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void ExecuteIndexRBitInstruction(ref ushort indexAddressingMode)
        {
            _clock.Add(20);

            sbyte displacement = (sbyte)Fetch(); // Displacement comes before opcode in index bit
            byte instruction = Fetch();          // So we fetch actual opcode after displacement
            _currentInstruction = instruction;
            switch (instruction)
            {
                // RL(C) instructions: Rotate (IR + d) left circular/through carry
                case 0x00: RLC_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.B);  _clock.Add(3); break; // RLC (IR + d), B | UNDOCUMENTED
                case 0x01: RLC_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.C);  _clock.Add(3); break; // RLC (IR + d), C | UNDOCUMENTED
                case 0x02: RLC_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.D);  _clock.Add(3); break; // RLC (IR + d), D | UNDOCUMENTED
                case 0x03: RLC_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.E);  _clock.Add(3); break; // RLC (IR + d), E | UNDOCUMENTED
                case 0x04: RLC_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.H);  _clock.Add(3); break; // RLC (IR + d), H | UNDOCUMENTED
                case 0x05: RLC_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.L);  _clock.Add(3); break; // RLC (IR + d), L | UNDOCUMENTED
                case 0x07: RLC_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.A);  _clock.Add(3); break; // RLC (IR + d), A | UNDOCUMENTED
                case 0x06: RLC_IRDMEM(displacement, ref indexAddressingMode);  _clock.Add(3); break;                    // RLC (IR + d)

                case 0x10: RL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.B);  _clock.Add(3); break; // RL (IR + d), B | UNDOCUMENTED
                case 0x11: RL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.C);  _clock.Add(3); break; // RL (IR + d), C | UNDOCUMENTED
                case 0x12: RL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.D);  _clock.Add(3); break; // RL (IR + d), D | UNDOCUMENTED
                case 0x13: RL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.E);  _clock.Add(3); break; // RL (IR + d), E | UNDOCUMENTED
                case 0x14: RL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.H);  _clock.Add(3); break; // RL (IR + d), H | UNDOCUMENTED
                case 0x15: RL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.L);  _clock.Add(3); break; // RL (IR + d), L | UNDOCUMENTED
                case 0x17: RL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.A);  _clock.Add(3); break; // RL (IR + d), A | UNDOCUMENTED
                case 0x16: RL_IRDMEM(displacement, ref indexAddressingMode);  _clock.Add(3); break;                    // RL (IR + d)


                // RR(C) instructions: Rotate (IR + d) right circular/through carry
                case 0x08: RRC_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.B);  _clock.Add(3); break; // RRC (IR + d), B | UNDOCUMENTED
                case 0x09: RRC_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.C);  _clock.Add(3); break; // RRC (IR + d), C | UNDOCUMENTED
                case 0x0A: RRC_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.D);  _clock.Add(3); break; // RRC (IR + d), D | UNDOCUMENTED
                case 0x0B: RRC_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.E);  _clock.Add(3); break; // RRC (IR + d), E | UNDOCUMENTED
                case 0x0C: RRC_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.H);  _clock.Add(3); break; // RRC (IR + d), H | UNDOCUMENTED
                case 0x0D: RRC_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.L);  _clock.Add(3); break; // RRC (IR + d), L | UNDOCUMENTED
                case 0x0F: RRC_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.A);  _clock.Add(3); break; // RRC (IR + d), A | UNDOCUMENTED
                case 0x0E: RRC_IRDMEM(displacement, ref indexAddressingMode);  _clock.Add(3); break;                    // RRC (IR + d)

                case 0x18: RR_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.B);  _clock.Add(3); break; // RR (IR + d), B | UNDOCUMENTED
                case 0x19: RR_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.C);  _clock.Add(3); break; // RR (IR + d), C | UNDOCUMENTED
                case 0x1A: RR_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.D);  _clock.Add(3); break; // RR (IR + d), D | UNDOCUMENTED
                case 0x1B: RR_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.E);  _clock.Add(3); break; // RR (IR + d), E | UNDOCUMENTED
                case 0x1C: RR_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.H);  _clock.Add(3); break; // RR (IR + d), H | UNDOCUMENTED
                case 0x1D: RR_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.L);  _clock.Add(3); break; // RR (IR + d), L | UNDOCUMENTED
                case 0x1F: RR_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.A);  _clock.Add(3); break; // RR (IR + d), A | UNDOCUMENTED
                case 0x1E: RR_IRDMEM(displacement, ref indexAddressingMode);  _clock.Add(3); break;                    // RR (IR + d)


                // SL(A/L) instructions: Shift (IR + d) left arithmetic/logical
                case 0x20: SLA_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.B);  _clock.Add(3); break; // SLA (IR + d), B | UNDOCUMENTED
                case 0x21: SLA_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.C);  _clock.Add(3); break; // SLA (IR + d), C | UNDOCUMENTED
                case 0x22: SLA_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.D);  _clock.Add(3); break; // SLA (IR + d), D | UNDOCUMENTED
                case 0x23: SLA_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.E);  _clock.Add(3); break; // SLA (IR + d), E | UNDOCUMENTED
                case 0x24: SLA_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.H);  _clock.Add(3); break; // SLA (IR + d), H | UNDOCUMENTED
                case 0x25: SLA_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.L);  _clock.Add(3); break; // SLA (IR + d), L | UNDOCUMENTED
                case 0x27: SLA_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.A);  _clock.Add(3); break; // SLA (IR + d), A | UNDOCUMENTED
                case 0x26: SLA_IRDMEM(displacement, ref indexAddressingMode);  _clock.Add(3); break;                    // SLA (IR + d)

                case 0x30: SLL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.B);  _clock.Add(3); break; // SLL (IR + d), B | UNDOCUMENTED
                case 0x31: SLL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.C);  _clock.Add(3); break; // SLL (IR + d), C | UNDOCUMENTED
                case 0x32: SLL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.D);  _clock.Add(3); break; // SLL (IR + d), D | UNDOCUMENTED
                case 0x33: SLL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.E);  _clock.Add(3); break; // SLL (IR + d), E | UNDOCUMENTED
                case 0x34: SLL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.H);  _clock.Add(3); break; // SLL (IR + d), H | UNDOCUMENTED
                case 0x35: SLL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.L);  _clock.Add(3); break; // SLL (IR + d), L | UNDOCUMENTED
                case 0x37: SLL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.A);  _clock.Add(3); break; // SLL (IR + d), A | UNDOCUMENTED
                case 0x36: SLL_IRDMEM(displacement, ref indexAddressingMode);  _clock.Add(3); break;                    // SLL (IR + d)


                // SR(A/L) instructions: Shift (IR + d) right arithmetic/logical
                case 0x28: SRA_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.B);  _clock.Add(3); break; // SRA (IR + d), B | UNDOCUMENTED
                case 0x29: SRA_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.C);  _clock.Add(3); break; // SRA (IR + d), C | UNDOCUMENTED
                case 0x2A: SRA_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.D);  _clock.Add(3); break; // SRA (IR + d), D | UNDOCUMENTED
                case 0x2B: SRA_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.E);  _clock.Add(3); break; // SRA (IR + d), E | UNDOCUMENTED
                case 0x2C: SRA_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.H);  _clock.Add(3); break; // SRA (IR + d), H | UNDOCUMENTED
                case 0x2D: SRA_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.L);  _clock.Add(3); break; // SRA (IR + d), L | UNDOCUMENTED
                case 0x2F: SRA_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.A);  _clock.Add(3); break; // SRA (IR + d), A | UNDOCUMENTED
                case 0x2E: SRA_IRDMEM(displacement, ref indexAddressingMode);  _clock.Add(3); break;                    // SRA (IR + d)

                case 0x38: SRL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.B);  _clock.Add(3); break; // SRL (IR + d), B | UNDOCUMENTED
                case 0x39: SRL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.C);  _clock.Add(3); break; // SRL (IR + d), C | UNDOCUMENTED
                case 0x3A: SRL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.D);  _clock.Add(3); break; // SRL (IR + d), D | UNDOCUMENTED
                case 0x3B: SRL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.E);  _clock.Add(3); break; // SRL (IR + d), E | UNDOCUMENTED
                case 0x3C: SRL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.H);  _clock.Add(3); break; // SRL (IR + d), H | UNDOCUMENTED
                case 0x3D: SRL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.L);  _clock.Add(3); break; // SRL (IR + d), L | UNDOCUMENTED
                case 0x3F: SRL_IRDMEM_R(displacement, ref indexAddressingMode, ref Registers.A);  _clock.Add(3); break; // SRL (IR + d), A | UNDOCUMENTED
                case 0x3E: SRL_IRDMEM(displacement, ref indexAddressingMode);  _clock.Add(3); break;                    // SRL (IR + d)


                // BIT instructions: Tests bit B of register R, sets Z if tested bit is zero
                case 0x40:  // BIT 0, (IR + d) | UNDOCUMENTED
                case 0x41:  // BIT 0, (IR + d) | UNDOCUMENTED
                case 0x42:  // BIT 0, (IR + d) | UNDOCUMENTED
                case 0x43:  // BIT 0, (IR + d) | UNDOCUMENTED
                case 0x44:  // BIT 0, (IR + d) | UNDOCUMENTED
                case 0x45:  // BIT 0, (IR + d) | UNDOCUMENTED
                case 0x46:  // BIT 0, (IR + d)
                case 0x47:  // BIT 0, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(0, displacement, ref indexAddressingMode); break;

                case 0x48:  // BIT 1, (IR + d) | UNDOCUMENTED
                case 0x49:  // BIT 1, (IR + d) | UNDOCUMENTED
                case 0x4A:  // BIT 1, (IR + d) | UNDOCUMENTED
                case 0x4B:  // BIT 1, (IR + d) | UNDOCUMENTED
                case 0x4C:  // BIT 1, (IR + d) | UNDOCUMENTED
                case 0x4D:  // BIT 1, (IR + d) | UNDOCUMENTED
                case 0x4E:  // BIT 1, (IR + d)
                case 0x4F:  // BIT 1, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(1, displacement, ref indexAddressingMode); break;

                case 0x50:  // BIT 2, (IR + d) | UNDOCUMENTED
                case 0x51:  // BIT 2, (IR + d) | UNDOCUMENTED
                case 0x52:  // BIT 2, (IR + d) | UNDOCUMENTED
                case 0x53:  // BIT 2, (IR + d) | UNDOCUMENTED
                case 0x54:  // BIT 2, (IR + d) | UNDOCUMENTED
                case 0x55:  // BIT 2, (IR + d) | UNDOCUMENTED
                case 0x56:  // BIT 2, (IR + d)
                case 0x57:  // BIT 2, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(2, displacement, ref indexAddressingMode); break;

                case 0x58:  // BIT 3, (IR + d) | UNDOCUMENTED
                case 0x59:  // BIT 3, (IR + d) | UNDOCUMENTED
                case 0x5A:  // BIT 3, (IR + d) | UNDOCUMENTED
                case 0x5B:  // BIT 3, (IR + d) | UNDOCUMENTED
                case 0x5C:  // BIT 3, (IR + d) | UNDOCUMENTED
                case 0x5D:  // BIT 3, (IR + d) | UNDOCUMENTED
                case 0x5E:  // BIT 3, (IR + d)
                case 0x5F:  // BIT 3, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(3, displacement, ref indexAddressingMode); break;

                case 0x60:  // BIT 4, (IR + d) | UNDOCUMENTED
                case 0x61:  // BIT 4, (IR + d) | UNDOCUMENTED
                case 0x62:  // BIT 4, (IR + d) | UNDOCUMENTED
                case 0x63:  // BIT 4, (IR + d) | UNDOCUMENTED
                case 0x64:  // BIT 4, (IR + d) | UNDOCUMENTED
                case 0x65:  // BIT 4, (IR + d) | UNDOCUMENTED
                case 0x66:  // BIT 4, (IR + d)
                case 0x67:  // BIT 4, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(4, displacement, ref indexAddressingMode); break;

                case 0x68:  // BIT 5, (IR + d) | UNDOCUMENTED
                case 0x69:  // BIT 5, (IR + d) | UNDOCUMENTED
                case 0x6A:  // BIT 5, (IR + d) | UNDOCUMENTED
                case 0x6B:  // BIT 5, (IR + d) | UNDOCUMENTED
                case 0x6C:  // BIT 5, (IR + d) | UNDOCUMENTED
                case 0x6D:  // BIT 5, (IR + d) | UNDOCUMENTED
                case 0x6E:  // BIT 5, (IR + d)
                case 0x6F:  // BIT 5, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(5, displacement, ref indexAddressingMode); break;

                case 0x70:  // BIT 6, (IR + d) | UNDOCUMENTED
                case 0x71:  // BIT 6, (IR + d) | UNDOCUMENTED
                case 0x72:  // BIT 6, (IR + d) | UNDOCUMENTED
                case 0x73:  // BIT 6, (IR + d) | UNDOCUMENTED
                case 0x74:  // BIT 6, (IR + d) | UNDOCUMENTED
                case 0x75:  // BIT 6, (IR + d) | UNDOCUMENTED
                case 0x76:  // BIT 6, (IR + d)
                case 0x77:  // BIT 6, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(6, displacement, ref indexAddressingMode); break;

                case 0x78:  // BIT 7, (IR + d) | UNDOCUMENTED
                case 0x79:  // BIT 7, (IR + d) | UNDOCUMENTED
                case 0x7A:  // BIT 7, (IR + d) | UNDOCUMENTED
                case 0x7B:  // BIT 7, (IR + d) | UNDOCUMENTED
                case 0x7C:  // BIT 7, (IR + d) | UNDOCUMENTED
                case 0x7D:  // BIT 7, (IR + d) | UNDOCUMENTED
                case 0x7E:  // BIT 7, (IR + d)
                case 0x7F:  // BIT 7, (IR + d) | UNDOCUMENTED
                    BIT_B_IRDMEM(7, displacement, ref indexAddressingMode); break;


                // RES instructions: Reset bit B of (IR + d)
                case 0x80: RES_B_IRDMEM_R(0, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // RES 0, (IR + d), B | UNDOCUMENTED
                case 0x81: RES_B_IRDMEM_R(0, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // RES 0, (IR + d), C | UNDOCUMENTED
                case 0x82: RES_B_IRDMEM_R(0, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // RES 0, (IR + d), D | UNDOCUMENTED
                case 0x83: RES_B_IRDMEM_R(0, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // RES 0, (IR + d), E | UNDOCUMENTED
                case 0x84: RES_B_IRDMEM_R(0, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // RES 0, (IR + d), H | UNDOCUMENTED
                case 0x85: RES_B_IRDMEM_R(0, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // RES 0, (IR + d), L | UNDOCUMENTED
                case 0x87: RES_B_IRDMEM_R(0, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // RES 0, (IR + d), A | UNDOCUMENTED
                case 0x86: RES_B_IRDMEM(0, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // RES 0, (IR + d)

                case 0x88: RES_B_IRDMEM_R(1, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // RES 1, (IR + d), B | UNDOCUMENTED
                case 0x89: RES_B_IRDMEM_R(1, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // RES 1, (IR + d), C | UNDOCUMENTED
                case 0x8A: RES_B_IRDMEM_R(1, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // RES 1, (IR + d), D | UNDOCUMENTED
                case 0x8B: RES_B_IRDMEM_R(1, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // RES 1, (IR + d), E | UNDOCUMENTED
                case 0x8C: RES_B_IRDMEM_R(1, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // RES 1, (IR + d), H | UNDOCUMENTED
                case 0x8D: RES_B_IRDMEM_R(1, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // RES 1, (IR + d), L | UNDOCUMENTED
                case 0x8F: RES_B_IRDMEM_R(1, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // RES 1, (IR + d), A | UNDOCUMENTED
                case 0x8E: RES_B_IRDMEM(1, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // RES 1, (IR + d)

                case 0x90: RES_B_IRDMEM_R(2, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // RES 2, (IR + d), B | UNDOCUMENTED
                case 0x91: RES_B_IRDMEM_R(2, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // RES 2, (IR + d), C | UNDOCUMENTED
                case 0x92: RES_B_IRDMEM_R(2, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // RES 2, (IR + d), D | UNDOCUMENTED
                case 0x93: RES_B_IRDMEM_R(2, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // RES 2, (IR + d), E | UNDOCUMENTED
                case 0x94: RES_B_IRDMEM_R(2, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // RES 2, (IR + d), H | UNDOCUMENTED
                case 0x95: RES_B_IRDMEM_R(2, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // RES 2, (IR + d), L | UNDOCUMENTED
                case 0x97: RES_B_IRDMEM_R(2, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // RES 2, (IR + d), A | UNDOCUMENTED
                case 0x96: RES_B_IRDMEM(2, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // RES 2, (IR + d)

                case 0x98: RES_B_IRDMEM_R(3, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // RES 3, (IR + d), B | UNDOCUMENTED
                case 0x99: RES_B_IRDMEM_R(3, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // RES 3, (IR + d), C | UNDOCUMENTED
                case 0x9A: RES_B_IRDMEM_R(3, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // RES 3, (IR + d), D | UNDOCUMENTED
                case 0x9B: RES_B_IRDMEM_R(3, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // RES 3, (IR + d), E | UNDOCUMENTED
                case 0x9C: RES_B_IRDMEM_R(3, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // RES 3, (IR + d), H | UNDOCUMENTED
                case 0x9D: RES_B_IRDMEM_R(3, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // RES 3, (IR + d), L | UNDOCUMENTED
                case 0x9F: RES_B_IRDMEM_R(3, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // RES 3, (IR + d), A | UNDOCUMENTED
                case 0x9E: RES_B_IRDMEM(3, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // RES 3, (IR + d)

                case 0xA0: RES_B_IRDMEM_R(4, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // RES 4, (IR + d), B | UNDOCUMENTED
                case 0xA1: RES_B_IRDMEM_R(4, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // RES 4, (IR + d), C | UNDOCUMENTED
                case 0xA2: RES_B_IRDMEM_R(4, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // RES 4, (IR + d), D | UNDOCUMENTED
                case 0xA3: RES_B_IRDMEM_R(4, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // RES 4, (IR + d), E | UNDOCUMENTED
                case 0xA4: RES_B_IRDMEM_R(4, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // RES 4, (IR + d), H | UNDOCUMENTED
                case 0xA5: RES_B_IRDMEM_R(4, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // RES 4, (IR + d), L | UNDOCUMENTED
                case 0xA7: RES_B_IRDMEM_R(4, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // RES 4, (IR + d), A | UNDOCUMENTED
                case 0xA6: RES_B_IRDMEM(4, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // RES 4, (IR + d)

                case 0xA8: RES_B_IRDMEM_R(5, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // RES 5, (IR + d), B | UNDOCUMENTED
                case 0xA9: RES_B_IRDMEM_R(5, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // RES 5, (IR + d), C | UNDOCUMENTED
                case 0xAA: RES_B_IRDMEM_R(5, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // RES 5, (IR + d), D | UNDOCUMENTED
                case 0xAB: RES_B_IRDMEM_R(5, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // RES 5, (IR + d), E | UNDOCUMENTED
                case 0xAC: RES_B_IRDMEM_R(5, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // RES 5, (IR + d), H | UNDOCUMENTED
                case 0xAD: RES_B_IRDMEM_R(5, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // RES 5, (IR + d), L | UNDOCUMENTED
                case 0xAF: RES_B_IRDMEM_R(5, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // RES 5, (IR + d), A | UNDOCUMENTED
                case 0xAE: RES_B_IRDMEM(5, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // RES 5, (IR + d)

                case 0xB0: RES_B_IRDMEM_R(6, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // RES 6, (IR + d), B | UNDOCUMENTED
                case 0xB1: RES_B_IRDMEM_R(6, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // RES 6, (IR + d), C | UNDOCUMENTED
                case 0xB2: RES_B_IRDMEM_R(6, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // RES 6, (IR + d), D | UNDOCUMENTED
                case 0xB3: RES_B_IRDMEM_R(6, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // RES 6, (IR + d), E | UNDOCUMENTED
                case 0xB4: RES_B_IRDMEM_R(6, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // RES 6, (IR + d), H | UNDOCUMENTED
                case 0xB5: RES_B_IRDMEM_R(6, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // RES 6, (IR + d), L | UNDOCUMENTED
                case 0xB7: RES_B_IRDMEM_R(6, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // RES 6, (IR + d), A | UNDOCUMENTED
                case 0xB6: RES_B_IRDMEM(6, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // RES 6, (IR + d)

                case 0xB8: RES_B_IRDMEM_R(7, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // RES 7, (IR + d), B | UNDOCUMENTED
                case 0xB9: RES_B_IRDMEM_R(7, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // RES 7, (IR + d), C | UNDOCUMENTED
                case 0xBA: RES_B_IRDMEM_R(7, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // RES 7, (IR + d), D | UNDOCUMENTED
                case 0xBB: RES_B_IRDMEM_R(7, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // RES 7, (IR + d), E | UNDOCUMENTED
                case 0xBC: RES_B_IRDMEM_R(7, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // RES 7, (IR + d), H | UNDOCUMENTED
                case 0xBD: RES_B_IRDMEM_R(7, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // RES 7, (IR + d), L | UNDOCUMENTED
                case 0xBF: RES_B_IRDMEM_R(7, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // RES 7, (IR + d), A | UNDOCUMENTED
                case 0xBE: RES_B_IRDMEM(7, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // RES 7, (IR + d)


                // SET instructions: Set bit B of (IR + d)
                case 0xC0: SET_B_IRDMEM_R(0, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // SET 0, (IR + d), B | UNDOCUMENTED
                case 0xC1: SET_B_IRDMEM_R(0, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // SET 0, (IR + d), C | UNDOCUMENTED
                case 0xC2: SET_B_IRDMEM_R(0, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // SET 0, (IR + d), D | UNDOCUMENTED
                case 0xC3: SET_B_IRDMEM_R(0, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // SET 0, (IR + d), E | UNDOCUMENTED
                case 0xC4: SET_B_IRDMEM_R(0, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // SET 0, (IR + d), H | UNDOCUMENTED
                case 0xC5: SET_B_IRDMEM_R(0, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // SET 0, (IR + d), L | UNDOCUMENTED
                case 0xC7: SET_B_IRDMEM_R(0, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // SET 0, (IR + d), A | UNDOCUMENTED
                case 0xC6: SET_B_IRDMEM(0, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // SET 0, (IR + d)

                case 0xC8: SET_B_IRDMEM_R(1, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // SET 1, (IR + d), B | UNDOCUMENTED
                case 0xC9: SET_B_IRDMEM_R(1, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // SET 1, (IR + d), C | UNDOCUMENTED
                case 0xCA: SET_B_IRDMEM_R(1, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // SET 1, (IR + d), D | UNDOCUMENTED
                case 0xCB: SET_B_IRDMEM_R(1, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // SET 1, (IR + d), E | UNDOCUMENTED
                case 0xCC: SET_B_IRDMEM_R(1, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // SET 1, (IR + d), H | UNDOCUMENTED
                case 0xCD: SET_B_IRDMEM_R(1, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // SET 1, (IR + d), L | UNDOCUMENTED
                case 0xCF: SET_B_IRDMEM_R(1, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // SET 1, (IR + d), A | UNDOCUMENTED
                case 0xCE: SET_B_IRDMEM(1, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // SET 1, (IR + d)

                case 0xD0: SET_B_IRDMEM_R(2, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // SET 2, (IR + d), B | UNDOCUMENTED
                case 0xD1: SET_B_IRDMEM_R(2, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // SET 2, (IR + d), C | UNDOCUMENTED
                case 0xD2: SET_B_IRDMEM_R(2, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // SET 2, (IR + d), D | UNDOCUMENTED
                case 0xD3: SET_B_IRDMEM_R(2, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // SET 2, (IR + d), E | UNDOCUMENTED
                case 0xD4: SET_B_IRDMEM_R(2, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // SET 2, (IR + d), H | UNDOCUMENTED
                case 0xD5: SET_B_IRDMEM_R(2, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // SET 2, (IR + d), L | UNDOCUMENTED
                case 0xD7: SET_B_IRDMEM_R(2, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // SET 2, (IR + d), A | UNDOCUMENTED
                case 0xD6: SET_B_IRDMEM(2, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // SET 2, (IR + d)

                case 0xD8: SET_B_IRDMEM_R(3, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // SET 3, (IR + d), B | UNDOCUMENTED
                case 0xD9: SET_B_IRDMEM_R(3, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // SET 3, (IR + d), C | UNDOCUMENTED
                case 0xDA: SET_B_IRDMEM_R(3, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // SET 3, (IR + d), D | UNDOCUMENTED
                case 0xDB: SET_B_IRDMEM_R(3, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // SET 3, (IR + d), E | UNDOCUMENTED
                case 0xDC: SET_B_IRDMEM_R(3, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // SET 3, (IR + d), H | UNDOCUMENTED
                case 0xDD: SET_B_IRDMEM_R(3, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // SET 3, (IR + d), L | UNDOCUMENTED
                case 0xDF: SET_B_IRDMEM_R(3, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // SET 3, (IR + d), A | UNDOCUMENTED
                case 0xDE: SET_B_IRDMEM(3, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // SET 3, (IR + d)

                case 0xE0: SET_B_IRDMEM_R(4, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // SET 4, (IR + d), B | UNDOCUMENTED
                case 0xE1: SET_B_IRDMEM_R(4, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // SET 4, (IR + d), C | UNDOCUMENTED
                case 0xE2: SET_B_IRDMEM_R(4, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // SET 4, (IR + d), D | UNDOCUMENTED
                case 0xE3: SET_B_IRDMEM_R(4, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // SET 4, (IR + d), E | UNDOCUMENTED
                case 0xE4: SET_B_IRDMEM_R(4, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // SET 4, (IR + d), H | UNDOCUMENTED
                case 0xE5: SET_B_IRDMEM_R(4, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // SET 4, (IR + d), L | UNDOCUMENTED
                case 0xE7: SET_B_IRDMEM_R(4, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // SET 4, (IR + d), A | UNDOCUMENTED
                case 0xE6: SET_B_IRDMEM(4, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // SET 4, (IR + d)

                case 0xE8: SET_B_IRDMEM_R(5, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // SET 5, (IR + d), B | UNDOCUMENTED
                case 0xE9: SET_B_IRDMEM_R(5, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // SET 5, (IR + d), C | UNDOCUMENTED
                case 0xEA: SET_B_IRDMEM_R(5, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // SET 5, (IR + d), D | UNDOCUMENTED
                case 0xEB: SET_B_IRDMEM_R(5, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // SET 5, (IR + d), E | UNDOCUMENTED
                case 0xEC: SET_B_IRDMEM_R(5, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // SET 5, (IR + d), H | UNDOCUMENTED
                case 0xED: SET_B_IRDMEM_R(5, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // SET 5, (IR + d), L | UNDOCUMENTED
                case 0xEF: SET_B_IRDMEM_R(5, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // SET 5, (IR + d), A | UNDOCUMENTED
                case 0xEE: SET_B_IRDMEM(5, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // SET 5, (IR + d)

                case 0xF0: SET_B_IRDMEM_R(6, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // SET 6, (IR + d), B | UNDOCUMENTED
                case 0xF1: SET_B_IRDMEM_R(6, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // SET 6, (IR + d), C | UNDOCUMENTED
                case 0xF2: SET_B_IRDMEM_R(6, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // SET 6, (IR + d), D | UNDOCUMENTED
                case 0xF3: SET_B_IRDMEM_R(6, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // SET 6, (IR + d), E | UNDOCUMENTED
                case 0xF4: SET_B_IRDMEM_R(6, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // SET 6, (IR + d), H | UNDOCUMENTED
                case 0xF5: SET_B_IRDMEM_R(6, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // SET 6, (IR + d), L | UNDOCUMENTED
                case 0xF7: SET_B_IRDMEM_R(6, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // SET 6, (IR + d), A | UNDOCUMENTED
                case 0xF6: SET_B_IRDMEM(6, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // SET 6, (IR + d)

                case 0xF8: SET_B_IRDMEM_R(7, displacement, ref indexAddressingMode, ref Registers.B); _clock.Add(3); break; // SET 7, (IR + d), B | UNDOCUMENTED
                case 0xF9: SET_B_IRDMEM_R(7, displacement, ref indexAddressingMode, ref Registers.C); _clock.Add(3); break; // SET 7, (IR + d), C | UNDOCUMENTED
                case 0xFA: SET_B_IRDMEM_R(7, displacement, ref indexAddressingMode, ref Registers.D); _clock.Add(3); break; // SET 7, (IR + d), D | UNDOCUMENTED
                case 0xFB: SET_B_IRDMEM_R(7, displacement, ref indexAddressingMode, ref Registers.E); _clock.Add(3); break; // SET 7, (IR + d), E | UNDOCUMENTED
                case 0xFC: SET_B_IRDMEM_R(7, displacement, ref indexAddressingMode, ref Registers.H); _clock.Add(3); break; // SET 7, (IR + d), H | UNDOCUMENTED
                case 0xFD: SET_B_IRDMEM_R(7, displacement, ref indexAddressingMode, ref Registers.L); _clock.Add(3); break; // SET 7, (IR + d), L | UNDOCUMENTED
                case 0xFF: SET_B_IRDMEM_R(7, displacement, ref indexAddressingMode, ref Registers.A); _clock.Add(3); break; // SET 7, (IR + d), A | UNDOCUMENTED
                case 0xFE: SET_B_IRDMEM(7, displacement, ref indexAddressingMode); _clock.Add(3); break;                    // SET 7, (IR + d)
            }
        }
    }
}
