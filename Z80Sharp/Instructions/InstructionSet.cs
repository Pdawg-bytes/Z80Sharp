using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    // Z80 Instruction Tables
    public partial class Z80
    {
        /// <summary>
        /// The main instruction table, containing all of the regular instructions.
        /// </summary>
        private readonly Action[] instructionTable = new Action[256];

        /// <summary>
        /// Index X register instruction table.
        /// </summary>
        private readonly Action[] DDInstructionTable = new Action[256];

        /// <summary>
        /// Index Y register instruction table.
        /// </summary>
        private readonly Action[] FDInstructionTable = new Action[256];

        /// <summary>
        /// Misc. instruction table.
        /// </summary>
        private readonly Action[] EDInstructionTable = new Action[256];

        /// <summary>
        /// Bit instruction table.
        /// </summary>
        private readonly Action[] CBInstructionTable = new Action[256];


        private void ExecuteMainInstruction()
        {
            _logger.Log(Enums.LogSeverity.Decode, $"MAIN decoded: 0x{_currentInstruction:X2}");
            switch (_currentInstruction)
            {

                case 0x00: NOP(); break;
                // case 0x76: HALT(); break; // Uncomment if HALT is defined

                // LD instructions: Load immediate 16-bit values into register pairs
                case 0x01: LD_RR_NN(B); break;  // LD BC, NN
                case 0x11: LD_RR_NN(D); break;  // LD DE, NN
                case 0x21: LD_RR_NN(H); break;  // LD HL, NN
                case 0x31: LD_SP_NN(); break;   // LD SP, NN

                // LD instructions: Load immediate 8-bit values into registers
                case 0x06: LD_R_N(B); break;    // LD B, N
                case 0x0E: LD_R_N(C); break;    // LD C, N
                case 0x16: LD_R_N(D); break;    // LD D, N
                case 0x1E: LD_R_N(E); break;    // LD E, N
                case 0x26: LD_R_N(H); break;    // LD H, N
                case 0x2E: LD_R_N(L); break;    // LD L, N
                case 0x3E: LD_R_N(A); break;    // LD A, N

                // LD instructions: Load values between registers and memory
                case 0x36: LD_HLMEM_N(); break; // LD (HL), N
                case 0x02: LD_RRMEM_R(B, A); break;  // LD (BC), A
                case 0x12: LD_RRMEM_R(D, A); break;  // LD (DE), A
                case 0x0A: LD_R_RRMEM(A, B); break;  // LD A, (BC)
                case 0x1A: LD_R_RRMEM(A, D); break;  // LD A, (DE)

                // LD instructions: Load values between registers and absolute memory addresses
                case 0x32: LD_NNMEM_R(A); break;    // LD (NN), A
                case 0x22: LD_NNMEM_RR(H); break;   // LD (NN), HL
                case 0x2A: LD_RR_NNMEM(H); break;   // LD HL, (NN)
                case 0x3A: LD_R_NNMEM(A); break;    // LD A, (NN)

                // LD instructions: Load 8-bit register to 8-bit register or from memory (LD R, R and LD R, (HL))
                case 0x40: LD_R_R(B, B); break;  // LD B, B
                case 0x41: LD_R_R(B, C); break;  // LD B, C
                case 0x42: LD_R_R(B, D); break;  // LD B, D
                case 0x43: LD_R_R(B, E); break;  // LD B, E
                case 0x44: LD_R_R(B, H); break;  // LD B, H
                case 0x45: LD_R_R(B, L); break;  // LD B, L
                case 0x46: LD_R_RRMEM(B, H); break;  // LD B, (HL)
                case 0x47: LD_R_R(B, A); break;  // LD B, A

                case 0x48: LD_R_R(C, B); break;  // LD C, B
                case 0x49: LD_R_R(C, C); break;  // LD C, C
                case 0x4A: LD_R_R(C, D); break;  // LD C, D
                case 0x4B: LD_R_R(C, E); break;  // LD C, E
                case 0x4C: LD_R_R(C, H); break;  // LD C, H
                case 0x4D: LD_R_R(C, L); break;  // LD C, L
                case 0x4E: LD_R_RRMEM(C, H); break;  // LD C, (HL)
                case 0x4F: LD_R_R(C, A); break;  // LD C, A

                case 0x50: LD_R_R(D, B); break;  // LD D, B
                case 0x51: LD_R_R(D, C); break;  // LD D, C
                case 0x52: LD_R_R(D, D); break;  // LD D, D
                case 0x53: LD_R_R(D, E); break;  // LD D, E
                case 0x54: LD_R_R(D, H); break;  // LD D, H
                case 0x55: LD_R_R(D, L); break;  // LD D, L
                case 0x56: LD_R_RRMEM(D, H); break;  // LD D, (HL)
                case 0x57: LD_R_R(D, A); break;  // LD D, A

                case 0x58: LD_R_R(E, B); break;  // LD E, B
                case 0x59: LD_R_R(E, C); break;  // LD E, C
                case 0x5A: LD_R_R(E, D); break;  // LD E, D
                case 0x5B: LD_R_R(E, E); break;  // LD E, E
                case 0x5C: LD_R_R(E, H); break;  // LD E, H
                case 0x5D: LD_R_R(E, L); break;  // LD E, L
                case 0x5E: LD_R_RRMEM(E, H); break;  // LD E, (HL)
                case 0x5F: LD_R_R(E, A); break;  // LD E, A

                case 0x60: LD_R_R(H, B); break;  // LD H, B
                case 0x61: LD_R_R(H, C); break;  // LD H, C
                case 0x62: LD_R_R(H, D); break;  // LD H, D
                case 0x63: LD_R_R(H, E); break;  // LD H, E
                case 0x64: LD_R_R(H, H); break;  // LD H, H
                case 0x65: LD_R_R(H, L); break;  // LD H, L
                case 0x66: LD_R_RRMEM(H, H); break;  // LD H, (HL)
                case 0x67: LD_R_R(H, A); break;  // LD H, A

                case 0x68: LD_R_R(L, B); break;  // LD L, B
                case 0x69: LD_R_R(L, C); break;  // LD L, C
                case 0x6A: LD_R_R(L, D); break;  // LD L, D
                case 0x6B: LD_R_R(L, E); break;  // LD L, E
                case 0x6C: LD_R_R(L, H); break;  // LD L, H
                case 0x6D: LD_R_R(L, L); break;  // LD L, L
                case 0x6E: LD_R_RRMEM(L, H); break;  // LD L, (HL)
                case 0x6F: LD_R_R(L, A); break;  // LD L, A

                case 0x78: LD_R_R(A, B); break;  // LD A, B
                case 0x79: LD_R_R(A, C); break;  // LD A, C
                case 0x7A: LD_R_R(A, D); break;  // LD A, D
                case 0x7B: LD_R_R(A, E); break;  // LD A, E
                case 0x7C: LD_R_R(A, H); break;  // LD A, H
                case 0x7D: LD_R_R(A, L); break;  // LD A, L
                case 0x7E: LD_R_RRMEM(A, H); break;  // LD A, (HL)
                case 0x7F: LD_R_R(A, A); break;  // LD A, A

                // LD instructions: Load memory from register
                case 0x70: LD_RRMEM_R(H, B); break;  // LD (HL), B
                case 0x71: LD_RRMEM_R(H, C); break;  // LD (HL), C
                case 0x72: LD_RRMEM_R(H, D); break;  // LD (HL), D
                case 0x73: LD_RRMEM_R(H, E); break;  // LD (HL), E
                case 0x74: LD_RRMEM_R(H, H); break;  // LD (HL), H
                case 0x75: LD_RRMEM_R(H, L); break;  // LD (HL), L
                case 0x77: LD_RRMEM_R(H, A); break;  // LD (HL), A

                default:
                    _logger.Log(Enums.LogSeverity.Fatal, $"Unrecognized MAIN opcode: 0x{_currentInstruction:X2}");
                    Halted = true;
                    throw new InvalidOperationException($"Unrecognized MAIN opcode: 0x{_currentInstruction:X2}");
            }
        }
        /// <summary>
        /// Initialize instruction tables.
        /// </summary>
        private void InitializeInstructionHandlers()
        {
            instructionTable[0x00] = NOP;
            // 0x76: HALT


            instructionTable[0x01] = () => LD_RR_NN(B); // LD BC, NN
            instructionTable[0x11] = () => LD_RR_NN(D); // LD DE, NN
            instructionTable[0x21] = () => LD_RR_NN(H); // LD HL, NN
            instructionTable[0x31] = LD_SP_NN; // LD SP, NN


            instructionTable[0x06] = () => LD_R_N(B); // LD B, N
            instructionTable[0x0E] = () => LD_R_N(C); // LD C, N
            instructionTable[0x16] = () => LD_R_N(D); // LD D, N
            instructionTable[0x1E] = () => LD_R_N(E); // LD E, N
            instructionTable[0x26] = () => LD_R_N(H); // LD H, N
            instructionTable[0x2E] = () => LD_R_N(L); // LD L, N
            instructionTable[0x3E] = () => LD_R_N(A); // LD A, N


            instructionTable[0x36] = LD_HLMEM_N; // LD (HL), N


            instructionTable[0x02] = () => LD_RRMEM_R(B, A); // LD (BC), A
            instructionTable[0x12] = () => LD_RRMEM_R(D, A); // LD (DE), A

            instructionTable[0x0A] = () => LD_R_RRMEM(A, B); // LD A, (BC)
            instructionTable[0x1A] = () => LD_R_RRMEM(A, D); // LD A, (DE)


            instructionTable[0x32] = () => LD_NNMEM_R(A); // LD (NN), A

            instructionTable[0x22] = () => LD_NNMEM_RR(H); // LD (NN), HL

            instructionTable[0x2A] = () => LD_RR_NNMEM(H); // LD HL, (NN)
            instructionTable[0x3A] = () => LD_R_NNMEM(A); // LD A, (NN)


            instructionTable[0x40] = () => LD_R_R(B, B); // LD B, B
            instructionTable[0x41] = () => LD_R_R(B, C); // LD B, C
            instructionTable[0x42] = () => LD_R_R(B, D); // LD B, D
            instructionTable[0x43] = () => LD_R_R(B, E); // LD B, E
            instructionTable[0x44] = () => LD_R_R(B, H); // LD B, H
            instructionTable[0x45] = () => LD_R_R(B, L); // LD B, L
            instructionTable[0x46] = () => LD_R_RRMEM(B, H); // LD B, (HL)
            instructionTable[0x47] = () => LD_R_R(B, A); // LD B, A

            instructionTable[0x48] = () => LD_R_R(C, B); // LD C, B
            instructionTable[0x49] = () => LD_R_R(C, C); // LD C, C
            instructionTable[0x4A] = () => LD_R_R(C, D); // LD C, D
            instructionTable[0x4B] = () => LD_R_R(C, E); // LD C, E
            instructionTable[0x4C] = () => LD_R_R(C, H); // LD C, H
            instructionTable[0x4D] = () => LD_R_R(C, L); // LD C, L
            instructionTable[0x4E] = () => LD_R_RRMEM(C, H); // LD C, (HL)
            instructionTable[0x4F] = () => LD_R_R(C, A); // LD C, A

            instructionTable[0x50] = () => LD_R_R(D, B); // LD D, B
            instructionTable[0x51] = () => LD_R_R(D, C); // LD D, C
            instructionTable[0x52] = () => LD_R_R(D, D); // LD D, D
            instructionTable[0x53] = () => LD_R_R(D, E); // LD D, E
            instructionTable[0x54] = () => LD_R_R(D, H); // LD D, H
            instructionTable[0x55] = () => LD_R_R(D, L); // LD D, L
            instructionTable[0x56] = () => LD_R_RRMEM(D, H); // LD D, (HL)
            instructionTable[0x57] = () => LD_R_R(D, A); // LD D, A

            instructionTable[0x58] = () => LD_R_R(E, B); // LD E, B
            instructionTable[0x59] = () => LD_R_R(E, C); // LD E, C
            instructionTable[0x5A] = () => LD_R_R(E, D); // LD E, D
            instructionTable[0x5B] = () => LD_R_R(E, E); // LD E, E
            instructionTable[0x5C] = () => LD_R_R(E, H); // LD E, H
            instructionTable[0x5D] = () => LD_R_R(E, L); // LD E, L
            instructionTable[0x5E] = () => LD_R_RRMEM(E, H); // LD E, (HL)
            instructionTable[0x5F] = () => LD_R_R(E, A); // LD E, A

            instructionTable[0x60] = () => LD_R_R(H, B); // LD H, B
            instructionTable[0x61] = () => LD_R_R(H, C); // LD H, C
            instructionTable[0x62] = () => LD_R_R(H, D); // LD H, D
            instructionTable[0x63] = () => LD_R_R(H, E); // LD H, E
            instructionTable[0x64] = () => LD_R_R(H, H); // LD H, H
            instructionTable[0x65] = () => LD_R_R(H, L); // LD H, L
            instructionTable[0x66] = () => LD_R_RRMEM(H, H); // LD H, (HL)
            instructionTable[0x67] = () => LD_R_R(H, A); // LD H, A

            instructionTable[0x68] = () => LD_R_R(L, B); // LD L, B
            instructionTable[0x69] = () => LD_R_R(L, C); // LD L, C
            instructionTable[0x6A] = () => LD_R_R(L, D); // LD L, D
            instructionTable[0x6B] = () => LD_R_R(L, E); // LD L, E
            instructionTable[0x6C] = () => LD_R_R(L, H); // LD L, H
            instructionTable[0x6D] = () => LD_R_R(L, L); // LD L, L
            instructionTable[0x6E] = () => LD_R_RRMEM(L, H); // LD L, (HL)
            instructionTable[0x6F] = () => LD_R_R(L, A); // LD L, A

            instructionTable[0x78] = () => LD_R_R(A, B); // LD A, B
            instructionTable[0x79] = () => LD_R_R(A, C); // LD A, C
            instructionTable[0x7A] = () => LD_R_R(A, D); // LD A, D
            instructionTable[0x7B] = () => LD_R_R(A, E); // LD A, E
            instructionTable[0x7C] = () => LD_R_R(A, H); // LD A, H
            instructionTable[0x7D] = () => LD_R_R(A, L); // LD A, L
            instructionTable[0x7E] = () => LD_R_RRMEM(A, H); // LD A, (HL)
            instructionTable[0x7F] = () => LD_R_R(A, A); // LD A, A

            instructionTable[0x70] = () => LD_RRMEM_R(H, B); // LD (HL), B
            instructionTable[0x71] = () => LD_RRMEM_R(H, C); // LD (HL), C
            instructionTable[0x72] = () => LD_RRMEM_R(H, D); // LD (HL), D
            instructionTable[0x73] = () => LD_RRMEM_R(H, E); // LD (HL), E
            instructionTable[0x74] = () => LD_RRMEM_R(H, H); // LD (HL), H
            instructionTable[0x75] = () => LD_RRMEM_R(H, L); // LD (HL), L
            instructionTable[0x77] = () => LD_RRMEM_R(H, A); // LD (HL), A
        }
    }
}
