using System.Diagnostics;
using System.Runtime.CompilerServices;
using Z80Sharp.Interfaces;

namespace Z80Sharp.Registers
{
    public unsafe partial struct ProcessorRegisters
    {
        #region Register pairs
        /// <summary>
        /// The 16-bit register pair composed of B and C.
        /// </summary>
        public ushort BC
        {
            get => (ushort)((RegisterSet[B] << 8) | RegisterSet[C]);
            set
            {
                RegisterSet[B] = (byte)(value >> 8);
                RegisterSet[C] = (byte)value;
            }
        }

        /// <summary>
        /// The 16-bit register pair composed of D and E.
        /// </summary>
        public ushort DE
        {
            get => (ushort)((RegisterSet[D] << 8) | RegisterSet[E]);
            set
            {
                RegisterSet[D] = (byte)(value >> 8);
                RegisterSet[E] = (byte)value;
            }
        }

        /// <summary>
        /// The 16-bit register pair composed of H and L.
        /// </summary>
        public ushort HL
        {
            get => (ushort)((RegisterSet[H] << 8) | RegisterSet[L]);
            set
            {
                RegisterSet[H] = (byte)(value >> 8);
                RegisterSet[L] = (byte)value;
            }
        }

        /// <summary>
        /// The 16-bit register pair composed of A and F.
        /// </summary>
        /// <remarks>
        /// This register pair is generally not used in code, and is only addressed in this way in EXX.
        /// </remarks>
        public ushort AF
        {
            get => (ushort)((RegisterSet[A] << 8) | RegisterSet[F]);
            set
            {
                RegisterSet[A] = (byte)(value >> 8);
                RegisterSet[F] = (byte)value;
            }
        }
        #endregion

        #region Alternate register pairs
        /// <summary>
        /// The 16-bit register pair composed of B' and C'.
        /// </summary>
        public ushort BC_
        {
            get => (ushort)((RegisterSet[B_] << 8) | RegisterSet[C_]);
            set
            {
                RegisterSet[B_] = (byte)(value >> 8);
                RegisterSet[C_] = (byte)value;
            }
        }

        /// <summary>
        /// The 16-bit register pair composed of D' and E'.
        /// </summary>
        public ushort DE_
        {
            get => (ushort)((RegisterSet[D_] << 8) | RegisterSet[E_]);
            set
            {
                RegisterSet[D_] = (byte)(value >> 8);
                RegisterSet[E_] = (byte)value;
            }
        }

        /// <summary>
        /// The 16-bit register pair composed of H' and L'.
        /// </summary>
        public ushort HL_
        {
            get => (ushort)((RegisterSet[H_] << 8) | RegisterSet[L_]);
            set
            {
                RegisterSet[H_] = (byte)(value >> 8);
                RegisterSet[H_] = (byte)value;
            }
        }

        /// <summary>
        /// The 16-bit register pair composed of A' and F'.
        /// </summary>
        /// <remarks>
        /// This register pair is generally not used in code, and is only addressed in this way in EXX.
        /// </remarks>
        public ushort AF_
        {
            get => (ushort)((RegisterSet[A_] << 8) | RegisterSet[F_]);
            set
            {
                RegisterSet[A_] = (byte)(value >> 8);
                RegisterSet[F_] = (byte)value;
            }
        }
        #endregion

        #region Utility register pairs
        /// <summary>
        /// The stack pointer register.
        /// </summary>
        public ushort SP
        {
            get => (ushort)((RegisterSet[SPi] << 8) | RegisterSet[SPiL]);
            set
            {
                RegisterSet[SPi] = (byte)(value >> 8);
                RegisterSet[SPiL] = (byte)value;
            }
        }

        /// <summary>
        /// The program counter register.
        /// </summary>
        /*public ushort PC
        {
            get => (ushort)((RegisterSet[PCi] << 8) | RegisterSet[PCiL]);
            set
            {
                RegisterSet[PCi] = (byte)(value >> 8);
                RegisterSet[PCiL] = (byte)value;
            }
        }*/
        public ushort PC;
        #endregion

        #region Index register pairs
        /// <summary>
        /// The index X register.
        /// </summary>
        public ushort IX
        {
            get => (ushort)((RegisterSet[IXh] << 8) | RegisterSet[IXl]);
            set
            {
                RegisterSet[IXh] = (byte)(value >> 8);
                RegisterSet[IXl] = (byte)value;
            }
        }

        /// <summary>
        /// The index Y register.
        /// </summary>
        public ushort IY
        {
            get => (ushort)((RegisterSet[IYh] << 8) | RegisterSet[IYl]);
            set
            {
                RegisterSet[IYh] = (byte)(value >> 8);
                RegisterSet[IYl] = (byte)value;
            }
        }
        #endregion
    }
}