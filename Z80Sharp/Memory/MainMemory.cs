using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Z80Sharp.Memory
{
    public class MainMemory
    {
        private byte[] _memory { get; set; }

        public MainMemory(ushort size) 
        {
            _memory = new byte[size];
        }

        public byte Read(ushort address) => _memory[address];
        public byte Write(ushort address, byte value) => _memory[address] = value;

        //public ushort ReadWord(ushort address) => (ushort) (Read(address) | (Read((ushort)(address + 1)) << 8));

        public ushort Length { get => (ushort)_memory.Length; }
    }
}
