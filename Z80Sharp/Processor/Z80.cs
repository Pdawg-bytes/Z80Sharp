using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using Z80Sharp.Interfaces;
using Z80Sharp.Memory;
using Z80Sharp.Registers;

namespace Z80Sharp.Processor
{
    /// <summary>
    /// The default implementation of the Z80. Less verbose, more performant.
    /// </summary>
    public class Z80(ushort memSize, IZ80Logger logger) : Z80Base(memSize, logger)
    {
        public override byte Fetch()
        {
            byte val = _memory.Read(Registers.PC);
            Registers.PC++;
            return val;
        }
    }
}
