using Z80Sharp.Enums;
using System.Diagnostics.CodeAnalysis;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Registers
{
    public unsafe partial struct ProcessorRegisters
    {
        public bool IFF1;
        public bool IFF2;
        public InterruptMode InterruptMode;

        /// <summary>
        /// Gets the name of the current interrupt mode.
        /// </summary>
        /// <param name="interruptMode">The interrupt mode the processor is currently in.</param>
        /// <returns>The name, in string form, of the mode.</returns>
        public string InterruptModeName(InterruptMode interruptMode) => interruptMode switch
        {
            InterruptMode.IM0 => "IM0",
            InterruptMode.IM1 => "IM1",
            InterruptMode.IM2 => "IM2",
            _ => "UNKNOWN INTERRUPT MODE"
        };

        /// <summary>
        /// Checks if any of a flag is set for use in a jump condition.
        /// </summary>
        /// <param name="condition">The flag condition.</param>
        /// <returns>True if the flag condition matches the expected value; false if otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EvaluateJumpFlagCondition([ConstantExpected] byte condition)
        {
            switch (condition & 0xFE) // Mask out LSB to handle paired conditions.
            {
                // ((condition & 1) == 1) checks if the flag should be cleared or set using the LSB of condition.
                // https://www.zilog.com/docs/z80/um0080.pdf page 277 details the condition values used below.
                case 0x00:
                    return IsFlagSet(FlagType.Z) == ((condition & 1) == 1);
                case 0x02:
                    return IsFlagSet(FlagType.C) == ((condition & 1) == 1);
                case 0x04:
                    return IsFlagSet(FlagType.PV) == ((condition & 1) == 1);
                case 0x06:
                    return IsFlagSet(FlagType.S) == ((condition & 1) == 1);
                default:
                    return false;
            }
        }


        #region Register exchange operations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void R8Exchange(ref byte reg1, ref byte reg2)
        {
            byte reg1_old = reg1;
            reg1 = reg2;
            reg2 = reg1_old;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void R16Exchange(ref ushort regPair1, ref ushort regPair2)
        {
            (regPair1, regPair2) = (regPair2, regPair1);
        }
        #endregion


        #region Flags register operations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlag(FlagType flag)
        {
            F |= (byte)flag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearFlag(FlagType flag)
        {
            F &= (byte)~flag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsFlagSet(FlagType flag)
        {
            return (F & (byte)flag) == (byte)flag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlagBits(byte flagMask)
        {
            F |= flagMask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvertFlag(FlagType flag)
        {
            F ^= (byte)flag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlagConditionally(FlagType flag, bool condition)
        {
            if (condition)
                SetFlag(flag);
            else
                ClearFlag(flag);
        }
        #endregion
    }
}