using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace Z80Sharp.Helpers
{
    public static class BitUtils
    {
        /*
         * Reference: https://stackoverflow.com/questions/5419453/getting-upper-and-lower-byte-of-an-integer-in-c-sharp-and-putting-it-as-a-char-a 
         */

        /// <summary>
        /// Retrieves the high byte of a <see cref="ushort"/> value.
        /// </summary>
        /// <param name="value">The <see cref="ushort"/> to extract the data from.</param>
        /// <returns>The upper 8 bits of the <see cref="ushort"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetUpperByte(this ushort value) => (byte)(value >> 8);

        /// <summary>
        /// Retrieves the low byte of a <see cref="ushort"/> value.
        /// </summary>
        /// <param name="value">The <see cref="ushort"/> to extract the data from.</param>
        /// <returns>The lower 8 bits of the <see cref="ushort"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static byte GetLowerByte(this ushort value) => (byte)(value & 0xFF);

        /// <summary>
        /// Sets the high byte of a <see cref="ushort"/> value.
        /// </summary>
        /// <param name="value">The <see cref="ushort"/> to extract the data from.</param>
        /// <param name="input">The value to set the upper byte to.</param>
        /// <returns>A <see cref="ushort"/> with its upper byte swapped.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SetUpperByte(this ushort value, byte input) => (ushort)((value & 0xFF00) | input);

        /// <summary>
        /// Sets the high byte of a <see cref="short"/> value.
        /// </summary>
        /// <param name="value">The <see cref="ushort"/> to extract the data from.</param>
        /// <param name="input">The value to set the lower byte to.</param>
        /// <returns>A <see cref="ushort"/> with its lower byte swapped.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ushort SetLowerByte(this ushort value, byte input) => (ushort)((value & 0x00FF) | (input << 8));
    }
}
