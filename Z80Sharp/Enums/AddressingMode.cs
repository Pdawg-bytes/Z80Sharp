using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80Sharp.Enums
{
    /// <summary>
    /// Defines a list of the available addressing modes used across bit instructions.
    /// </summary>
    public enum AddressingMode : byte
    {
        Regular = 0,
        IndexX = 18,
        IndexY = 20
    }
}