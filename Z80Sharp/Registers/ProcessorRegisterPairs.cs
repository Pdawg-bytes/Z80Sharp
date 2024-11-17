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
            get => (ushort)((pRegSet[B] << 8) | pRegSet[C]);
            set
            {
                pRegSet[B] = (byte)(value >> 8);
                pRegSet[C] = (byte)value;
            }
        }

        /// <summary>
        /// The 16-bit register pair composed of D and E.
        /// </summary>
        public ushort DE
        {
            get => (ushort)((pRegSet[D] << 8) | pRegSet[E]);
            set
            {
                pRegSet[D] = (byte)(value >> 8);
                pRegSet[E] = (byte)value;
            }
        }

        /// <summary>
        /// The 16-bit register pair composed of H and L.
        /// </summary>
        public ushort HL
        {
            get => (ushort)((pRegSet[H] << 8) | pRegSet[L]);
            set
            {
                pRegSet[H] = (byte)(value >> 8);
                pRegSet[L] = (byte)value;
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
            get => (ushort)((pRegSet[A] << 8) | pRegSet[F]); // A = 7, F = 6
            set
            {
                pRegSet[A] = (byte)(value >> 8);
                pRegSet[F] = (byte)value;
            }
        }
        #endregion

        #region Alternate register pairs
        /// <summary>
        /// The 16-bit register pair composed of B' and C'.
        /// </summary>
        public ushort BC_
        {
            get => (ushort)((pRegSet[B_] << 8) | pRegSet[C_]);
            set
            {
                pRegSet[B_] = (byte)(value >> 8);
                pRegSet[C_] = (byte)value;
            }
        }

        /// <summary>
        /// The 16-bit register pair composed of D' and E'.
        /// </summary>
        public ushort DE_
        {
            get => (ushort)((pRegSet[D_] << 8) | pRegSet[E_]);
            set
            {
                pRegSet[D_] = (byte)(value >> 8);
                pRegSet[E_] = (byte)value;
            }
        }

        /// <summary>
        /// The 16-bit register pair composed of H' and L'.
        /// </summary>
        public ushort HL_
        {
            get => (ushort)((pRegSet[H_] << 8) | pRegSet[L_]); // Example indices
            set
            {
                pRegSet[H_] = (byte)(value >> 8);
                pRegSet[H_] = (byte)value;
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
            get => (ushort)((pRegSet[A_] << 8) | pRegSet[F_]);
            set
            {
                pRegSet[A_] = (byte)(value >> 8);
                pRegSet[F_] = (byte)value;
            }
        }
        #endregion

        #region Utility register pairs
        /// <summary>
        /// The stack pointer register.
        /// </summary>
        public ushort SP
        {
            get => (ushort)((pRegSet[SPi] << 8) | pRegSet[SPiL]);
            set
            {
                pRegSet[SPi] = (byte)(value >> 8);
                pRegSet[SPiL] = (byte)value;
            }
        }

        /// <summary>
        /// The program counter register.
        /// </summary>
        public ushort PC
        {
            get => (ushort)((pRegSet[PCi] << 8) | pRegSet[PCiL]);
            set
            {
                pRegSet[PCi] = (byte)(value >> 8);
                pRegSet[PCiL] = (byte)value;
            }
        }
        #endregion

        #region Index register pairs
        /// <summary>
        /// The index X register.
        /// </summary>
        public ushort IX
        {
            get => (ushort)((pRegSet[IXh] << 8) | pRegSet[IXl]);
            set
            {
                pRegSet[IXh] = (byte)(value >> 8);
                pRegSet[IXl] = (byte)value;
            }
        }

        /// <summary>
        /// The index Y register.
        /// </summary>
        public ushort IY
        {
            get => (ushort)((pRegSet[IYh] << 8) | pRegSet[IYl]);
            set
            {
                pRegSet[IYh] = (byte)(value >> 8);
                pRegSet[IYl] = (byte)value;
            }
        }
        #endregion
    }
}