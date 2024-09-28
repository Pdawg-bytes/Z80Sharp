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
    public partial class Z80
    {
        private void IN_A_NPORT()
        {
            ushort port = (ushort)(Fetch() + (Registers.RegisterSet[A] << 8));
            Registers.RegisterSet[A] = _dataBus.ReadPort(port);
            LogInstructionExec($"0xDB: IN A, (N:0x{port:X4})");
        }
        private void IN_R_CPORT(byte operatingRegister)
        {
            byte data = _dataBus.ReadPort(Registers.BC);
            Registers.RegisterSet[operatingRegister] = data;

            Registers.SetFlagConditionally(FlagType.S, (data & 0x80) != 0);             // (S) (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, data == 0);                      // (Z) (Set if result is zero)
            Registers.SetFlagConditionally(FlagType.PV, FlagHelpers.CheckParity(data)); // (PV) (Set if bit parity is even)
            Registers.SetFlagConditionally(FlagType.X, (data & 0x20) != 0);             // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (data & 0x08) != 0);             // (Y) (Undocumented flag)

            LogInstructionExec($"0x{_currentInstruction}: IN {Registers.RegisterName(operatingRegister)}, (C)");
        }

        private void OUT_NPORT_A()
        {
            ushort port = (ushort)(Fetch() + Registers.RegisterSet[A] << 8);
            _dataBus.WritePort(port, Registers.RegisterSet[A]);
            LogInstructionExec($"0xD3: OUT (N:0x{port:X4}), A");
        }
        private void OUT_CPORT_R(byte operatingRegister)
        {
            _dataBus.WritePort(Registers.BC, Registers.RegisterSet[operatingRegister]);
            LogInstructionExec($"0x{_currentInstruction}: OUT (C), {Registers.RegisterName(operatingRegister)}");
        }
    }
}