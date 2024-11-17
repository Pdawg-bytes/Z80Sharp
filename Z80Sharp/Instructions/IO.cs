using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using Z80Sharp.Helpers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public sealed partial class Z80
    {
        private void IN_A_NPORT()
        {
            ushort port = (ushort)(Fetch() + (Registers.RegisterSet[A] << 8));
            Registers.RegisterSet[A] = _dataBus.ReadPort(port);
            //LogInstructionExec($"0xDB: IN A, (N:0x{port:X4})");
        }
        private void IN_R_CPORT(byte operatingRegister)
        {
            byte data = _dataBus.ReadPort(Registers.BC);
            Registers.RegisterSet[operatingRegister] = data;

            Registers.SetFlagConditionally(FlagType.S, (data & 0x80) != 0);             // (S) (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, data == 0);                      // (Z) (Set if result is zero)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(data)); // (PV) (Set if bit parity is even)
            Registers.SetFlagConditionally(FlagType.X, (data & 0x20) != 0);             // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (data & 0x08) != 0);             // (Y) (Undocumented flag)

            //LogInstructionExec($"0x{_currentInstruction:X2}: IN {Registers.RegisterName(operatingRegister)}, (C)");
        }
        private void IN_CPORT() // UNDOCUMENTED
        {
            byte data = _dataBus.ReadPort(Registers.BC);
            Registers.RegisterSet[F] &= (byte)~(FlagType.N | FlagType.H);
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(data));
            //LogInstructionExec("0x70: IN (C)");
        }

        private void INI()
        {
            byte data = _dataBus.ReadPort(Registers.BC);
            _memory.Write(Registers.HL++, data);
            byte regB = Registers.RegisterSet[B]--;

            Registers.SetFlag(FlagType.N);
            Registers.SetFlagConditionally(FlagType.Z, regB == 0);
            //LogInstructionExec("0xA2: INI");
        }
        private void INIR()
        {
            INIR();

            if (Registers.RegisterSet[B] != 0)
            {
                Registers.PC -= 2;
            }

            //LogInstructionExec("0xA2: INI");
        }
        private void IND()
        {
            byte data = _dataBus.ReadPort(Registers.BC);
            _memory.Write(Registers.HL--, data);
            byte regB = Registers.RegisterSet[B]--;

            Registers.SetFlag(FlagType.N);
            Registers.SetFlagConditionally(FlagType.Z, regB == 0);
            //LogInstructionExec("0xAA: IND");
        }
        private void INDR()
        {
            IND();

            if (Registers.RegisterSet[B] != 0)
            {
                Registers.PC -= 2;
            }

            //LogInstructionExec("0xBA: INDR");
        }

        private void OUT_NPORT_A()
        {
            ushort port = (ushort)((Registers.RegisterSet[A] << 8) | Fetch());
            _dataBus.WritePort(port, Registers.RegisterSet[A]);
            //LogInstructionExec($"0xD3: OUT (N:0x{(port & 0x00FF):X2}), A");
        }
        private void OUT_CPORT_R(byte operatingRegister)
        {
            _dataBus.WritePort(Registers.BC, Registers.RegisterSet[operatingRegister]);
            //LogInstructionExec($"0x{_currentInstruction:X2}: OUT (C), {Registers.RegisterName(operatingRegister)}");
        }
        private void OUT_CPORT_0()
        {
            _dataBus.WritePort(Registers.BC, 0); // Should be 255 on a CMOS Z80, 0 on NMOS
            //LogInstructionExec("0x71: OUT (C), 0");
        }

        private void OUTI()
        {
            byte hlMem = _memory.Read(Registers.HL++);
            _dataBus.WritePort(Registers.BC, hlMem);
            byte regB = Registers.RegisterSet[B]--;

            Registers.SetFlag(FlagType.N);
            Registers.SetFlagConditionally(FlagType.Z, regB == 0);
            //LogInstructionExec("0xA3: OUTI");
        }
        private void OTIR()
        {
            OUTI();

            if (Registers.RegisterSet[B] != 0)
            {
                Registers.PC -= 2;
            }

            //LogInstructionExec("0xB3: OTIR");
        }
        private void OUTD()
        {
            byte hlMem = _memory.Read(Registers.HL--);
            _dataBus.WritePort(Registers.BC, hlMem);
            byte regB = Registers.RegisterSet[B]--;

            Registers.SetFlag(FlagType.N);
            Registers.SetFlagConditionally(FlagType.Z, regB == 0);
            //LogInstructionExec("0xAB: OUTD");
        }
        private void OTDR()
        {
            OUTD();

            if (Registers.RegisterSet[B] != 0)
            {
                Registers.PC -= 2;
            }

            //LogInstructionExec("0xBB: OTDR");
        }
    }
}