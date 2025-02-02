using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void JP_NN() => Registers.MEMPTR = Registers.PC = FetchImmediateWord();

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void JP_RR(ref ushort operatingRegister) => Registers.PC = operatingRegister;

        private void JP_NN_C([ConstantExpected] byte flagCondition)
        {
            ushort jumpTo = FetchImmediateWord();
            if (Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                Registers.PC = jumpTo;
            }
            Registers.MEMPTR = jumpTo;
        }


        private void JR_D()
        {
            byte offset = Fetch();
            Registers.PC += (ushort)(sbyte)offset;
            Registers.MEMPTR = Registers.PC;
        }

        private void JR_CC_D([ConstantExpected] byte flagCondition)
        {
            if (!Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                Registers.PC += 1;
                _clock.LastOperationStatus = false;
                return;
            }

            byte offset = Fetch();
            Registers.PC += (ushort)(sbyte)offset;
            Registers.MEMPTR = Registers.PC;
            _clock.LastOperationStatus = true;
        }


        private void DJNZ_D()
        {
            Registers.B--;

            if (Registers.B == 0) 
            { 
                Registers.PC += 1;
                _clock.LastOperationStatus = true;
                return; 
            }

            byte offset = Fetch();
            Registers.PC += (ushort)(sbyte)offset;
            Registers.MEMPTR = Registers.PC;
            _clock.LastOperationStatus = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RET()
        {
            POP_PC();
            Registers.MEMPTR = Registers.PC;
        }

        private void RET_CC([ConstantExpected] byte flagCondition)
        {
            if (Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                POP_PC();
                Registers.MEMPTR = Registers.PC;
                _clock.LastOperationStatus = true;
                return;
            }
            _clock.LastOperationStatus = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RETN()
        {
            POP_PC();
            Registers.MEMPTR = Registers.PC;
            Registers.IFF1 = Registers.IFF2;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void RETI()
        {
            POP_PC();
            Registers.MEMPTR = Registers.PC;
        }


        private void RST_H([ConstantExpected] byte pcStart)
        {
            PUSH_PC();
            Registers.PC = pcStart;
            Registers.MEMPTR = pcStart;
        }


        private void CALL_NN()
        {
            ushort jumpTo = FetchImmediateWord();
            PUSH_PC();
            Registers.PC = jumpTo;
            Registers.MEMPTR = jumpTo;
        }
        private void CALL_CC_NN([ConstantExpected] byte flagCondition)
        {
            ushort jumpTo = FetchImmediateWord();
            if (Registers.EvaluateJumpFlagCondition(flagCondition))
            {
                PUSH_PC();
                Registers.PC = jumpTo;
                _clock.LastOperationStatus = true;
                return;
            }
            Registers.MEMPTR = jumpTo;
            _clock.LastOperationStatus = false;
        }
    }
}