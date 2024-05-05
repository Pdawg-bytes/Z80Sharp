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
    public class DebugZ80 : IProcessor
    {
        private MainMemory _memory;
        private readonly IZ80Logger _logger;

        public IRegisterSet Registers { get; set; } = new ProcessorRegisters();

        public bool Halted { get; set; }

        public DebugZ80(ushort memSize, IZ80Logger logger)
        {
            _memory = new MainMemory(memSize);
            _logger = logger;
        }

        public void Run()
        {
            for (ushort i = 0; i < _memory.Length; i++)
            {
                Fetch();
            }
        }

        public void Stop()
        {

        }

        public void Reset()
        {
            for (ushort i = 0; i < _memory.Length; i++)
            {
                _memory.Write(i, 0x00);
            }

            Registers.IFF1 = false;
            Registers.IFF2 = false;
            Registers.PC = 0;
            Registers.A = 0xFF;
            Registers.SP = 0xFFFF;
            Registers.F = 0xFF;
        }

        public void ExecuteStep()
        {
            // TODO: Interrupt support

            if (Halted) return;
        }

        /// <summary>
        /// Reads current byte at the <see cref="IRegisterSet.PC"/>.
        /// </summary>
        /// <returns>The value at the address.</returns>
        private byte Fetch()
        {
            byte ret = _memory.Read(Registers.PC);
            _logger.Log(Enums.LogSeverity.Memory, $"READ at 0x{Registers.PC.ToString("X")} -> {ret.ToString("X")}");

            Registers.PC++;
            return ret;
        }
    }
}
