using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void JP_NN()
        {
            ushort jumpTo = FetchImmediateWord();
            Registers.RegisterSet[PCi] = (byte)(jumpTo >> 8);
            Registers.RegisterSet[PCi + 1] = (byte)jumpTo;
            LogInstructionExec($"0xC3: JP 0x{jumpTo:X4}");
        }

        private void JP_RR(byte operatingRegister)
        {
            ushort jumpTo = Registers.GetR16FromHighIndexer(operatingRegister);
            Registers.PC = jumpTo;
            LogInstructionExec($"0x{_currentInstruction:X2}: JP ({Registers.RegisterName(operatingRegister, true)}:0x{jumpTo:X4})");
        }

        private void JP_NN_C(byte flagCondition)
        {
            if (Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                ushort jumpTo = FetchImmediateWord();
                Registers.PC = jumpTo;
                LogInstructionExec($"0x{_currentInstruction:X2}: JP {Registers.JumpConditionName(flagCondition)}, 0x{jumpTo:X4}");
            }
            else
            {
                LogInstructionExec($"0x{_currentInstruction:X2}: JP (no jump), {Registers.JumpConditionName(flagCondition)} not set.");
            }
        }

        // No generic instruction here as JR D is the only (JR n) format instruction.
        private void JR_D()
        {
            ushort jumpTo = (ushort)(Registers.PC + (sbyte)Fetch());
            Registers.PC = jumpTo;
            LogInstructionExec($"JR D:0x{FetchLast():X2}");
        }

        // Technically, if we were doing this off of ((_currentInstruction >> 3) & 0x07), we'd need to (& 0x3) the value
        // again to get the right flag condition. However, instead of computing it on the fly, we can use our jump table
        // to pass in the correct values when we call this function from its respective opcodes.
        private void JR_CC_D(byte flagCondition)
        {
            if (Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                sbyte displacement = (sbyte)Fetch();
                ushort jumpTo = (ushort)(Registers.PC + displacement);
                Registers.PC = jumpTo;
                LogInstructionExec($"0x{_currentInstruction:X2}: JR {Registers.JumpConditionName(flagCondition)}, D:0x{displacement.ToString()}");
            }
            else
            {
                LogInstructionExec($"0x{_currentInstruction:X2}: JR (no jump), {Registers.JumpConditionName(flagCondition)} not set.");
            }
        }
    }
}