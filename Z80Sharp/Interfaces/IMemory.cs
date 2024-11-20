namespace Z80Sharp.Interfaces
{
    public interface IMemory
    {
        byte Read(int address);
        void Write(int address, byte value);

        ushort ReadWord(int address);
        void WriteWord(int address, ushort value);

        /// <remarks>
        /// This value should never exceed 65536.
        /// </remarks>
        uint Length { get; }
    }
}