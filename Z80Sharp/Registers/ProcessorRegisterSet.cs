using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Registers
{
    public unsafe partial struct ProcessorRegisters
    {
        // General-purpose registers
        private ushort _af, _bc, _de, _hl;
        private ushort _af_, _bc_, _de_, _hl_;

        // System registers
        private ushort _sp, _pc;

        // Index registers
        private ushort _ix, _iy;

        // Interrupt and refresh registers
        private byte _i, _r;


        [UnscopedRef] public ref byte I => ref _i;
        [UnscopedRef] public ref byte R => ref _r;

        [UnscopedRef] public ref ushort AF => ref _af;
        [UnscopedRef] public ref byte A => ref Unsafe.Add(ref F, 1);
        [UnscopedRef] public ref byte F => ref Unsafe.As<ushort, byte>(ref _af);

        [UnscopedRef] public ref ushort BC => ref _bc;
        [UnscopedRef] public ref byte C => ref Unsafe.As<ushort, byte>(ref _bc);
        [UnscopedRef] public ref byte B => ref Unsafe.Add(ref C, 1);

        [UnscopedRef] public ref ushort DE => ref _de;
        [UnscopedRef] public ref byte E => ref Unsafe.As<ushort, byte>(ref _de);
        [UnscopedRef] public ref byte D => ref Unsafe.Add(ref E, 1);

        [UnscopedRef] public ref ushort HL => ref _hl;
        [UnscopedRef] public ref byte L => ref Unsafe.As<ushort, byte>(ref _hl);
        [UnscopedRef] public ref byte H => ref Unsafe.Add(ref L, 1);

        [UnscopedRef] public ref ushort AF_ => ref _af_;
        [UnscopedRef] public ref ushort BC_ => ref _bc_;
        [UnscopedRef] public ref ushort DE_ => ref _de_;
        [UnscopedRef] public ref ushort HL_ => ref _hl_;

        [UnscopedRef] public ref ushort SP => ref _sp;
        [UnscopedRef] public ref byte SPlo => ref Unsafe.As<ushort, byte>(ref _sp);
        [UnscopedRef] public ref byte SPhi => ref Unsafe.Add(ref SPlo, 1);

        [UnscopedRef] public ref ushort IX => ref _ix;
        [UnscopedRef] public ref byte IXlo => ref Unsafe.As<ushort, byte>(ref _ix);
        [UnscopedRef] public ref byte IXhi => ref Unsafe.Add(ref IXlo, 1);

        [UnscopedRef] public ref ushort IY => ref _iy;
        [UnscopedRef] public ref byte IYlo => ref Unsafe.As<ushort, byte>(ref _iy);
        [UnscopedRef] public ref byte IYhi => ref Unsafe.Add(ref IYlo, 1);

        [UnscopedRef] public ref ushort PC => ref _pc;
    }
}