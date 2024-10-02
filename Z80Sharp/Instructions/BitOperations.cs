using Z80Sharp.Enums;
using Z80Sharp.Helpers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void BIT_B_R(byte bit, byte operatingRegister)
        {
            byte result = (byte)(Registers.RegisterSet[operatingRegister] & (byte)(1 << bit));

            Registers.SetFlagConditionally(FlagType.Z, result == 0);    // (Z) (Set if tested bit is 0)
            Registers.ClearFlag(FlagType.N);                            // (N) (Unconditionally reset)
            Registers.SetFlag(FlagType.H);                              // (H) (Unconditionally set)

            Registers.SetFlagConditionally(FlagType.X, (result & 0x08) != 0);       // (X)  (Copy of bit 3)
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x20) != 0);       // (Y)  (Copy of bit 5)
            Registers.SetFlagConditionally(FlagType.PV, result == 0);               // (PV) (Set if tested bit is 0)
            Registers.SetFlagConditionally(FlagType.S, bit == 7 && result != 0);    // (S)  (Set if sign bit is not zero)

            LogInstructionExec($"0x{_currentInstruction:X2}: BIT {bit}, {Registers.RegisterName(operatingRegister)}");
        }
        private void BIT_B_RRMEM(byte bit, byte operatingRegister)
        {
            byte result = (byte)(_memory.Read(Registers.GetR16FromHighIndexer(operatingRegister)) & (byte)(1 << bit));

            Registers.SetFlagConditionally(FlagType.Z, result == 0);    // (Z) (Set if tested bit is 0)
            Registers.ClearFlag(FlagType.N);                            // (N) (Unconditionally reset)
            Registers.SetFlag(FlagType.H);                              // (H) (Unconditionally set)

            Registers.SetFlagConditionally(FlagType.X, (result & 0x08) != 0);       // (X)  (Copy of bit 3)
            Registers.SetFlagConditionally(FlagType.Y, (result & 0x20) != 0);       // (Y)  (Copy of bit 5)
            Registers.SetFlagConditionally(FlagType.PV, result == 0);               // (PV) (Set if tested bit is 0)
            Registers.SetFlagConditionally(FlagType.S, bit == 7 && result != 0);    // (S)  (Set if sign bit is not zero)

            LogInstructionExec($"0x{_currentInstruction:X2}: BIT {bit}, ({Registers.RegisterName(operatingRegister, true)})");
        }

        private void RES_B_R(byte bit, byte operatingRegister)
        {
            Registers.RegisterSet[operatingRegister] = (byte)(Registers.RegisterSet[operatingRegister] & ~(byte)(1 << bit)); // Clear bit n of R
            LogInstructionExec($"0x{_currentInstruction:X2}: RES {bit}, {Registers.RegisterName(operatingRegister)}");
        }
        private void RES_B_RRMEM(byte bit, byte operatingRegister)
        {
            ushort reg = Registers.GetR16FromHighIndexer(operatingRegister);
            _memory.Write(reg, (byte)(_memory.Read(reg) & ~(byte)(1 << bit))); // Clear bit n of (RR)
            LogInstructionExec($"0x{_currentInstruction:X2}: RES {bit}, ({Registers.RegisterName(operatingRegister, true)})");
        }

        private void SET_B_R(byte bit, byte operatingRegister)
        {
            Registers.RegisterSet[operatingRegister] = (byte)(Registers.RegisterSet[operatingRegister] | (byte)(1 << bit)); // Set bit n of R
            LogInstructionExec($"0x{_currentInstruction:X2}: RES {bit}, {Registers.RegisterName(operatingRegister)}");
        }
        private void SET_B_RRMEM(byte bit, byte operatingRegister)
        {
            ushort reg = Registers.GetR16FromHighIndexer(operatingRegister);
            _memory.Write(reg, (byte)(_memory.Read(reg) | (byte)(1 << bit))); // Set bit n of (RR)
            LogInstructionExec($"0x{_currentInstruction:X2}: RES {bit}, ({Registers.RegisterName(operatingRegister, true)})");
        }
    }
}
