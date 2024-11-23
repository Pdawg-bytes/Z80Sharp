using System.Runtime.CompilerServices;
using Z80Sharp.Enums;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExecuteMiscInstruction()
        {
            byte instruction = Fetch();
            _currentInstruction = instruction;
            switch (instruction)
            {
                case 0x44: NEG(); break; // NEG

                case 0x45: RETN(); break; // RETN
                case 0x4D: RETI(); break; // RETI

                case 0x67: RRD(); break; // RRD
                case 0x6F: RLD(); break; // RLD

                case 0xA0: LDI(); break;  // LDI
                case 0xB0: LDIR(); break; // LDIR
                case 0xA8: LDD(); break;  // LDD
                case 0xB8: LDDR(); break; // LDDR

                case 0xA1: CPI(); break;  // CPI
                case 0xB1: CPIR(); break; // CPIR
                case 0xA9: CPD(); break;  // CPD
                case 0xB9: CPDR(); break; // CPDR

                case 0xA2: INI(); break;  // INI
                case 0xB2: INIR(); break; // INIR
                case 0xAA: IND(); break;  // IND
                case 0xBA: INDR(); break; // INDR

                case 0xA3: OUTI(); break; // OUTI
                case 0xB3: OTIR(); break; // OTIR
                case 0xAB: OUTD(); break; // OUTD
                case 0xBB: OTDR(); break; // OTDR

                // ADC/SBC HL: Add/Subtract with carry to/from HL
                case 0x42: SBC_HL_RR(ref Registers.BC); break; // SBC HL, BC
                case 0x52: SBC_HL_RR(ref Registers.DE); break; // SBC HL, DE
                case 0x62: SBC_HL_RR(ref Registers.HL); break; // SBC HL, HL
                case 0x72: SBC_HL_RR(ref Registers.SP); break; // SBC HL, SP

                case 0x4A: ADC_HL_RR(ref Registers.BC); break; // ADC HL, BC
                case 0x5A: ADC_HL_RR(ref Registers.DE); break; // ADC HL, DE
                case 0x6A: ADC_HL_RR(ref Registers.HL); break; // ADC HL, HL
                case 0x7A: ADC_HL_RR(ref Registers.SP); break; // ADC HL, SP

                // LD instructions: Load values to/from registers or memory
                case 0x43: LD_NNMEM_RR(ref Registers.BC); break; // LD (NN), BC
                case 0x53: LD_NNMEM_RR(ref Registers.DE); break; // LD (NN), DE
                case 0x63: LD_NNMEM_RR(ref Registers.HL); break; // LD (NN), HL | (UNDOCUMENTED)
                case 0x73: LD_NNMEM_RR(ref Registers.SP); break; // LD (NN), SP

                case 0x4B: LD_RR_NNMEM(ref Registers.BC); break; // LD BC, (NN)
                case 0x5B: LD_RR_NNMEM(ref Registers.DE); break; // LD DE, (NN)
                case 0x6B: LD_RR_NNMEM(ref Registers.HL); break; // LD HL, (NN) | (UNDOCUMENTED)
                case 0x7B: LD_RR_NNMEM(ref Registers.SP); break; // LD SP, (NN)

                case 0x47: LD_R_R(ref Registers.I, ref Registers.A); break; // LD I, A
                case 0x57: LD_R_R(ref Registers.A, ref Registers.I); break; // LD A, I
                case 0x4F: LD_R_R(ref Registers.R, ref Registers.A); break; // LD R, A
                case 0x5F: LD_R_R(ref Registers.A, ref Registers.R); break; // LD A, R

                // IMx instructions: change the interrupt mode of the Z80
                case 0x46: IM_M(InterruptMode.IM0); break; // IM0
                case 0x56: IM_M(InterruptMode.IM1); break; // IM1
                case 0x5E: IM_M(InterruptMode.IM2); break; // IM2

                // IN instructions: read from port into register
                case 0x40: IN_R_CPORT(ref Registers.B); break; // IN B, (C)
                case 0x48: IN_R_CPORT(ref Registers.C); break; // IN C, (C)
                case 0x50: IN_R_CPORT(ref Registers.D); break; // IN D, (C)
                case 0x58: IN_R_CPORT(ref Registers.E); break; // IN E, (C)
                case 0x60: IN_R_CPORT(ref Registers.H); break; // IN H, (C)
                case 0x68: IN_R_CPORT(ref Registers.L); break; // IN L, (C)
                case 0x78: IN_R_CPORT(ref Registers.A); break; // IN A, (C)

                case 0x70: IN_CPORT(); break; // IN (C) | UNDOCUMENTED

                // OUT instructions: output on port from register value
                case 0x41: OUT_CPORT_R(ref Registers.B); break; // OUT (C), B
                case 0x49: OUT_CPORT_R(ref Registers.C); break; // OUT (C), C
                case 0x51: OUT_CPORT_R(ref Registers.D); break; // OUT (C), D
                case 0x59: OUT_CPORT_R(ref Registers.E); break; // OUT (C), E
                case 0x61: OUT_CPORT_R(ref Registers.H); break; // OUT (C), H
                case 0x69: OUT_CPORT_R(ref Registers.L); break; // OUT (C), L
                case 0x79: OUT_CPORT_R(ref Registers.A); break; // OUT (C), A

                case 0x71: OUT_CPORT_0(); break;  // OUT (C), 0 | UNDOCUMENTED

                default:
                    _logger.Log(Enums.LogSeverity.Fatal, $"Unrecognized MISC opcode: 0x{_currentInstruction:X2}");
                    Halted = true;
                    break;
            }
        }
    }
}