namespace Z80Sharp.Interfaces
{
    public interface IMemory
    {
        byte Read(ushort address);
        void Write(ushort address, byte value);

        ushort ReadWord(ushort address);
        void WriteWord(ushort address, ushort value);

        /// <remarks>
        /// This value should never exceed 65536.
        /// </remarks>
        uint Length { get; }
    }
}