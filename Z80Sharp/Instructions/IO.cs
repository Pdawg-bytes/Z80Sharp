﻿using System.Runtime.CompilerServices;
using Z80Sharp.Enums;
using static System.Runtime.InteropServices.JavaScript.JSType;

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
            Registers.SetFlagConditionally(FlagType.Y, (data & 0x20) != 0); // (Y) (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.X, (data & 0x08) != 0); // (X) (Undocumented flag)
        }
        private void IN_CPORT()
        {
            byte data = _dataBus.ReadPort(Registers.BC);
            Registers.F &= (byte)~(FlagType.N | FlagType.H);
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(data));
        }

        private void INBlock(bool increment, bool repeat)
        {
            byte data = _dataBus.ReadPort(Registers.BC);
            _memory.Write(increment ? Registers.HL++ : Registers.HL--, data);
            byte temp = (byte)(Registers.C + data + (increment ? 1 : -1));

            byte regB = --Registers.B;

            Registers.F = 0;
            Registers.F |= (byte)((byte)(FlagType.S | FlagType.Y | FlagType.X) & regB);
            Registers.SetFlagConditionally(FlagType.Z, regB == 0);
            Registers.F |= temp < data ? (byte)(FlagType.H | FlagType.C) : (byte)0;
            Registers.SetFlagConditionally(FlagType.PV, CheckParity((byte)((temp & 0x07) ^ regB)));
            Registers.SetFlagConditionally(FlagType.N, (data & 0x80) != 0);

            Registers.SetFlagConditionally(FlagType.X, (regB & 0x08) != 0); // (X) (copy of bit 3)
            Registers.SetFlagConditionally(FlagType.Y, (regB & 0x20) != 0); // (Y) (copy of bit 5)

            if (repeat && Registers.B != 0)
            {
                Registers.PC -= 2;
                _clock.LastOperationStatus = true;
                return;
            }
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
            _dataBus.WritePort(port, Registers.A);
        }

        private void OUT_CPORT_R(ref byte operatingRegister) => _dataBus.WritePort(Registers.BC, operatingRegister);
        
        private void OUT_CPORT_0() => _dataBus.WritePort(Registers.BC, 0x00); // Should be 255 on a CMOS Z80, 0 on NMOS

        private void OUTBlock(bool increment, bool repeat)
        {
            byte hlMem = _memory.Read(increment ? Registers.HL++ : Registers.HL--);
            _dataBus.WritePort(Registers.BC, hlMem);
            byte temp = (byte)(Registers.L + hlMem);

            byte regB = --Registers.B;

            Registers.F = 0;
            Registers.F |= (byte)((byte)(FlagType.S | FlagType.Y | FlagType.X) & regB);
            Registers.SetFlagConditionally(FlagType.Z, regB == 0);
            Registers.F |= temp < hlMem ? (byte)(FlagType.H | FlagType.C) : (byte)0;
            Registers.SetFlagConditionally(FlagType.PV, CheckParity((byte)((temp & 0x07) ^ regB)));
            Registers.SetFlagConditionally(FlagType.N, (hlMem & 0x80) != 0);

            Registers.SetFlagConditionally(FlagType.X, (regB & 0x08) != 0); // (X) (copy of bit 3)
            Registers.SetFlagConditionally(FlagType.Y, (regB & 0x20) != 0); // (Y) (copy of bit 5)

            if (repeat && Registers.B != 0)
            {
                Registers.PC -= 2;
                _clock.LastOperationStatus = true;
                return;
            }
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