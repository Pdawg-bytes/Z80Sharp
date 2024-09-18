using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Helpers;

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
        public void Write(ushort address, byte value) => _memory[address] = value;

        public ushort ReadWord(ushort address) => (ushort)(Read(address) | (Read((ushort)(address + 1)) << 8));
        public void WriteWord(ushort address, ushort value) { Write(address, value.GetLowerByte()); Write((ushort)(address + 1), value.GetUpperByte()); }

        public ushort Length { get => (ushort)_memory.Length; }
    }
}
