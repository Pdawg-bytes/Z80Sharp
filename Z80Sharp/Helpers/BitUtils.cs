using System;
using System.Collections.Generic;
using System.Linq;
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
        /// Retrieves the high byte of a <see cref="short"/> value.
        /// </summary>
        /// <param name="value">The <see cref="short"/> to extract the data from.</param>
        /// <returns>The upper 8 bytes of the <see cref="short"/>.</returns>
        public static byte GetUpperByte(this short value) => (byte)(value >> 8);

        /// <summary>
        /// Retrieves the low byte of a <see cref="short"/> value.
        /// </summary>
        /// <param name="value">The <see cref="short"/> to extract the data from.</param>
        /// <returns>The lower 8 bytes of the <see cref="short"/>.</returns>
        public static byte GetLowerByte(this short value) => (byte)(value & 0xFF);

        /// <summary>
        /// Sets the high byte of a <see cref="short"/> value.
        /// </summary>
        /// <param name="value">The full register pair</param>
        /// <param name="input">The value to set the</param>
        /// <returns>A <see cref="short"/> with its upper bytes swapped.</returns>
        public static short SetUpperByte(this short value, byte input) => (short)(ushort)((value & 0x00FF) | (input << 8));
    }
}
