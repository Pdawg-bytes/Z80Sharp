using Z80Sharp.Helpers;
using Z80Sharp.Interfaces;
using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Memory
{
    public unsafe class MainMemory : IMemory
    {
        private GCHandle _memHandle;
        private byte* pMem;
        public byte[] _memory;

        public MainMemory(int size) 
        {
            _memory = new byte[size];
            _memHandle = GCHandle.Alloc(_memory, GCHandleType.Pinned);
            pMem = (byte*)_memHandle.AddrOfPinnedObject();
        }
        ~MainMemory()
        {
            _memHandle.Free();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Read(ushort address) => pMem[address];
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(ushort address, byte value) => pMem[address] = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadWord(ushort address) => (ushort)(Read(address) | (Read((ushort)(address + 1)) << 8));
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWord(ushort address, ushort value) { Write(address, value.GetLowerByte()); Write((ushort)(address + 1), value.GetUpperByte()); }

        public uint Length { get => (uint)_memory.Length; }
    }
}