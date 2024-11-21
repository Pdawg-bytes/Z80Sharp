using System.Diagnostics.CodeAnalysis;
using Z80Sharp.Enums;
using Z80Sharp.Helpers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        private void BIT_B_R([ConstantExpected] byte bit, ref byte operatingRegister)
        {
            byte result = (byte)(operatingRegister & (byte)(1 << bit));

            Registers.SetFlagConditionally(FlagType.Z, result == 0);    // (Z) (Set if tested bit is 0)
            Registers.ClearFlag(FlagType.N);                            // (N) (Unconditionally reset)
            Registers.SetFlag(FlagType.H);                              // (H) (Unconditionally set)

            Registers.SetFlagConditionally(FlagType.X, (result & 0x08) != 0);       // (X)  (Copy of bit 3)
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x20) != 0);       // (Y)  (Copy of bit 5)
            Registers.SetFlagConditionally(FlagType.PV, result == 0);               // (PV) (Set if tested bit is 0)
            Registers.SetFlagConditionally(FlagType.S, bit == 7 && result != 0);    // (S)  (Set if sign bit is not zero)

            LogInstructionExec($"0x{_currentInstruction:X2}: BIT {bit}, R");
        }
        private void BIT_B_RRMEM([ConstantExpected] byte bit, ref ushort operatingRegister)
        {
            byte result = (byte)(_memory.Read(operatingRegister) & (byte)(1 << bit));

            Registers.SetFlagConditionally(FlagType.Z, result == 0);    // (Z) (Set if tested bit is 0)
            Registers.ClearFlag(FlagType.N);                            // (N) (Unconditionally reset)
            Registers.SetFlag(FlagType.H);                              // (H) (Unconditionally set)

            Registers.SetFlagConditionally(FlagType.X, (result & 0x08) != 0);       // (X)  (Copy of bit 3)
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x20) != 0);       // (Y)  (Copy of bit 5)
            Registers.SetFlagConditionally(FlagType.PV, result == 0);               // (PV) (Set if tested bit is 0)
            Registers.SetFlagConditionally(FlagType.S, bit == 7 && result != 0);    // (S)  (Set if sign bit is not zero)

            LogInstructionExec($"0x{_currentInstruction:X2}: BIT {bit}, (RR)");
        }
        private void BIT_B_IRDMEM([ConstantExpected] byte bit, sbyte displacement, ref ushort indexAddressingMode)
        {
            byte result = (byte)(_memory.Read((ushort)(indexAddressingMode + displacement)) & (byte)(1 << bit));

            Registers.SetFlagConditionally(FlagType.Z, result == 0);    // (Z) (Set if tested bit is 0)
            Registers.ClearFlag(FlagType.N);                            // (N) (Unconditionally reset)
            Registers.SetFlag(FlagType.H);                              // (H) (Unconditionally set)

            Registers.SetFlagConditionally(FlagType.X, (result & 0x08) != 0);       // (X)  (Copy of bit 3)
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x20) != 0);       // (Y)  (Copy of bit 5)
            Registers.SetFlagConditionally(FlagType.PV, result == 0);               // (PV) (Set if tested bit is 0)
            Registers.SetFlagConditionally(FlagType.S, bit == 7 && result != 0);    // (S)  (Set if sign bit is not zero)

            LogInstructionExec($"0x{_currentInstruction:X2}: BIT {bit}, (IR + d)");
        }

        private void RES_B_R([ConstantExpected] byte bit, ref byte operatingRegister)
        {
            operatingRegister &= (byte)~(1 << bit); // Clear bit n of R
            LogInstructionExec($"0x{_currentInstruction:X2}: RES {bit}, R");
        }
        private void RES_B_RRMEM([ConstantExpected] byte bit, ref ushort operatingRegister)
        {
            _memory.Write(operatingRegister, (byte)(_memory.Read(operatingRegister) & (byte)~(1 << bit))); // Clear bit n of (RR)
            LogInstructionExec($"0x{_currentInstruction:X2}: RES {bit}, (RR)");
        }
        private void RES_B_IRDMEM([ConstantExpected] byte bit, sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            _memory.Write(ird, (byte)(_memory.Read(ird) & (byte)~(1 << bit))); // Clear bit n of (RR)
            LogInstructionExec($"0x{_currentInstruction:X2}: RES {bit}, (IR + d)");
        }
        private void RES_B_IRDMEM_R([ConstantExpected] byte bit, sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            byte result = (byte)(_memory.Read(ird) & (byte)~(1 << bit));
            _memory.Write(ird, result); // Clear bit n of (RR)
            outputRegister = result;
            LogInstructionExec($"0x{_currentInstruction:X2}: RES {bit}, (IR + d), R");
        }

        private void SET_B_R([ConstantExpected] byte bit, ref byte operatingRegister)
        {
            operatingRegister |= (byte)(1 << bit);
            LogInstructionExec($"0x{_currentInstruction:X2}: SET {bit}, R");
        }
        private void SET_B_RRMEM([ConstantExpected] byte bit, ref ushort operatingRegister)
        {
            _memory.Write(operatingRegister, (byte)(_memory.Read(operatingRegister) | (byte)(1 << bit)));
            LogInstructionExec($"0x{_currentInstruction:X2}: SET {bit}, (RR)");
        }
        private void SET_B_IRDMEM([ConstantExpected] byte bit, sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            _memory.Write(ird, (byte)(_memory.Read(ird) | (byte)(1 << bit)));
            LogInstructionExec($"0x{_currentInstruction:X2}: SET {bit}, (IR + d)");
        }
        private void SET_B_IRDMEM_R([ConstantExpected] byte bit, sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);
            byte result = (byte)(_memory.Read(ird) | (byte)(1 << bit));
            _memory.Write(ird, result);
            outputRegister = result;
            LogInstructionExec($"0x{_currentInstruction:X2}: SET {bit}, (IR + d), R");
        }
    }
}
