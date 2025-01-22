using Z80Sharp.Enums;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        private void IN_A_NPORT() => Registers.A = _dataBus.ReadPort((ushort)(Fetch() + (Registers.A << 8)));

        private void IN_R_CPORT(ref byte operatingRegister)
        {
            byte data = _dataBus.ReadPort(Registers.BC);
            operatingRegister = data;

            Registers.SetFlagConditionally(FlagType.S, (data & 0x80) != 0); // (S) (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, data == 0);          // (Z) (Set if result is zero)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(data)); // (PV) (Set if bit parity is even)
            Registers.SetFlagConditionally(FlagType.X, (data & 0x20) != 0); // (X) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (data & 0x08) != 0); // (Y) (Undocumented flag)
        }
        private void IN_CPORT()
        {
            byte data = _dataBus.ReadPort(Registers.BC);
            Registers.F &= (byte)~(FlagType.N | FlagType.H);
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(data));
        }

        private void INI()
        {
            byte data = _dataBus.ReadPort(Registers.BC);
            _memory.Write(Registers.HL++, data);
            byte regB = Registers.B--;

            Registers.SetFlag(FlagType.N);
            Registers.SetFlagConditionally(FlagType.Z, regB == 0);
        }
        private void INIR()
        {
            INI();
            if (Registers.B != 0)
            {
                Registers.PC -= 2;
                _clock.LastOperationStatus = false;
                return;
            }
            _clock.LastOperationStatus = true;
        }
        private void IND()
        {
            byte data = _dataBus.ReadPort(Registers.BC);
            _memory.Write(Registers.HL--, data);
            byte regB = Registers.B--;

            Registers.SetFlag(FlagType.N);
            Registers.SetFlagConditionally(FlagType.Z, regB == 0);
        }
        private void INDR()
        {
            IND();
            if (Registers.B != 0)
            {
                Registers.PC -= 2;
                _clock.LastOperationStatus = false;
                return;
            }
            _clock.LastOperationStatus = true;
        }


        private void OUT_NPORT_A()
        {
            ushort port = (ushort)((Registers.A << 8) | Fetch());
            _dataBus.WritePort(port, Registers.A);
        }

        private void OUT_CPORT_R(ref byte operatingRegister) => _dataBus.WritePort(Registers.BC, operatingRegister);
        
        private void OUT_CPORT_0() => _dataBus.WritePort(Registers.BC, 0xFF); // Should be 255 on a CMOS Z80, 0 on NMOS

        private void OUTI()
        {
            byte hlMem = _memory.Read(Registers.HL++);
            _dataBus.WritePort(Registers.BC, hlMem);
            byte regB = Registers.B--;

            Registers.SetFlag(FlagType.N);
            Registers.SetFlagConditionally(FlagType.Z, regB == 0);
        }
        private void OTIR()
        {
            OUTI();
            if (Registers.B != 0)
            {
                Registers.PC -= 2;
                _clock.LastOperationStatus = false;
                return;
            }
            _clock.LastOperationStatus = true;
        }
        private void OUTD()
        {
            byte hlMem = _memory.Read(Registers.HL--);
            _dataBus.WritePort(Registers.BC, hlMem);
            byte regB = Registers.B--;

            Registers.SetFlag(FlagType.N);
            Registers.SetFlagConditionally(FlagType.Z, regB == 0);
        }
        private void OTDR()
        {
            OUTD();
            if (Registers.B != 0)
            {
                Registers.PC -= 2;
                _clock.LastOperationStatus = false;
                return;
            }
            _clock.LastOperationStatus = true;
        }
    }
}