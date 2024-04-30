using Z80Sharp.Helpers;
using Z80Sharp.Interfaces;

namespace Z80Sharp.Registers
{
    /// <summary>
    /// Main implementation of <see cref="IRegisterSet"/>
    /// </summary>
    public class GeneralRegisters : IRegisterSet
    {
        public short BC { get; set; }

        public short DE { get; set; }

        public short HL { get; set; }

        public byte StatusRegister { get; set; }

        private byte _a;
        public byte A 
        { 
            get => _a; 
            set => _a = value; 
        }

        public byte B 
        { 
            get => BC.GetUpperByteShort();
            set;
        }

        public byte C 
        { 
            get; 
            set; 
        }

        public byte D 
        { 
            get; 
            set; 
        }

        public byte E 
        {
            get; 
            set; 
        }

        public byte F 
        { 
            get; 
            set; 
        }

        public byte H 
        { 
            get; 
            set; 
        }

        public byte L 
        { 
            get; 
            set; 
        }
    }
}