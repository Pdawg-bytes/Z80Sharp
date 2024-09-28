using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using Z80Sharp.Helpers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        // JP: unconditional jumps to value in register or immediate in memory.
        private void JP_NN()
        {
            ushort jumpTo = FetchImmediateWord();
            Registers.RegisterSet[PCi] = jumpTo.GetUpperByte();
            Registers.RegisterSet[PCi + 1] = jumpTo.GetLowerByte();
            LogInstructionExec($"0xC3: JP 0x{jumpTo:X4}");
        }
        private void JP_RR(byte operatingRegister)
        {
            ushort jumpTo = Registers.GetR16FromHighIndexer(operatingRegister);
            Registers.PC = jumpTo;
            LogInstructionExec($"0x{_currentInstruction:X2}: JP ({Registers.RegisterName(operatingRegister, true)}:0x{jumpTo:X4})");
        }
        // Conditional jump to immediate
        private void JP_NN_C(byte flagCondition)
        {
            ushort jumpTo = FetchImmediateWord();
            if (Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                Registers.PC = jumpTo;
                LogInstructionExec($"0x{_currentInstruction:X2}: JP {Registers.JumpConditionName(flagCondition)} 0x{jumpTo:X4}");
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
            sbyte displacement = (sbyte)Fetch();
            if (Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                ushort jumpTo = (ushort)(Registers.PC + displacement);
                Registers.PC = jumpTo;
                LogInstructionExec($"0x{_currentInstruction:X2}: JR {Registers.JumpConditionName(flagCondition)} D:0x{displacement.ToString()} -> 0x{jumpTo:X4}");
            }
            else
            {
                LogInstructionExec($"0x{_currentInstruction:X2}: JR (no jump), {Registers.JumpConditionName(flagCondition)} not set.");
            }
        }

        // Operation: Fetch displacement; Decrement B; Check if B == zero; If not, jump to PC + displacement.
        private void DJNZ_D()
        {
            sbyte displacement = (sbyte)Fetch();
            byte b = Registers.RegisterSet[B];
            Registers.RegisterSet[B] = --b;
            if (b != 0)
            {
                ushort jumpTo = (ushort)(Registers.PC + displacement);
                Registers.PC = jumpTo;
                LogInstructionExec($"0x{_currentInstruction:X2}: DJNZ D:0x{displacement.ToString()} -> 0x{jumpTo:X4}");
            }
            else
            {
                LogInstructionExec($"0x{_currentInstruction:X2}: DJNZ (no jump), B not 0 after decrement.");
            }
        }


        // Return instructions
        private void RET()
        {
            ushort sp = Registers.SP;
            Registers.RegisterSet[PCi + 1] = _memory.Read(sp++);
            Registers.RegisterSet[PCi] = _memory.Read(sp++);
            Registers.SP = sp;
            LogInstructionExec("0xC9: RET");
        }
        private void RET_CC(byte flagCondition)
        {
            if (Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                ushort sp = Registers.SP;
                Registers.RegisterSet[PCi + 1] = _memory.Read(sp++);
                Registers.RegisterSet[PCi] = _memory.Read(sp++);
                Registers.SP = sp;
                LogInstructionExec($"0x{_currentInstruction:X2}: RET {Registers.JumpConditionName(flagCondition)}");
            }
            else
            {
                LogInstructionExec($"0x{_currentInstruction:X2}: RET (no ret), {Registers.JumpConditionName(flagCondition)} not set to expected value.");
            }
        }


        private void RST_HH(byte pcStart)
        {
            ushort sp = Registers.SP;
            _memory.Write(--sp, Registers.RegisterSet[PCi]);
            _memory.Write(--sp, Registers.RegisterSet[PCi + 1]);
            Registers.SP = sp;
            Registers.PC = pcStart;
            LogInstructionExec($"0x{_currentInstruction:X2}: RST 0x{pcStart:X4}");
        }
        private void RST_HH_SILENT(byte pcStart)
        {
            ushort sp = Registers.SP;
            _memory.Write(--sp, Registers.RegisterSet[PCi]);
            _memory.Write(--sp, Registers.RegisterSet[PCi + 1]);
            Registers.SP = sp;
            Registers.PC = pcStart;
        }


        private void CALL_NN()
        {
            ushort jumpTo = FetchImmediateWord();
            ushort sp = Registers.SP;
            _memory.Write(--sp, Registers.RegisterSet[PCi]);
            _memory.Write(--sp, Registers.RegisterSet[PCi + 1]);
            Registers.SP = sp;
            Registers.PC = jumpTo;
            LogInstructionExec($"0x{_currentInstruction:X2}: CALL NN:0x{jumpTo:X4}");
        }
        private void CALL_CC_NN(byte flagCondition)
        {
            ushort jumpTo = FetchImmediateWord();
            if (Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                Registers.PC = jumpTo;
                LogInstructionExec($"0x{_currentInstruction:X2}: CALL {Registers.JumpConditionName(flagCondition)}, NN:{jumpTo:X4}");
            }
        }
    }
}