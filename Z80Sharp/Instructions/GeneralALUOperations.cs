using Z80Sharp.Enums;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        // Reference: https://stackoverflow.com/questions/8119577/z80-daa-instruction
        // I copied this code...I wish there was a better way to do this while keeping it 100% accurate.
        private void DAA()
        {
            int t = 0;

            if (Registers.IsFlagSet(FlagType.H) || ((Registers.A & 0xF) > 9)) t++;
            if (Registers.IsFlagSet(FlagType.C) || (Registers.A > 0x99)) { t += 2; Registers.SetFlag(FlagType.C); }

            if (Registers.IsFlagSet(FlagType.N) && !Registers.IsFlagSet(FlagType.H)) Registers.ClearFlag(FlagType.H);
            else
            {
                if (Registers.IsFlagSet(FlagType.N) && Registers.IsFlagSet(FlagType.H))
                    Registers.SetFlagConditionally(FlagType.H, (Registers.A & 0x0F) < 6);
                else
                    Registers.SetFlagConditionally(FlagType.H, (Registers.A & 0x0F) >= 0x0A);
            }

            switch (t)
            {
                case 1:
                    Registers.A += Registers.IsFlagSet(FlagType.N) ? (byte)0xFA : (byte)0x06; // -6:6
                    break;
                case 2:
                    Registers.A += Registers.IsFlagSet(FlagType.N) ? (byte)0xA0 : (byte)0x60; // -0x60:0x60
                    break;
                case 3:
                    Registers.A += Registers.IsFlagSet(FlagType.N) ? (byte)0x9A : (byte)0x66; // -0x66:0x66
                    break;
            }

            Registers.SetFlagConditionally(FlagType.S, (Registers.A & 0x80) != 0);
            Registers.SetFlagConditionally(FlagType.Z, Registers.A == 0);
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(Registers.A));

            Registers.SetFlagConditionally(FlagType.X, (Registers.A & 0x08) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (Registers.A & 0x20) != 0);
        }


        private void NEG()
        {
            byte value = Registers.A;
            int result = 0 - value;
            Registers.A = (byte)result;

            Registers.F = (byte)(0xA8 & result);
            Registers.SetFlagConditionally(FlagType.Z, Registers.A == 0);
            Registers.SetFlagConditionally(FlagType.H, (value & 0x0F) != 0);
            Registers.SetFlagConditionally(FlagType.PV, value == 0x80);
            Registers.SetFlagConditionally(FlagType.C, value != 0);
            Registers.SetFlag(FlagType.N);
        }


        private void CPL()
        {
            Registers.A = (byte)~Registers.A;
            Registers.SetFlag(FlagType.N);
            Registers.SetFlag(FlagType.H);
            Registers.SetFlagConditionally(FlagType.X, (Registers.A & 0x08) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (Registers.A & 0x20) != 0);
        }


        private void CCF()
        {
            Registers.SetFlagConditionally(FlagType.H, Registers.IsFlagSet(FlagType.C));
            Registers.InvertFlag(FlagType.C);
            Registers.ClearFlag(FlagType.N);
            Registers.SetFlagConditionally(FlagType.X, (Registers.A & 0x08) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (Registers.A & 0x20) != 0);
        }

        private void SCF()
        {
            Registers.SetFlag(FlagType.C);
            Registers.ClearFlag(FlagType.N);
            Registers.ClearFlag(FlagType.H);
            Registers.SetFlagConditionally(FlagType.X, (Registers.A & 0x08) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (Registers.A & 0x20) != 0);
        }
    }
}