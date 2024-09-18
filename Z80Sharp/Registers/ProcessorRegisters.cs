using System.Runtime.CompilerServices;
using Z80Sharp.Enums;
using Z80Sharp.Helpers;
using Z80Sharp.Interfaces;

namespace Z80Sharp.Registers
{
    public partial struct ProcessorRegisters /*: IRegisterSet */
    {
        public ProcessorRegisters()
        {
            RegisterSet = new byte[26];
        }

        private static readonly Dictionary<byte, string> _highBitRegisterPairs = new()
        {
            { A, "AF" }, { F, "AF" }, { A_, "AF'" }, { F_, "AF'" },
            { B, "BC" }, { C, "BC" }, { B_, "BC'" }, { C_, "BC'" },
            { D, "DE" }, { E, "DE" }, { D_, "DE'" }, { E_, "DE'" },
            { H, "HL" }, { L, "HL" }, { H_, "HL'" }, { L_, "HL'" },
            { IXi, "IX" }, { IYi, "IY" }, { SPi, "SP" }, { PCi, "PC" }
        };

        private static readonly Dictionary<byte, string> _lowBitRegisters = new()
        {
            { A, "A" }, { F, "F" }, { A_, "A'" }, { F_, "F'" },
            { B, "B" }, { C, "C" }, { B_, "B'" }, { C_, "C'" },
            { D, "D" }, { E, "E" }, { D_, "D'" }, { E_, "E'" },
            { H, "H" }, { L, "L" }, { H_, "H'" }, { L_, "L'" },
            { I, "I" }, { R, "R" }
        };

        /// <summary>
        /// Gets the name of an 8 or 16-bit register given its indexer.
        /// </summary>
        /// <param name="registerIndexer">The register indexer.</param>
        /// <param name="highBits">True if the register is a pair.</param>
        /// <returns>The name of the register according to the above dictionary.</returns>
        public string RegisterName(byte registerIndexer, bool highBits = false)
        {
            if (highBits && _highBitRegisterPairs.TryGetValue(registerIndexer, out var registerName))
            {
                return registerName;
            }
            if (!highBits && _lowBitRegisters.TryGetValue(registerIndexer, out registerName))
            {
                return registerName;
            }
            return highBits ? "UNKNOWN 16BIT REGISTER/PAIR" : "UNKNOWN REGISTER";
        }

        /// <summary>
        /// Gets the name of a jump condition given its condition operation.
        /// </summary>
        /// <param name="condition">The condition being used in the jump operation.</param>
        /// <remarks>https://www.zilog.com/docs/z80/um0080.pdf page 277 details the names and values of each condition.</remarks>
        /// <returns>The name of the <paramref name="condition"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string JumpConditionName(byte condition) => condition switch
        {
            0b000 => "NZ",
            0b001 => "Z",
            0b010 => "NC",
            0b011 => "C",
            0b100 => "PO",
            0b101 => "PE",
            0b110 => "P",
            0b111 => "M"
        };

        /// <summary>
        /// Checks if any of a flag is set for use in a jump condition.
        /// </summary>
        /// <param name="condition">The flag condition.</param>
        /// <returns>True if the flag condition matches the expected value; false if otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EvaluateJumpFlagCondition(byte condition)
        {
            switch (condition & 0xFE) // Mask out LSB to handle paired conditions.
            {
                // ((condition & 1) == 1) checks if the flag should be cleared or set using the LSB of condition.
                // https://www.zilog.com/docs/z80/um0080.pdf page 277 details the condition values used below.
                case 0x00:
                    return IsFlagSet(StatusRegisterFlag.ZeroFlag) == ((condition & 1) == 1);
                case 0x02:
                    return IsFlagSet(StatusRegisterFlag.CarryFlag) == ((condition & 1) == 1);
                case 0x04:
                    return IsFlagSet(StatusRegisterFlag.ParityOverflowFlag) == ((condition & 1) == 1);
                case 0x06:
                    return IsFlagSet(StatusRegisterFlag.SignFlag) == ((condition & 1) == 1);
                default:
                    return false;
            }
        }

        /// <summary>
        /// Gets the 16-bit register pair given the high register in the pair.
        /// </summary>
        /// <param name="indexer">The indexer of the high register.</param>
        /// <returns>The value inside the 16-bit register pair.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort GetR16FromHighIndexer(byte indexer) => (ushort)(RegisterSet[indexer] << 8 | RegisterSet[indexer + 1]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void R8Exchange(byte reg1, byte reg2)
        {
            byte reg1_old = RegisterSet[reg1];
            RegisterSet[reg1] = reg2;
            RegisterSet[reg2] = reg1_old;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void R16Exchange(byte regPair1, byte regPair2)
        {
            ushort regPair1_old = GetR16FromHighIndexer(regPair1);
            ushort regPair2_old = GetR16FromHighIndexer(regPair2);
            RegisterSet[regPair1] = regPair2_old.GetUpperByte();
            RegisterSet[regPair1 + 1] = regPair2_old.GetLowerByte();
            RegisterSet[regPair2] = regPair1_old.GetUpperByte();
            RegisterSet[regPair2 + 1] = regPair1_old.GetLowerByte();
        }


        public byte[] RegisterSet { get; init; }

        public bool IFF1 { get; set; }
        public bool IFF2 { get; set; }

        public InterruptMode InterruptMode { get; set; }


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
        public const byte IXi = 18;
        /// <summary>
        /// The index Y register indexer.
        /// </summary>
        /// <remarks>
        /// This register is 16-bit only, therefore, it is 2 bytes wide.
        /// </remarks>
        public const byte IYi = 20;
        #endregion


        #region Utility register indexers
        /// <summary>
        /// The stack pointer register indexer.
        /// </summary>
        /// <remarks>
        /// This register is 16-bit only, therefore, it is 2 bytes wide.
        /// </remarks>
        public const byte SPi = 22;
        /// <summary>
        /// The program counter register indexer.
        /// </summary>
        /// <remarks>
        /// This register is 16-bit only, therefore, it is 2 bytes wide.
        /// </remarks>
        public const byte PCi = 24;
        #endregion


        #region Flags register operations
        public void SetFlag(StatusRegisterFlag flag)
        {
            RegisterSet[F] |= (byte)flag;
        }

        public void ClearFlag(StatusRegisterFlag flag)
        {
            RegisterSet[F] &= (byte)~flag;
        }

        public bool IsFlagSet(StatusRegisterFlag flag)
        {
            return (RegisterSet[F] & (byte)flag) == (byte)flag;
        }
        #endregion
    }
}