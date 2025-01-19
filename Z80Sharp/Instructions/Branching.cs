﻿using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using Z80Sharp.Helpers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        private void JP_NN()
        {
            ushort jumpTo = FetchImmediateWord();
            Registers.PC = jumpTo;
            //LogInstructionExec($"0xC3: JP 0x{jumpTo:X4}");
        }
        private void JP_RR(ref ushort operatingRegister)
        {
            Registers.PC = operatingRegister;
            //LogInstructionExec($"0x{_currentInstruction:X2}: JP RR");
        }
        private void JP_NN_C([ConstantExpected] byte flagCondition)
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

        private void JR_D()
        {
            byte offset = Fetch();
            Registers.PC += (ushort)(sbyte)offset;
            //LogInstructionExec($"0x{_currentInstruction:X2}: JR D:0x{displacement:X2} -> 0x{Registers.PC:X4}");
        }
        // Technically, if we were doing this off of ((_currentInstruction >> 3) & 0x07), we'd need to (& 0x3) the value
        // again to get the right flag condition. However, instead of computing it on the fly, we can use our jump table
        // to pass in the correct values when we call this function from its respective opcodes.
        private void JR_CC_D([ConstantExpected] byte flagCondition)
        {
            if (!Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                Registers.PC += 1;
                return;
                //LogInstructionExec($"0x{_currentInstruction:X2}: JR {Registers.JumpConditionName(flagCondition)} D:0x{((byte)displacement).ToString("X2")} -> 0x{jumpTo:X4}");
            }
            byte offset = Fetch();
            Registers.PC += (ushort)(sbyte)offset;
        }

        private void DJNZ_D()
        {
            Registers.B--;

            if (Registers.B == 0) { Registers.PC += 1; return; }

            var offset = Fetch();
            Registers.PC += (ushort)(sbyte)offset;
        }


        private void RET()
        {
            POP_PC();
            //LogInstructionExec("0xC9: RET");
        }
        private void RET_CC([ConstantExpected] byte flagCondition)
        {
            if (Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                POP_PC();
                //LogInstructionExec($"0x{_currentInstruction:X2}: RET {Registers.JumpConditionName(flagCondition)}");
            }
            else
            {
                //LogInstructionExec($"0x{_currentInstruction:X2}: RET (no ret), {Registers.JumpConditionName(flagCondition)} not set to expected value.");
            }
        }
        private void RETN()
        {
            POP_PC();
            Registers.IFF1 = Registers.IFF2;
            //LogInstructionExec("0x45: RETN");
        }
        private void RETI()
        {
            POP_PC();
            // signal device triggering NMI that routine has completed
            //LogInstructionExec("0x4D: RETI");
        }


        private void RST_HH([ConstantExpected] byte pcStart)
        {
            PUSH_PC();
            Registers.PC = pcStart;
            //LogInstructionExec($"0x{_currentInstruction:X2}: RST 0x{pcStart:X4}");
        }
        private void RST_HH_SILENT(byte pcStart)
        {
            PUSH_PC();
            Registers.PC = pcStart;
        }


        private void CALL_NN()
        {
            ushort jumpTo = FetchImmediateWord();
            PUSH_PC();
            Registers.PC = jumpTo;
            //LogInstructionExec($"0x{_currentInstruction:X2}: CALL NN:0x{jumpTo:X4}");
        }
        private void CALL_CC_NN([ConstantExpected] byte flagCondition)
        {
            ushort jumpTo = FetchImmediateWord();
            if (Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                PUSH_PC();
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