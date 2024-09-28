using Z80Sharp.Enums;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void ExecuteMiscInstruction()
        {
            _logger.Log(Enums.LogSeverity.Decode, $"MISC decoded: 0x{_currentInstruction:X2}");
            switch (Fetch())
            {
                // IMx instructions: change the interrupt mode of the Z80
                case 0x46: IM_M(InterruptMode.IM0); break; // IM0
                case 0x56: IM_M(InterruptMode.IM1); break; // IM1
                case 0x5E: IM_M(InterruptMode.IM2); break; // IM2

                // IN instructions: read from port into register
                case 0x40: IN_R_CPORT(B); break; // IN B, (C)
                case 0x48: IN_R_CPORT(C); break; // IN C, (C)
                case 0x50: IN_R_CPORT(D); break; // IN D, (C)
                case 0x58: IN_R_CPORT(E); break; // IN E, (C)
                case 0x60: IN_R_CPORT(H); break; // IN H, (C)
                case 0x68: IN_R_CPORT(L); break; // IN L, (C)
                case 0x78: IN_R_CPORT(A); break; // IN A, (C)

                // OUT instructions: output on port from register value
                case 0x41: OUT_CPORT_R(B); break; // OUT (C), B
                case 0x49: OUT_CPORT_R(C); break; // OUT (C), C
                case 0x51: OUT_CPORT_R(D); break; // OUT (C), D
                case 0x59: OUT_CPORT_R(E); break; // OUT (C), E
                case 0x61: OUT_CPORT_R(H); break; // OUT (C), H
                case 0x69: OUT_CPORT_R(L); break; // OUT (C), L
                case 0x79: OUT_CPORT_R(A); break; // OUT (C), A

                default:
                    _logger.Log(Enums.LogSeverity.Fatal, $"Unrecognized MISC opcode: 0x{_currentInstruction:X2}");
                    Halted = true;
                    break;
            }
        }
    }
}