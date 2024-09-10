﻿using System;
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

                // LD instructions: Load values between 16-bit register pairs
                case 0xF9: LD_RR_RR(SPi, H); break; // LD SP, HL

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

                // JP and JR instructions: Jump to absolute or relative address
                case 0x10: DJNZ_D(); break; // DJNZ D
                case 0xC3: JP_NN(); break;  // JP NN
                case 0xE9: JP_RR(H); break; // JP (HL)
                case 0xC2: JP_NN_C(0b000); break; // JP NZ, NN
                case 0xCA: JP_NN_C(0b001); break; // JP Z, NN
                case 0xD2: JP_NN_C(0b010); break; // JP NC, NN
                case 0xDA: JP_NN_C(0b011); break; // JP C, NN
                case 0xE2: JP_NN_C(0b100); break; // JP PO, NN
                case 0xEA: JP_NN_C(0b101); break; // JP PE, NN
                case 0xF2: JP_NN_C(0b110); break; // JP P, NN
                case 0xFA: JP_NN_C(0b111); break; // JP M, NN

                case 0x18: JR_D(); break; // JR D
                case 0x20: JR_CC_D(0b000); break; // JR NZ, D
                case 0x28: JR_CC_D(0b001); break; // JR Z, D
                case 0x30: JR_CC_D(0b010); break; // JR NC, D
                case 0x38: JR_CC_D(0b011); break; // JR C, D

                // RET instructions: return 
                case 0xC9: RET(); break; // RET
                case 0xC0: RET_CC(0b000); break; // RET NZ
                case 0xC8: RET_CC(0b001); break; // RET Z
                case 0xD0: RET_CC(0b010); break; // RET NC
                case 0xD8: RET_CC(0b011); break; // RET C
                case 0xE0: RET_CC(0b100); break; // RET PO
                case 0xE8: RET_CC(0b101); break; // RET PE
                case 0xF0: RET_CC(0b110); break; // RET P
                case 0xF8: RET_CC(0b111); break; // RET M

                // POP instructions: pop value at SP into RR and inc. SP
                case 0xC1: POP_RR(B); break; // POP BC
                case 0xD1: POP_RR(D); break; // POP DE
                case 0xE1: POP_RR(H); break; // POP HL
                case 0xF1: POP_AF(); break;  // POP AF

                // PUSH instructions: push value in RR on to stack and dec. SP
                case 0xC5: PUSH_RR(B); break; // PUSH BC
                case 0xD5: PUSH_RR(D); break; // PUSH DE
                case 0xE5: PUSH_RR(H); break; // PUSH HL
                case 0xF5: PUSH_AF(); break;  // PUSH AF

                // RST instructions: restart to 0xXX
                case 0xC7: RST_HH(0x00); break; // RST 00h
                case 0xD7: RST_HH(0x10); break; // RST 10h
                case 0xE7: RST_HH(0x20); break; // RST 20h
                case 0xF7: RST_HH(0x30); break; // RST 30h
                case 0xCF: RST_HH(0x08); break; // RST 08h
                case 0xDF: RST_HH(0x18); break; // RST 18h
                case 0xEF: RST_HH(0x28); break; // RST 28h
                case 0xFF: RST_HH(0x38); break; // RST 38h

                // CALL instructions: push current PC to stack and jump to immediate
                case 0xCD: CALL_NN(); break; // CALL NN
                case 0xC4: CALL_CC_NN(0b000); break; // CALL NZ, NN
                case 0xCC: CALL_CC_NN(0b001); break; // CALL Z, NN
                case 0xD4: CALL_CC_NN(0b010); break; // CALL NC, NN
                case 0xDC: CALL_CC_NN(0b011); break; // CALL C, NN
                case 0xE4: CALL_CC_NN(0b100); break; // CALL PO, NN
                case 0xEC: CALL_CC_NN(0b101); break; // CALL PE, NN
                case 0xF4: CALL_CC_NN(0b110); break; // CALL P, NN
                case 0xFC: CALL_CC_NN(0b111); break; // CALL M, NN

                case 0xF3: DI(); break; // Disable Interrupts
                case 0xFB: EI(); break; // Enable Interrupts

                default:
                    _logger.Log(Enums.LogSeverity.Fatal, $"Unrecognized MAIN opcode: 0x{_currentInstruction:X2}");
                    Halted = true;
                    break;
            }
        }

        private void ExecuteMiscInstruction()
        {
            _logger.Log(Enums.LogSeverity.Decode, $"MISC decoded: 0x{_currentInstruction:X2}");
        }

        private void ExecuteBitInstruction()
        {
            _logger.Log(Enums.LogSeverity.Decode, $"BIT decoded: 0x{_currentInstruction:X2}");
        }

        private void ExecuteIndexXInstruction()
        {
            _logger.Log(Enums.LogSeverity.Decode, $"INXR decoded: 0x{_currentInstruction:X2}");
        }

        private void ExecuteIndexYInstruction()
        {
            _logger.Log(Enums.LogSeverity.Decode, $"INYR decoded: 0x{_currentInstruction:X2}");
        }
    }
}