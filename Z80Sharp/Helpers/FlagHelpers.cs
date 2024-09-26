using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80Sharp.Helpers
{
    internal static class FlagHelpers
    {
        /// <summary>
        /// Checks the parity of a <see cref="ushort"/>.
        /// </summary>
        /// <param name="value">The value to check.</param>
        /// <returns>True if the amount of 1s are even, false otherwise.</returns>
        internal static bool CheckParity(ushort value)
        {
            value ^= (ushort)(value >> 8);
            value ^= (ushort)(value >> 4);
            value ^= (ushort)(value >> 2);
            value ^= (ushort)(value >> 1);

            return ((value & 1) == 0);
        }
    }
}