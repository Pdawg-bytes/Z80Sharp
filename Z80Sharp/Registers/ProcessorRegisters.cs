using Z80Sharp.Helpers;
using Z80Sharp.Interfaces;

namespace Z80Sharp.Registers
{
    /// <summary>
    /// Main implementation of <see cref="IRegisterSet"/>
    /// </summary>
    public struct ProcessorRegisters : IRegisterSet
    {
        #region Utility registers
        public ushort SP { get; set; }
        public ushort PC { get; set; }

        public byte I { get; set; }
        public byte R { get; set; }

        public bool IFF1 { get; set; }
        public bool IFF2 { get; set; }
        #endregion


        #region General-purpose 16-bit pairs
        public ushort BC { get; set; }
        public ushort DE { get; set; }
        public ushort HL { get; set; }
        #endregion


        #region Index registers
        public ushort IX { get; set; }
        public ushort IY { get; set; }

        public byte IXH
        {
            get => IX.GetUpperByte();
            set => IX.SetUpperByte(value);
        }
        public byte IXL
        {
            get => IX.GetLowerByte();
            set => IX.SetLowerByte(value);
        }

        public byte IYH
        {
            get => IY.GetUpperByte();
            set => IY.SetUpperByte(value);
        }
        public byte IYL
        {
            get => IY.GetLowerByte();
            set => IY.SetLowerByte(value);
        }
        #endregion


        #region Flags register
        private byte _f;
        public byte F 
        { 
            get => _f; 
            set => _f = value; 
        }
        #endregion


        #region Accumulator
        private byte _a;
        public byte A 
        { 
            get => _a; 
            set => _a = value; 
        }
        #endregion


        #region General-purpose 8-bit registers
        public byte B 
        { 
            get => BC.GetUpperByte();
            set => BC.SetUpperByte(value);
        }

        public byte C 
        {
            get => BC.GetLowerByte();
            set => BC.SetLowerByte(value);
        }

        public byte D 
        {
            get => DE.GetUpperByte();
            set => DE.SetUpperByte(value);
        }

        public byte E 
        {
            get => DE.GetLowerByte();
            set => DE.SetLowerByte(value);
        }
        #endregion


        # region Composition of HL
        public byte H 
        { 
            get => HL.GetUpperByte(); 
            set => HL.SetLowerByte(value); 
        }

        public byte L 
        { 
            get => HL.GetLowerByte(); 
            set => HL.SetLowerByte(value); 
        }
        #endregion
    }
}