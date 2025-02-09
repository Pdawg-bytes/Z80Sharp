using System.Runtime.CompilerServices;
using Z80Sharp.Enums;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void IN_A_NPORT()
        {
            ushort port = (ushort)((Registers.A << 8) + Fetch());
            Registers.MEMPTR = (ushort)(port + 1);
            Registers.A = _dataBus.ReadPort(port);
        }

        private void IN_R_CPORT(ref byte operatingRegister)
        {
            byte data = _dataBus.ReadPort(Registers.BC);
            Registers.MEMPTR = (ushort)(Registers.BC + 1);
            operatingRegister = data;

            Registers.F = (byte)(Registers.F & (byte)FlagType.C);
            Registers.F |= (byte)(0xA8 & data);
            Registers.SetFlagConditionally(FlagType.Z, data == 0);
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(data));
        }
        private void IN_CPORT()
        {
            byte data = _dataBus.ReadPort(Registers.BC);
            Registers.MEMPTR = (ushort)(Registers.BC + 1);

            Registers.F = (byte)(Registers.F & (byte)FlagType.C);
            Registers.F |= (byte)(0xA8 & data);
            Registers.SetFlagConditionally(FlagType.Z, data == 0);
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(data));
            Registers.ClearFlag(FlagType.H);
            Registers.ClearFlag(FlagType.N);
        }

        // Reimplementation of redcode's INXR.
        private void INBlock(bool increment, bool repeat)
        {
            byte io = _dataBus.ReadPort(Registers.BC);
            uint temp = (uint)(io + (byte)(Registers.C + (increment ? 1 : -1)));

            _memory.Write(increment ? Registers.HL++ : Registers.HL--, io);
            Registers.MEMPTR = (ushort)(Registers.BC + (ushort)(increment ? 1 : -1));
            byte regB = --Registers.B;

            byte hcf = (temp > 255) ? (byte)(FlagType.H | FlagType.C) : (byte)0;
            byte p = (byte)((temp & 0x07) ^ regB);
            byte nf = (byte)((io >> 6) & (byte)FlagType.N);

            if (repeat && Registers.B != 0)
            {
                UpdateCommonInxrOtxrFlags(regB, hcf, p, nf);
                Registers.PC -= 2;
                Registers.MEMPTR = (ushort)(Registers.PC + 1);
                _clock.LastOperationStatus = true;
                return;
            }

            Registers.F = (byte)(0xA8 & regB);
            Registers.SetFlagConditionally(FlagType.Z, regB == 0);
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(p));
            Registers.SetFlagBits((byte)(hcf | nf));

            _clock.LastOperationStatus = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void INI() => INBlock(true, false);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void INIR() => INBlock(true, true);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void IND() => INBlock(false, false);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void INDR() => INBlock(false, true);


        private void OUT_NPORT_A()
        {
            ushort port = (ushort)((Registers.A << 8) | Fetch());
            Registers.MEMPTR = (ushort)((Registers.A << 8) + ((port + 1) & 0xFF));
            _dataBus.WritePort(port, Registers.A);
        }

        private void OUT_CPORT_R(ref byte operatingRegister)
        {
            Registers.MEMPTR = (ushort)(Registers.BC + 1);
            _dataBus.WritePort(Registers.BC, operatingRegister);
        }

        private void OUT_CPORT_0()
        {
            Registers.MEMPTR = (ushort)(Registers.BC + 1);
            _dataBus.WritePort(Registers.BC, 0x00); // Should be 255 on a CMOS Z80, 0 on NMOS
        }

        // Reimplementation of redcode's OTXR.
        private void OUTBlock(bool increment, bool repeat)
        {
            byte io = _memory.Read(increment ? Registers.HL++ : Registers.HL--);
            uint temp = (uint)(io + Registers.L);

            _dataBus.WritePort(Registers.BC, io);
            Registers.MEMPTR = (ushort)(Registers.BC + (ushort)(increment ? 1 : -1));
            byte regB = --Registers.B;

            byte hcf = (temp > 255) ? (byte)(FlagType.H | FlagType.C) : (byte)0;
            byte p = (byte)((temp & 0x07) ^ regB);
            byte nf = (byte)((io >> 6) & (byte)FlagType.N);

            if (repeat && Registers.B != 0)
            {
                UpdateCommonInxrOtxrFlags(regB, hcf, p, nf);
                Registers.PC -= 2;
                Registers.MEMPTR = (ushort)(Registers.PC + 1);
                _clock.LastOperationStatus = true;
                return;
            }

            Registers.F = (byte)(0xA8 & regB);
            Registers.SetFlagConditionally(FlagType.Z, regB == 0);
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(p));
            Registers.SetFlagBits((byte)(hcf | nf));

            _clock.LastOperationStatus = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OUTI() => OUTBlock (true, false);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OTIR() => OUTBlock(true, true);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OUTD() => OUTBlock(false, false);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void OTDR() => OUTBlock(false, true);
    }
}