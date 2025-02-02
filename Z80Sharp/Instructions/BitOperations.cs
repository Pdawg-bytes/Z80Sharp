using Z80Sharp.Enums;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void BIT_B_R([ConstantExpected] byte bit, ref byte operatingRegister)
        {
            byte result = (byte)(operatingRegister & (byte)(1 << bit));

            Registers.SetFlagConditionally(FlagType.Z, result == 0);
            Registers.ClearFlag(FlagType.N);
            Registers.SetFlag(FlagType.H);

            Registers.SetFlagConditionally(FlagType.X, (operatingRegister & 0x08) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (operatingRegister & 0x20) != 0);
            Registers.SetFlagConditionally(FlagType.PV, result == 0);
            Registers.SetFlagConditionally(FlagType.S, bit == 7 && result != 0);
        }
        private void BIT_B_RRMEM([ConstantExpected] byte bit, ref ushort operatingRegister)
        {
            byte result = (byte)(_memory.Read(operatingRegister) & (byte)(1 << bit));

            Registers.SetFlagConditionally(FlagType.Z, result == 0);
            Registers.ClearFlag(FlagType.N);
            Registers.SetFlag(FlagType.H);

            Registers.SetFlagConditionally(FlagType.X, (Registers.MEMPTR & 0x0800) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (Registers.MEMPTR & 0x2000) != 0);
            Registers.SetFlagConditionally(FlagType.PV, result == 0);
            Registers.SetFlagConditionally(FlagType.S, bit == 7 && result != 0);
        }
        private void BIT_B_IRDMEM([ConstantExpected] byte bit, sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            byte result = (byte)(_memory.Read(ird) & (byte)(1 << bit));

            Registers.SetFlagConditionally(FlagType.Z, result == 0);
            Registers.ClearFlag(FlagType.N);
            Registers.SetFlag(FlagType.H);

            Registers.SetFlagConditionally(FlagType.X, (Registers.MEMPTR & 0x0800) != 0);
            Registers.SetFlagConditionally(FlagType.Y, (Registers.MEMPTR & 0x2000) != 0);
            Registers.SetFlagConditionally(FlagType.PV, result == 0);
            Registers.SetFlagConditionally(FlagType.S, bit == 7 && result != 0);
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RES_B_R([ConstantExpected] byte bit, ref byte operatingRegister) => operatingRegister &= (byte)~(1 << bit);

        private void RES_B_RRMEM([ConstantExpected] byte bit, ref ushort operatingRegister) => _memory.Write(operatingRegister, (byte)(_memory.Read(operatingRegister) & (byte)~(1 << bit)));

        private void RES_B_IRDMEM([ConstantExpected] byte bit, sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            _memory.Write(ird, (byte)(_memory.Read(ird) & (byte)~(1 << bit)));
        }

        private void RES_B_IRDMEM_R([ConstantExpected] byte bit, sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            byte result = (byte)(_memory.Read(ird) & (byte)~(1 << bit));
            _memory.Write(ird, result);
            outputRegister = result;
        }


        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void SET_B_R([ConstantExpected] byte bit, ref byte operatingRegister) => operatingRegister |= (byte)(1 << bit);

        private void SET_B_RRMEM([ConstantExpected] byte bit, ref ushort operatingRegister) => _memory.Write(operatingRegister, (byte)(_memory.Read(operatingRegister) | (byte)(1 << bit)));

        private void SET_B_IRDMEM([ConstantExpected] byte bit, sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            _memory.Write(ird, (byte)(_memory.Read(ird) | (byte)(1 << bit)));
        }

        private void SET_B_IRDMEM_R([ConstantExpected] byte bit, sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            Registers.MEMPTR = ird;
            byte result = (byte)(_memory.Read(ird) | (byte)(1 << bit));
            _memory.Write(ird, result);
            outputRegister = result;
        }
    }
}