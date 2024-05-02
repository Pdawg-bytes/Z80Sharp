using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Memory;
using Z80Sharp.Registers;

namespace Z80Sharp.Processor
{
    public class Z80
    {
        private MainMemory _memory;
        public ProcessorRegisters Registers;

        public bool Halted { get; set; }

        public Z80(ushort memSize) 
        {
            _memory = new MainMemory(memSize);
        }

        public void Run()
        {

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
        }

        public void Execute()
        {
            // TODO: Interrupt support

            if (Halted) return;
        }
    }
}
