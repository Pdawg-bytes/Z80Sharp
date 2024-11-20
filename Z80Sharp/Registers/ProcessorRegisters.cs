using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Diagnostics.CodeAnalysis;
using System.Reflection;
using System.Reflection.Metadata;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Z80Sharp.Enums;
using Z80Sharp.Helpers;
using Z80Sharp.Interfaces;

namespace Z80Sharp.Registers
{
    public unsafe partial struct ProcessorRegisters
    {
        [InlineArray(24)]
        public struct RegisterArray
        {
            internal byte _element0;
        }
        public RegisterArray RegisterSet;

        public bool IFF1;
        public bool IFF2;

        public InterruptMode InterruptMode;


        public ProcessorRegisters()
        {
            RegisterSet = new RegisterArray();
        }


        private static readonly Dictionary<byte, string> _highBitRegisterPairs = new()
        {
            { A, "AF" }, { F, "AF" }, { A_, "AF'" }, { F_, "AF'" },
            { B, "BC" }, { C, "BC" }, { B_, "BC'" }, { C_, "BC'" },
            { D, "DE" }, { E, "DE" }, { D_, "DE'" }, { E_, "DE'" },
            { H, "HL" }, { L, "HL" }, { H_, "HL'" }, { L_, "HL'" },
            { IXh, "IX" }, { IXl, "IX" }, { IYh, "IY" }, { IYl, "IY" },
            { SPi, "SP" }, { PCi, "PC" }
        };

        private static readonly Dictionary<byte, string> _lowBitRegisters = new()
        {
            { A, "A" }, { F, "F" }, { A_, "A'" }, { F_, "F'" },
            { B, "B" }, { C, "C" }, { B_, "B'" }, { C_, "C'" },
            { D, "D" }, { E, "E" }, { D_, "D'" }, { E_, "E'" },
            { H, "H" }, { L, "L" }, { H_, "H'" }, { L_, "L'" },
            { IXh, "IXH" }, { IXl, "IXL" }, { IYh, "IYH" }, { IYl, "IYL" },
            { I, "I" }, { R, "R" }
        };

        /// <summary>
        /// Gets the name of an 8 or 16-bit register given its indexer.
        /// </summary>
        /// <param name="registerIndexer">The register indexer.</param>
        /// <param name="highBits">True if the register is a pair.</param>
        /// <returns>The name of the register according to the above dictionary.</returns>
        public string RegisterName([ConstantExpected] byte registerIndexer, bool highBits = false)
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
        /// Gets the name of a jump condition given its condition operation.
        /// </summary>
        /// <param name="condition">The condition being used in the jump operation.</param>
        /// <remarks>https://www.zilog.com/docs/z80/um0080.pdf page 277 details the names and values of each condition.</remarks>
        /// <returns>The name of the <paramref name="condition"/>.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public string JumpConditionName([ConstantExpected] byte condition) => condition switch
        {
            NZ_C => "NZ",
            Z_C  => "Z",
            NC_C => "NC",
            C_C  => "C",
            PO_C => "PO",
            PE_C => "PE",
            P_C  => "P",
            M_C  => "M",
            _ => "?"
        };

        /// <summary>
        /// Checks if any of a flag is set for use in a jump condition.
        /// </summary>
        /// <param name="condition">The flag condition.</param>
        /// <returns>True if the flag condition matches the expected value; false if otherwise.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool EvaluateJumpFlagCondition([ConstantExpected] in byte condition)
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

        /// <summary>
        /// Gets the 16-bit register pair given the high register in the pair.
        /// </summary>
        /// <param name="indexer">The indexer of the high register.</param>
        /// <returns>The value inside the 16-bit register pair.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ushort GetR16FromHighIndexer([ConstantExpected] byte indexer) => (ushort)(RegisterSet[indexer] << 8 | RegisterSet[indexer + 1]);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void R8Exchange([ConstantExpected] byte reg1, [ConstantExpected] byte reg2)
        {
            byte reg1_old = RegisterSet[reg1];
            RegisterSet[reg1] = RegisterSet[reg2];
            RegisterSet[reg2] = reg1_old;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void R16Exchange([ConstantExpected] byte regPair1, [ConstantExpected] byte regPair2)
        {
            (RegisterSet[regPair1], RegisterSet[regPair2]) = (RegisterSet[regPair2], RegisterSet[regPair1]);
            (RegisterSet[regPair1 + 1], RegisterSet[regPair2 + 1]) = (RegisterSet[regPair2 + 1], RegisterSet[regPair1 + 1]);
        }



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
        /// The index X register high indexer.
        /// </summary>
        public const byte IXh = 18;

        /// <summary>
        /// The index X register low indexer.
        /// </summary>
        public const byte IXl = 19;
        /// <summary>
        /// The index Y register high indexer.
        /// </summary>
        public const byte IYh = 20;
        /// <summary>
        /// The index Y register low indexer.
        /// </summary>
        public const byte IYl = 21;
        #endregion


        #region Utility register indexers
        /// <summary>
        /// The stack pointer register indexer.
        /// </summary>
        /// <remarks>
        /// This register is 16-bit only, therefore, it is 2 bytes wide.
        /// </remarks>
        public const byte SPi = 22;
        public const byte SPiL = 23;
        /// <summary>
        /// The program counter register indexer.
        /// </summary>
        /// <remarks>
        /// This register is 16-bit only, therefore, it is 2 bytes wide.
        /// </remarks>
        public const byte PCi = 24;
        public const byte PCiL = 25;
        #endregion


        #region Flags register operations
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlag(FlagType flag)
        {
            RegisterSet[F] |= (byte)flag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void ClearFlag(FlagType flag)
        {
            RegisterSet[F] &= (byte)~flag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsFlagSet(FlagType flag)
        {
            return (RegisterSet[F] & (byte)flag) == (byte)flag;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetFlagBits(byte flagMask)
        {
            RegisterSet[F] |= flagMask;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void InvertFlag(FlagType flag)
        {
            RegisterSet[F] ^= (byte)flag;
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



        #region Conditional constants
        /// <summary>
        /// The non-zero condition.
        /// </summary>
        public const byte NZ_C = 0b000;
        /// <summary>
        /// The zero condition.
        /// </summary>
        public const byte Z_C = 0b001;
        /// <summary>
        /// The non-carry condition.
        /// </summary>
        public const byte NC_C = 0b010;
        /// <summary>
        /// The carry condition.
        /// </summary>
        public const byte C_C = 0b011;
        /// <summary>
        /// The parity/overflow unset condition.
        /// </summary>
        public const byte PO_C = 0b100;
        /// <summary>
        /// The parity/overflow set condition.
        /// </summary>
        public const byte PE_C = 0b101;
        /// <summary>
        /// The sign unset condition.
        /// </summary>
        public const byte P_C = 0b110;
        /// <summary>
        /// The sign set condition.
        /// </summary>
        public const byte M_C = 0b111;
        #endregion
    }
}