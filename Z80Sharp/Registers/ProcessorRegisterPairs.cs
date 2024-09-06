using Z80Sharp.Interfaces;

namespace Z80Sharp.Registers
{
    public partial struct ProcessorRegisters /*: IRegisterSet*/
    {
        #region Register pairs
        /// <summary>
        /// The 16-bit register pair composed of B and C.
        /// </summary>
        public ushort BC
        {
            get => (ushort)(RegisterSet[B] << 8 | RegisterSet[C]);
            set
            {
                RegisterSet[B] = (byte)(value >> 8);
                RegisterSet[C] = (byte)(value & 0xFF);
            }
        }
        /// <summary>
        /// The 16-bit register pair composed of D and E.
        /// </summary>
        public ushort DE
        {
            get => (ushort)(RegisterSet[D] << 8 | RegisterSet[E]);
            set
            {
                RegisterSet[D] = (byte)(value >> 8);
                RegisterSet[E] = (byte)(value & 0xFF);
            }
        }
        /// <summary>
        /// The 16-bit register pair composed of H and L.
        /// </summary>
        public ushort HL
        {
            get => (ushort)(RegisterSet[H] << 8 | RegisterSet[L]);
            set
            {
                RegisterSet[H] = (byte)(value >> 8);
                RegisterSet[L] = (byte)(value & 0xFF);
            }
        }
        /// <summary>
        /// The 16-bit register pair composed of A and F.
        /// </summary>
        public ushort AF
        {
            get => (ushort)(RegisterSet[A] << 8 | RegisterSet[F]);
            set
            {
                RegisterSet[A] = (byte)(value >> 8);
                RegisterSet[F] = (byte)(value & 0xFF);
            }
        }
        #endregion

        #region Alternate register pairs
        /// <summary>
        /// The alternate 16-bit register pair composed of B' and C'.
        /// </summary>
        public ushort HL_
        {
            get => (ushort)(RegisterSet[H_] << 8 | RegisterSet[L_]);
            set
            {
                RegisterSet[H_] = (byte)(value >> 8);
                RegisterSet[L_] = (byte)(value & 0xFF);
            }
        }
        /// <summary>
        /// The alternate 16-bit register pair composed of D' and E'.
        /// </summary>
        public ushort DE_
        {
            get => (ushort)(RegisterSet[D_] << 8 | RegisterSet[E_]);
            set
            {
                RegisterSet[D_] = (byte)(value >> 8);
                RegisterSet[E_] = (byte)(value & 0xFF);
            }
        }
        /// <summary>
        /// The alternate 16-bit register pair composed of B' and C'.
        /// </summary>
        public ushort BC_
        {
            get => (ushort)(RegisterSet[B_] << 8 | RegisterSet[C_]);
            set
            {
                RegisterSet[B_] = (byte)(value >> 8);
                RegisterSet[C_] = (byte)(value & 0xFF);
            }
        }
        /// <summary>
        /// The alternate 16-bit register pair composed of A' and F'.
        /// </summary>
        public ushort AF_
        {
            get => (ushort)(RegisterSet[A_] << 8 | RegisterSet[F_]);
            set
            {
                RegisterSet[A_] = (byte)(value >> 8);
                RegisterSet[F_] = (byte)(value & 0xFF);
            }
        }
        #endregion


        #region Utility register pairs
        public ushort SP
        {
            get => (ushort)(RegisterSet[SPi] << 8 | RegisterSet[SPi + 1]);
            set
            {
                RegisterSet[SPi] = (byte)(value >> 8);
                RegisterSet[SPi + 1] = (byte)(value & 0xFF);
            }
        }
        public ushort PC
        {
            get => (ushort)(RegisterSet[PCi] << 8 | RegisterSet[PCi + 1]);
            set
            {
                RegisterSet[PCi] = (byte)(value >> 8);
                RegisterSet[PCi + 1] = (byte)(value & 0xFF);
            }
        }
        #endregion

        #region Index register pairs
        public ushort IX
        {
            get => (ushort)(RegisterSet[IXi] << 8 | RegisterSet[IXi + 1]);
            set
            {
                RegisterSet[IXi] = (byte)(value >> 8);
                RegisterSet[IXi + 1] = (byte)(value & 0xFF);
            }
        }
        public ushort IY
        {
            get => (ushort)(RegisterSet[IYi] << 8 | RegisterSet[IYi + 1]);
            set
            {
                RegisterSet[IYi] = (byte)(value >> 8);
                RegisterSet[IYi + 1] = (byte)(value & 0xFF);
            }
        }
        #endregion
    }
}