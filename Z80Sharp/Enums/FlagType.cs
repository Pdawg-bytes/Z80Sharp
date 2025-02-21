namespace Z80Sharp.Enums
{
    /// <summary>
    /// The bit field (SZYHXPNC) that represents the flags register.
    /// </summary>
    [Flags]
    public enum FlagType : byte
    {
        /// <summary>(C): The Carry flag of the Z80.</summary>
        C = 1 << 0,
        /// <summary>(N): The Negative flag of the Z80.</summary>
        N = 1 << 1,
        /// <summary>(P/V): The Parity/Overflow flag of the Z80.</summary>
        PV = 1 << 2,
        /// <summary>The third flag in the status register is unused.</summary>
        X = 1 << 3,
        /// <summary>(H): The Half Carry flag of the Z80.</summary>
        H = 1 << 4,
        /// <summary>The fifth flag in the status register is unused.</summary>
        Y = 1 << 5,
        /// <summary>(Z): The Zero flag of the Z80.</summary>
        Z = 1 << 6,
        /// <summary>(S): The Sign flag of the Z80.</summary>
        S = 1 << 7
    }
}