﻿using System.Runtime.InteropServices;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Data
{
    public unsafe class Memory : IDisposable
    {
        private byte* _memory;
        private readonly nuint _size;

        public Memory(int size)
        {
            _size = (nuint)size;
            _memory = (byte*)NativeMemory.AllocZeroed(_size);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public byte Read(int address) => *(_memory + address);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Write(int address, byte value) => *(_memory + address) = value;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort ReadWord(int address) => *(ushort*)(_memory + address);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void WriteWord(int address, ushort value) => *(ushort*)(_memory + address) = value;


        public void WriteBytes(int address, byte[] data)
        {
            fixed (byte* src = data)
            {
                Buffer.MemoryCopy(src, _memory + address, (long)(_size - (nuint)address), data.Length);
            }
        }

        public void Clear() => NativeMemory.Clear(_memory, _size);


        public void Dispose()
        {
            if (_memory != null)
            {
                NativeMemory.Free(_memory);
                _memory = null;
            }
        }

        public uint Length => (uint)_size;
    }
}