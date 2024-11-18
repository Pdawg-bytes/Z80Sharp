using Z80Sharp.Enums;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        /// <summary>
        /// Checks the parity of a <see cref="ushort"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the amount of 1s is even, false otherwise.</returns>
        private bool CheckParity(ushort value)
        {
            bool parity = true;
            while (value > 0)
            {
                parity ^= (value & 1) == 1;
                value >>= 1;
            }
            return parity;
        }

        /// <summary>
        /// Sets the flags based on the result of a Rotate operation.
        /// </summary>
        /// <param name="result">The result of the rotation</param>
        private void SetFlagsRotate(byte result)
        {
            Registers.SetFlagConditionally(FlagType.S, (result & 0x80) != 0); // (S) Sign flag
            Registers.SetFlagConditionally(FlagType.Z, result == 0);          // (Z) Zero flag
            Registers.RegisterSet[F] &= (byte)~(FlagType.N | FlagType.H);     // (N, H) Reset unconditionally
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result)); // (PV) Parity flag

            Registers.SetFlagConditionally(FlagType.X, (result & 0x20) != 0); // (X) Bit 5
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x08) != 0); // (Y) Bit 3
        }

        private void SetFlagsLSH(byte result)
        {
            Registers.SetFlagConditionally(FlagType.S, (result & 0x80) != 0); // (S) Sign flag
            Registers.SetFlagConditionally(FlagType.Z, result == 0);          // (Z) Zero flag
            Registers.RegisterSet[F] &= (byte)~(FlagType.N | FlagType.H);     // (N, H) Reset unconditionally
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result)); // (PV) Parity flag

            Registers.SetFlagConditionally(FlagType.X, (result & 0x20) != 0); // (X) Bit 5
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x08) != 0); // (Y) Bit 3
        }
        private void SetFlagsRSH(byte result)
        {
            Registers.SetFlagConditionally(FlagType.S, (result & 0x80) != 0); // (S) Sign flag
            Registers.SetFlagConditionally(FlagType.Z, result == 0);          // (Z) Zero flag
            Registers.RegisterSet[F] &= (byte)~(FlagType.N | FlagType.H);     // (N, H) Reset unconditionally
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result)); // (PV) Parity flag

            Registers.SetFlagConditionally(FlagType.X, (result & 0x20) != 0); // (X) Bit 5
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x08) != 0); // (Y) Bit 3
        }
    }
}