using Z80Sharp.Enums;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        /// <summary>
        /// Checks the parity of a <see cref="ushort"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the amount of 1s are even, false otherwise.</returns>
        private bool CheckParity(ushort value)
        {
            value ^= (ushort)(value >> 8);
            value ^= (ushort)(value >> 4);
            value ^= (ushort)(value >> 2);
            value ^= (ushort)(value >> 1);

            return ((value & 1) == 0);
        }

        /// <summary>
        /// Sets the N, H, PV, Z, S, X, and Y flags based on the result of a Rotate operation.
        /// </summary>
        /// <param name="result">The result of the rotation</param>
        private void SetFlagsRotate(byte result)
        {
            Registers.SetFlagConditionally(FlagType.S, (result & 0x80) == 1); // (S)    (Set if sign is 1)
            Registers.SetFlagConditionally(FlagType.Z, result == 0);          // (Z)    (Set if result is 0)
            Registers.RegisterSet[F] = (byte)~(FlagType.N | FlagType.H);      // (N, H) (Unconditionally reset)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result)); // (PV)   (Set if bit parity is even)

            Registers.SetFlagConditionally(FlagType.X, (result & 0x20) > 0);  // (X)    (Result Bit 5)
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x08) > 0);  // (Y)    (Result Bit 3)
        }

        private void SetFlagsLSH(byte result)
        {
            Registers.SetFlagConditionally(FlagType.S, (result & 0x80) == 1); // (S)    (Set if sign is 1)
            Registers.SetFlagConditionally(FlagType.Z, result == 0);          // (Z)    (Set if result is 0)
            Registers.RegisterSet[F] = (byte)~(FlagType.N | FlagType.H);      // (N, H) (Unconditionally reset)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result)); // (PV)   (Set if bit parity is even)

            Registers.SetFlagConditionally(FlagType.X, (result & 0x20) > 0);  // (X)    (Result Bit 5)
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x08) > 0);  // (Y)    (Result Bit 3)
        }
        private void SetFlagsRSH(byte result)
        {
            Registers.SetFlagConditionally(FlagType.Z, result == 0);                  // (Z)    (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result));         // (PV)   (Set if bit parity is even)
            Registers.RegisterSet[F] = (byte)~(FlagType.N | FlagType.H | FlagType.S); // (N, H, S) (Unconditionally reset)

            Registers.SetFlagConditionally(FlagType.X, (result & 0x20) > 0);          // (X)    (Result Bit 5)
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x08) > 0);          // (Y)    (Result Bit 3)
        }
    }
}