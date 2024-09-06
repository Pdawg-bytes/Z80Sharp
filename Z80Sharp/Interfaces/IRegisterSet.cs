using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;

namespace Z80Sharp.Interfaces
{
    /// <summary>
    /// Defines the Z80 register set.
    /// </summary>
    public interface IRegisterSet
    {
        /// <summary>
        /// Gives the name of a register given its indexer.
        /// </summary>
        /// <param name="operatingRegister">The register indexer</param>
        /// <param name="highBits">Whether or not a 16-bit register is being referenced</param>
        public string RegisterName(byte operatingRegister, bool highBits = false);

        /// <summary>
        /// Gets the corresponding 16-bit pair value given the High indexer.
        /// </summary>
        /// <param name="indexer">The High indexer of the pair. E.G: GetR16FromHighIndexer(H) returns HL.</param>
        /// <returns>The value of the register pair.</returns>
        public ushort GetR16FromHighIndexer(byte indexer);

        /// <summary>
        /// The raw set of registers. Represented by a <see cref="byte"/>[26].
        /// </summary>
        public byte[] RegisterSet { get; init; }

        /// <summary>
        /// Interrupt Flip-Flop 1 register.
        /// </summary>
        public bool IFF1 { get; set; }
        /// <summary>
        /// Interrupt Flip-Flop 2 register.
        /// </summary>
        public bool IFF2 { get; set; }

        /// <summary>
        /// The current interrupt mode of the processor.
        /// </summary>
        public InterruptMode InterruptMode { get; set; }

        /// <summary>
        /// 16-bit wide register composed of 8-bit registers B (high byte) and C (low byte).
        /// </summary>
        public ushort BC { get; set; }
        /// <summary>
        /// 16-bit wide register composed of 8-bit registers D (high byte) and E (low byte).
        /// </summary>
        public ushort DE { get; set; }
        /// <summary>
        /// 16-bit wide register composed of 8-bit registers H (high byte) and L (low byte).
        /// </summary>
        public ushort HL { get; set; }
        /// <summary>
        /// 16-bit wide register composed of 8-bit registers A (high byte) and F (low byte).
        /// </summary>
        public ushort AF { get; set; }


        /// <summary>
        /// Alternate 16-bit wide register composed of 8-bit registers B' (high byte) and C' (low byte).
        /// </summary>
        public ushort BC_ { get; set; }
        /// <summary>
        /// Alternate 16-bit wide register composed of 8-bit registers D' (high byte) and E' (low byte).
        /// </summary>
        public ushort DE_ { get; set; }
        /// <summary>
        /// Alternate 16-bit wide register composed of 8-bit registers H' (high byte) and L' (low byte).
        /// </summary>
        public ushort HL_ { get; set; }
        /// <summary>
        /// Alternate 16-bit wide register composed of 8-bit registers A' (high byte) and F' (low byte).
        /// </summary>
        public ushort AF_ { get; set; }

        /// <summary>
        /// The Stack Pointer register.
        /// </summary>
        public ushort SP { get; set; }
        /// <summary>
        /// The Program Counter register.
        /// </summary>
        public ushort PC { get; set; }

        /// <summary>
        /// The Index X register.
        /// </summary>
        public ushort IX { get; set; }
        /// <summary>
        /// The Index Y register.
        /// </summary>
        public ushort IY { get; set; }
    }
}