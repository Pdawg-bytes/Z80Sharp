using System.Runtime.InteropServices;

namespace Z80Sharp.Data
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct Instruction
    {
        internal byte Opcode1;
        internal byte Opcode2;
        internal sbyte Opcode3;
        internal byte Opcode4;
    }
}