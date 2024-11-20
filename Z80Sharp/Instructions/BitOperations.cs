using System.Diagnostics.CodeAnalysis;
using Z80Sharp.Enums;
using Z80Sharp.Helpers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        private void BIT_B_R([ConstantExpected] byte bit, [ConstantExpected] byte operatingRegister)
        {
            byte result = (byte)(Registers.RegisterSet[operatingRegister] & (byte)(1 << bit));

            Registers.SetFlagConditionally(FlagType.Z, result == 0);    // (Z) (Set if tested bit is 0)
            Registers.ClearFlag(FlagType.N);                            // (N) (Unconditionally reset)
            Registers.SetFlag(FlagType.H);                              // (H) (Unconditionally set)

            Registers.SetFlagConditionally(FlagType.X, (result & 0x08) != 0);       // (X)  (Copy of bit 3)
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x20) != 0);       // (Y)  (Copy of bit 5)
            Registers.SetFlagConditionally(FlagType.PV, result == 0);               // (PV) (Set if tested bit is 0)
            Registers.SetFlagConditionally(FlagType.S, bit == 7 && result != 0);    // (S)  (Set if sign bit is not zero)

            //LogInstructionExec($"0x{_currentInstruction:X2}: BIT {bit}, {Registers.RegisterName(operatingRegister)}");
        }
        private void BIT_B_RRMEM([ConstantExpected] byte bit, [ConstantExpected] byte operatingRegister)
        {
            byte result = (byte)(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)) & (byte)(1 << bit));

            Registers.SetFlagConditionally(FlagType.Z, result == 0);    // (Z) (Set if tested bit is 0)
            Registers.ClearFlag(FlagType.N);                            // (N) (Unconditionally reset)
            Registers.SetFlag(FlagType.H);                              // (H) (Unconditionally set)

            Registers.SetFlagConditionally(FlagType.X, (result & 0x08) != 0);       // (X)  (Copy of bit 3)
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x20) != 0);       // (Y)  (Copy of bit 5)
            Registers.SetFlagConditionally(FlagType.PV, result == 0);               // (PV) (Set if tested bit is 0)
            Registers.SetFlagConditionally(FlagType.S, bit == 7 && result != 0);    // (S)  (Set if sign bit is not zero)

            //LogInstructionExec($"0x{_currentInstruction:X2}: BIT {bit}, ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void BIT_B_IRDMEM([ConstantExpected] byte bit, sbyte displacement, [ConstantExpected] byte indexAddressingMode)
        {
            byte result = (byte)(_memory.Read((ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement)) & (byte)(1 << bit));

            Registers.SetFlagConditionally(FlagType.Z, result == 0);    // (Z) (Set if tested bit is 0)
            Registers.ClearFlag(FlagType.N);                            // (N) (Unconditionally reset)
            Registers.SetFlag(FlagType.H);                              // (H) (Unconditionally set)

            Registers.SetFlagConditionally(FlagType.X, (result & 0x08) != 0);       // (X)  (Copy of bit 3)
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x20) != 0);       // (Y)  (Copy of bit 5)
            Registers.SetFlagConditionally(FlagType.PV, result == 0);               // (PV) (Set if tested bit is 0)
            Registers.SetFlagConditionally(FlagType.S, bit == 7 && result != 0);    // (S)  (Set if sign bit is not zero)

            //LogInstructionExec($"0x{_currentInstruction:X2}: BIT {bit}, ({Registers.RegisterName(indexAddressingMode, true)} + d)");
        }

        private void RES_B_R([ConstantExpected] byte bit, [ConstantExpected] byte operatingRegister)
        {
            Registers.RegisterSet[operatingRegister] &= (byte)~(1 << bit); // Clear bit n of R
            //LogInstructionExec($"0x{_currentInstruction:X2}: RES {bit}, {Registers.RegisterName(operatingRegister)}");
        }
        private void RES_B_RRMEM([ConstantExpected] byte bit, [ConstantExpected] byte operatingRegister)
        {
            ushort ird = (ushort)(Registers.GetR16FromHighIndexer(operatingRegister));
            _memory.Write(ird, (byte)(_memory.Read(ird) & ~(byte)(1 << bit))); // Clear bit n of (RR)
            //LogInstructionExec($"0x{_currentInstruction:X2}: RES {bit}, ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void RES_B_IRDMEM([ConstantExpected] byte bit, sbyte displacement, [ConstantExpected] byte indexAddressingMode)
        {
            ushort ird = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);
            _memory.Write(ird, (byte)(_memory.Read(ird) & ~(byte)(1 << bit))); // Clear bit n of (RR)
            //LogInstructionExec($"0x{_currentInstruction:X2}: RES {bit}, ({Registers.RegisterName(indexAddressingMode, true)})");
        }
        private void RES_B_IRDMEM_R([ConstantExpected] byte bit, sbyte displacement, [ConstantExpected] byte indexAddressingMode, [ConstantExpected] byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);
            byte result = (byte)(_memory.Read(ird) & ~(byte)(1 << bit));
            _memory.Write(ird, result); // Clear bit n of (RR)
            Registers.RegisterSet[outputRegister] = result;
            //LogInstructionExec($"0x{_currentInstruction:X2}: RES {bit}, ({Registers.RegisterName(indexAddressingMode, true)} + d), {Registers.RegisterName(outputRegister)}");
        }

        private void SET_B_R([ConstantExpected] byte bit, [ConstantExpected] byte operatingRegister)
        {
            Registers.RegisterSet[operatingRegister] |= (byte)(1 << bit);
            //LogInstructionExec($"0x{_currentInstruction:X2}: SET {bit}, {Registers.RegisterName(operatingRegister)}");
        }
        private void SET_B_RRMEM([ConstantExpected] byte bit, [ConstantExpected] byte operatingRegister)
        {
            ushort reg = (ushort)(Registers.GetR16FromHighIndexer(operatingRegister));
            _memory.Write(reg, (byte)(_memory.Read(reg) | (byte)(1 << bit)));
            //LogInstructionExec($"0x{_currentInstruction:X2}: SET {bit}, ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void SET_B_IRDMEM([ConstantExpected] byte bit, sbyte displacement, [ConstantExpected] byte indexAddressingMode)
        {
            ushort ird = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);
            _memory.Write(ird, (byte)(_memory.Read(ird) | (byte)(1 << bit)));
            //LogInstructionExec($"0x{_currentInstruction:X2}: SET {bit}, ({Registers.RegisterName(indexAddressingMode, true)})");
        }
        private void SET_B_IRDMEM_R([ConstantExpected] byte bit, sbyte displacement, [ConstantExpected] byte indexAddressingMode, [ConstantExpected] byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);
            byte result = (byte)(_memory.Read(ird) | (byte)(1 << bit));
            _memory.Write(ird, result);
            Registers.RegisterSet[outputRegister] = result;
            //LogInstructionExec($"0x{_currentInstruction:X2}: SET {bit}, ({Registers.RegisterName(indexAddressingMode, true)} + d), {Registers.RegisterName(outputRegister)}");
        }
    }
}
