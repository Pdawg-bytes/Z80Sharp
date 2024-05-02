namespace Z80Sharp.Enums
{
    /*
     * Reference: https://area51.dev/asm/z80/about/flags/
     */
    public enum StatusRegisterFlag : byte
    {
        /// <summary>
        /// (C): The Carry flag of the Z80.
        /// </summary>
        CarryFlag = 1 << 0,
        /// <summary>
        /// (N): The Negative flag of the Z80.
        /// </summary>
        AddSubFlag = 1 << 1,
        /// <summary>
        /// (P/V): The Parity/Overflow flag of the Z80.
        /// </summary>
        ParityOverflowFlag = 1 << 2,
        /// <summary>
        /// The third flag in the status register is unused.
        /// </summary>
        UnusedBit3Flag = 1 << 3,
        /// <summary>
        /// (H): The Half Carry flag of the Z80.
        /// </summary>
        HalfCarryFlag = 1 << 4,
        /// <summary>
        /// The fifth flag in the status register is unused.
        /// </summary>
        UnusedBit5Flag = 1 << 5,
        /// <summary>
        /// (Z): The Zero flag of the Z80.
        /// </summary>
        ZeroFlag = 1 << 6,
        /// <summary>
        /// (S): The Sign flag of the Z80.
        /// </summary>
        SignFlag = 1 << 7
    }
}