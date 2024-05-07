using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Interfaces;
using Z80Sharp.Memory;
using Z80Sharp.Registers;

namespace Z80Sharp.Processor
{
    /// <summary>
    /// A less performant but more verbose implementation of the Z80.
    /// </summary>
    public class DebugZ80(ushort memSize, IZ80Logger logger) : Z80Base(memSize, logger)
    {
        public override bool IsDebug() { return true; }

        /// <summary>
        /// Reads current byte at the <see cref="IRegisterSet.PC"/>.
        /// </summary>
        /// <returns>The value at the address.</returns>
        /// <remarks>Overriden to log memory reads.</remarks>
        public override byte Fetch()
        {
            byte ret = _memory.Read(Registers.PC);
            _logger.Log(Enums.LogSeverity.Memory, $"READ at 0x{Registers.PC.ToString("X")} -> 0x{ret.ToString("X")}");
            Registers.PC++;
            return ret;
        }
    }
}
