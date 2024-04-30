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
        /// 16-bit wide address register composed of 8-bit registers B and C.
        /// </summary>
        public short BC { get; set; }

        /// <summary>
        /// 16-bit wide address register composed of 8-bit registers D and E.
        /// </summary>
        public short DE { get; set; }

        /// <summary>
        /// 16-bit wide address register composed of 8-bit registers H and L.
        /// </summary>
        public short HL { get; set; }

        /// <summary>
        /// The status register of the Z80; 6 flags defined in <see cref="StatusRegisterFlag"/>
        /// </summary>
        public byte StatusRegister { get; set; }

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
        /// The F register. (8-bits wide)
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
