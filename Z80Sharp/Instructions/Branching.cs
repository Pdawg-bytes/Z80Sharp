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
    public unsafe partial class Z80
    {
        // JP: unconditional jumps to value in register or immediate in memory.
        private void JP_NN()
        {
            ushort jumpTo = FetchImmediateWord();
            Registers.PC = jumpTo;
            //LogInstructionExec($"0xC3: JP 0x{jumpTo:X4}");
        }
        private void JP_RR(byte operatingRegister)
        {
            ushort jumpTo = Registers.GetR16FromHighIndexer(operatingRegister);
            Registers.PC = jumpTo;
            //LogInstructionExec($"0x{_currentInstruction:X2}: JP ({Registers.RegisterName(operatingRegister, true)}:0x{jumpTo:X4})");
        }
        // Conditional jump to immediate
        private void JP_NN_C(byte flagCondition)
        {
            ushort jumpTo = FetchImmediateWord();
            if (Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                Registers.PC = jumpTo;
                //LogInstructionExec($"0x{_currentInstruction:X2}: JP {Registers.JumpConditionName(flagCondition)} 0x{jumpTo:X4}");
            }
            else
            {
                //LogInstructionExec($"0x{_currentInstruction:X2}: JP (no jump), {Registers.JumpConditionName(flagCondition)} not set.");
            }
        }

        // No generic instruction here as JR D is the only (JR n) format instruction.
        private void JR_D()
        {
            sbyte displacement = (sbyte)Fetch();
            ushort jumpTo = (ushort)(Registers.PC + displacement);
            Registers.PC = jumpTo;
            //LogInstructionExec($"0x{_currentInstruction:X2}: JR D:0x{displacement:X2} -> 0x{Registers.PC:X4}");
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
                //LogInstructionExec($"0x{_currentInstruction:X2}: JR {Registers.JumpConditionName(flagCondition)} D:0x{((byte)displacement).ToString("X2")} -> 0x{jumpTo:X4}");
            }
            else
            {
                //LogInstructionExec($"0x{_currentInstruction:X2}: JR (no jump), {Registers.JumpConditionName(flagCondition)} not set.");
            }
        }

        private void DJNZ_D()
        {
            sbyte displacement = (sbyte)Fetch();

            byte b = Registers.RegisterSet[B];

            Registers.RegisterSet[B] = DECAny(b);

            if (!Registers.IsFlagSet(FlagType.Z))
            {
                Registers.PC = (ushort)(Registers.PC + displacement);
                //LogInstructionExec($"0x{_currentInstruction:X2}: DJNZ D:0x{displacement.ToString()} -> 0x{Registers.PC:X4}");
            }
            else
            {
                //LogInstructionExec($"0x{_currentInstruction:X2}: DJNZ (no jump), B not 0 after decrement.");
            }
        }


        // Return instructions
        private void RET()
        {
            POP_PC_SILENT();
            //LogInstructionExec("0xC9: RET");
        }
        private void RET_CC(byte flagCondition)
        {
            if (Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                POP_PC_SILENT();
                //LogInstructionExec($"0x{_currentInstruction:X2}: RET {Registers.JumpConditionName(flagCondition)}");
            }
            else
            {
                //LogInstructionExec($"0x{_currentInstruction:X2}: RET (no ret), {Registers.JumpConditionName(flagCondition)} not set to expected value.");
            }
        }
        private void RETN()
        {
            POP_PC_SILENT();
            Registers.IFF1 = Registers.IFF2;
            //LogInstructionExec("0x45: RETN");
        }
        private void RETI()
        {
            POP_PC_SILENT();
            // signal device triggering NMI that routine has completed
            //LogInstructionExec("0x4D: RETI");
        }


        private void RST_HH(byte pcStart)
        {
            PUSH_PC_SILENT();
            Registers.PC = pcStart;
            //LogInstructionExec($"0x{_currentInstruction:X2}: RST 0x{pcStart:X4}");
        }
        private void RST_HH_SILENT(byte pcStart)
        {
            PUSH_PC_SILENT();
            Registers.PC = pcStart;
        }


        private void CALL_NN()
        {
            ushort jumpTo = FetchImmediateWord();
            PUSH_PC_SILENT();
            Registers.PC = jumpTo;
            //LogInstructionExec($"0x{_currentInstruction:X2}: CALL NN:0x{jumpTo:X4}");
        }
        private void CALL_CC_NN(byte flagCondition)
        {
            ushort jumpTo = FetchImmediateWord();
            if (Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                PUSH_PC_SILENT();
                Registers.PC = jumpTo;
                //LogInstructionExec($"0x{_currentInstruction:X2}: CALL {Registers.JumpConditionName(flagCondition)}, NN:{jumpTo:X4}");
            }
            else
            {
                //LogInstructionExec($"0x{_currentInstruction:X2}: CALL (no call), {Registers.JumpConditionName(flagCondition)} not set to expected value.");
            }
        }
    }
}