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
        private ProcessorRegisters _registers;

        public Z80(ushort memSize) 
        {
            _memory = new MainMemory(memSize);
        }

        public void Run()
        {
            Reset();
            while (true)
            {
                byte opcode = _memory.Read(_registers.PC);
                _registers.PC++;

                switch (opcode)
                {
                    default:
                        return;
                }
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

            _registers.IFF1 = false;
            _registers.IFF2 = false;
            _registers.PC = 0;
            _registers.A = 0xFF;
        }
    }
}
