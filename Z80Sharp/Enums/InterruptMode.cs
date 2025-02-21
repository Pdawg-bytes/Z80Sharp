namespace Z80Sharp.Enums
{
    /// <summary>
    /// Represents the interrupt modes available on the Z80.
    /// </summary>
    public enum InterruptMode : byte
    {
        /// <summary>
        /// Interrupt Mode 0 (IM 0): In this mode, the Z80 expects the interrupting device to provide an instruction on the data bus when an interrupt occurs.
        /// </summary>
        IM0 = 0,

        /// <summary>
        /// Interrupt Mode 1 (IM 1): The simplest interrupt mode where the Z80 automatically executes a RST 38h instruction, causing it to call a fixed interrupt service routine located at address 0x0038.
        /// </summary>
        IM1 = 1,

        /// <summary>
        /// Interrupt Mode 2 (IM 2): The most flexible and powerful interrupt mode, allowing for a vectorized interrupt handling mechanism. The Z80 uses the I register and an 8-bit vector provided by the interrupting device to form a pointer to the base of interrupt service routine.
        /// </summary>
        IM2 = 2,
    }
}