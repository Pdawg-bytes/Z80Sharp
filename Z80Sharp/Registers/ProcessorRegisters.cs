using Z80Sharp.Enums;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Registers
{
    public partial struct ProcessorRegisters
    {
        public bool IFF1;
        public bool IFF2;
        public InterruptMode InterruptMode;

        /// <summary>
        /// Increments the refresh register.
        /// </summary>
        /// <remarks>Only the lower 7 bits (bits 0-6) are incremented, while the MSB (bit 7) is preserved.</remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        //internal void IncrementRefresh() => R = (byte)((R & 0x80) | ((R + 1) & 0x7F));
        internal void IncrementRefresh() { }

        /// <summary>
        /// Gets the name of the current interrupt mode.
        /// </summary>
        /// <param name="interruptMode">The interrupt mode the processor is currently in.</param>
        /// <returns>The name, in string form, of the mode.</returns>
        internal string InterruptModeName(InterruptMode interruptMode) => interruptMode switch
        {
            InterruptMode.IM0 => "IM0",
            InterruptMode.IM1 => "IM1",
            InterruptMode.IM2 => "IM2",
            _ => "UNKNOWN INTERRUPT MODE"
        };

        /// <summary>
        /// Checks if any of a flag is set for use in a jump condition.
        /// </summary>
        /// <param name="condition">The flag condition.</param>
        /// <returns>True if the flag condition matches the expected value; false if otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal bool EvaluateJumpFlagCondition([ConstantExpected] byte condition)
        {
            switch (condition & 0xFE) // Mask out LSB to handle paired conditions.
            {
                // ((condition & 1) == 1) checks if the flag should be cleared or set using the LSB of condition.
                // https://www.zilog.com/docs/z80/um0080.pdf page 277 details the condition values used below.
                case 0x00:
                    return IsFlagSet(FlagType.Z) == ((condition & 1) == 1);
                case 0x02:
                    return IsFlagSet(FlagType.C) == ((condition & 1) == 1);
                case 0x04:
                    return IsFlagSet(FlagType.PV) == ((condition & 1) == 1);
                case 0x06:
                    return IsFlagSet(FlagType.S) == ((condition & 1) == 1);
                default:
                    return false;
            }
        }


        #region Register exchange operations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void R8Exchange(ref byte reg1, ref byte reg2)
        {
            byte reg1_old = reg1;
            reg1 = reg2;
            reg2 = reg1_old;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void R16Exchange(ref ushort regPair1, ref ushort regPair2)
        {
            (regPair1, regPair2) = (regPair2, regPair1);
        }
        #endregion


        #region Flags register operations
        /// <summary>
        /// Sets a specified flag by ORing its bits with the <see cref="F"/> register.
        /// </summary>
        /// <param name="flag">The flag to set</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlag(FlagType flag) => F |= (byte)flag;

        /// <summary>
        /// Clears a specified flag by ANDing its complementary bits with the <see cref="F"/> register.
        /// </summary>
        /// <param name="flag">The flag to clear</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearFlag(FlagType flag) => F &= (byte)~flag;

        /// <summary>
        /// Determines whether a specified flag is set in the <see cref="F"/> register.
        /// The method checks this by masking the <see cref="F"/> register with the provided flag.
        /// </summary>
        /// <param name="flag">The flag to check, represented as a <see cref="FlagType"/>.</param>
        /// <returns><c>true</c> if the specified flag is set; otherwise, <c>false</c>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsFlagSet(FlagType flag) => (F & (byte)flag) == (byte)flag;

        /// <summary>
        /// Sets one or more flags in the <see cref="F"/> register by applying a bitmask.
        /// Each bit set to 1 in the mask will set the corresponding flag in the register.
        /// </summary>
        /// <param name="flagMask">A bitmask indicating which flags to set.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlagBits(byte flagMask) => F |= flagMask;

        /// <summary>
        /// Toggles the specified flag in the <see cref="F"/> register.
        /// If the flag is currently set, it will be cleared; if it is cleared, it will be set.
        /// </summary>
        /// <param name="flag">The flag to toggle, represented as a <see cref="FlagType"/>.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvertFlag(FlagType flag) => F ^= (byte)flag;

        /// <summary>
        /// Sets or clears the specified flag in the <see cref="F"/> register based on a condition.
        /// </summary>
        /// <param name="flag">The flag to modify, represented as a <see cref="FlagType"/>.</param>
        /// <param name="condition">
        /// A boolean value indicating whether to set or clear the flag. 
        /// If <c>true</c>, the flag is set; if <c>false</c>, the flag is cleared.
        /// </param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlagConditionally(FlagType flag, bool condition)
        {
            if (condition)
                F |= (byte)flag;
            else
                F &= (byte)~flag;
        }
        #endregion
    }
}