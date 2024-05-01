using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Registers;

namespace Z80Sharp.Interfaces
{
    /// <summary>
    /// Defines the set of main data registers in the Zilog Z80.
    /// </summary>
    public interface IRegisterSet
    {
        /// <summary>
        /// The Index X register
        /// </summary>
        public ushort IX { get; set; }

        /// <summary>
        /// The Index Y register
        /// </summary>
        public ushort IY { get; set; }

        /// <summary>
        /// The stack pointer register.
        /// </summary>
        public ushort SP { get; set; }

        /// <summary>
        /// The program counter register.
        /// </summary>
        public ushort PC { get; set; }

        /// <summary>
        /// 16-bit wide address register composed of 8-bit registers B and C.
        /// </summary>
        public ushort BC { get; set; }

        /// <summary>
        /// 16-bit wide address register composed of 8-bit registers D and E.
        /// </summary>
        public ushort DE { get; set; }

        /// <summary>
        /// 16-bit wide address register composed of 8-bit registers H and L.
        /// </summary>
        public ushort HL { get; set; }

        /// <summary>
        /// The interrupt vector register.
        /// </summary>
        public byte I { get; set; }

        /// <summary>
        /// The memory refresh register.
        /// </summary>
        public byte R { get; set; }

        /// <summary>
        /// Interrupt Flip-Flop 1 register.
        /// </summary>
        public bool IFF1 { get; set; }

        /// <summary>
        /// Interrupt Flip-Flop 2 register.
        /// </summary>
        public bool IFF2 { get; set; }

        /// <summary>
        /// The A register. (8-bits wide)
        /// </summary>
        public byte A { get; set; }

        /// <summary>
        /// The B register. (8-bits wide)
        /// </summary>
        public byte B { get; set; }

        /// <summary>
        /// The C register. (8-bits wide)
        /// </summary>
        public byte C { get; set; }

        /// <summary>
        /// The D register. (8-bits wide)
        /// </summary>
        public byte D { get; set; }

        /// <summary>
        /// The E register. (8-bits wide)
        /// </summary>
        public byte E { get; set; }

        /// <summary>
        /// The status register of the Z80; 6 flags defined in <see cref="StatusRegisterFlag"/>
        /// </summary>
        public byte F { get; set; }

        /// <summary>
        /// The H register. (8-bits wide)
        /// </summary>
        public byte H { get; set; }

        /// <summary>
        /// The L register. (8-bits wide)
        /// </summary>
        public byte L { get; set; }
    }
}
