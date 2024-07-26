using Z80Sharp.Enums;
using Z80Sharp.Helpers;
using Z80Sharp.Interfaces;

namespace Z80Sharp.Registers
{
    /// <summary>
    /// Main implementation of <see cref="IRegisterSet"/>
    /// </summary>
    public partial struct ProcessorRegisters
    {
        public ProcessorRegisters()
        {
            RegisterSet = new byte[26];
        }

        public byte[] RegisterSet;

        /// <summary>
        /// Interrupt control flag 1
        /// </summary>
        public bool IFF1;
        /// <summary>
        /// Interrupt control flag 2
        /// </summary>
        public bool IFF2;


        #region Main register indexers
        /// <summary>
        /// The B register indexer
        /// </summary>
        public const byte B = 0;

        /// <summary>
        /// The C register indexer
        /// </summary>
        public const byte C = 1;

        /// <summary>
        /// The D register indexer
        /// </summary>
        public const byte D = 2;

        /// <summary>
        /// The E register indexer
        /// </summary>
        public const byte E = 3;

        /// <summary>
        /// The H register indexer
        /// </summary>
        public const byte H = 4;

        /// <summary>
        /// The L register indexer
        /// </summary>
        public const byte L = 5;

        /// <summary>
        /// The flags register indexer
        /// </summary>
        public const byte F = 6;

        /// <summary>
        /// The accumulator register indexer
        /// </summary>
        public const byte A = 7;
        #endregion


        #region Alternate register indexers
        /// <summary>
        /// The alternate B' register indexer
        /// </summary>
        public const byte B_ = 8;

        /// <summary>
        /// The alternate C' register indexer
        /// </summary>
        public const byte C_ = 9;

        /// <summary>
        /// The alternate D' register indexer
        /// </summary>
        public const byte D_ = 10;

        /// <summary>
        /// The alternate E' register indexer
        /// </summary>
        public const byte E_ = 11;

        /// <summary>
        /// The alternate H' register indexer
        /// </summary>
        public const byte H_ = 12;

        /// <summary>
        /// The alternate L' register indexer
        /// </summary>
        public const byte L_ = 13;

        /// <summary>
        /// The alternate flags F' register indexer
        /// </summary>
        public const byte F_ = 14;

        /// <summary>
        /// The alternate accumulator A' register indexer
        /// </summary>
        public const byte A_ = 15;
        #endregion


        #region Hardware control indexers
        /// <summary>
        /// Interrupt vector base indexer.
        /// </summary>
        public const byte I = 16;
        /// <summary>
        /// Memory refresh base indexer.
        /// </summary>
        public const byte R = 17;
        #endregion


        #region Index register indexers
        /// <summary>
        /// The index X register indexer.
        /// </summary>
        /// <remarks>
        /// This register is 16-bit only, therefore, it is 2 bytes wide.
        /// </remarks>
        public const byte IX = 18;
        /// <summary>
        /// The index Y register indexer.
        /// </summary>
        /// <remarks>
        /// This register is 16-bit only, therefore, it is 2 bytes wide.
        /// </remarks>
        public const byte IY = 20;
        #endregion


        #region Utility register indexers
        /// <summary>
        /// The stack pointer register indexer.
        /// </summary>
        /// <remarks>
        /// This register is 16-bit only, therefore, it is 2 bytes wide.
        /// </remarks>
        public byte SPi = 22;
        /// <summary>
        /// The program counter register indexer.
        /// </summary>
        /// <remarks>
        /// This register is 16-bit only, therefore, it is 2 bytes wide.
        /// </remarks>
        public const byte PCi = 24;
        #endregion


        #region Flags register operations
        public void SetFlag(StatusRegisterFlag flag, bool prime = false)
        {
            if (prime) RegisterSet[F_] |= (byte)flag;
            RegisterSet[F] |= (byte)flag;
        }

        public void ClearFlag(StatusRegisterFlag flag, bool prime = false)
        {
            if (prime) { RegisterSet[F_] &= (byte)~flag; }
            RegisterSet[F] &= (byte)~flag;
        }

        public bool IsFlagSet(StatusRegisterFlag flag, bool prime = false)
        {
            if (prime) { return (RegisterSet[F_] & (byte)flag) == (byte)flag; }
            return (RegisterSet[F] & (byte)flag) == (byte)flag;
        }
        #endregion
    }
}