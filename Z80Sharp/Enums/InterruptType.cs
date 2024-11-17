namespace Z80Sharp.Enums
{
    /// <summary>
    /// A bit field that represents the state of an interrupt.
    /// </summary>
    [Flags]
    public enum InterruptType : byte
    {
        /// <summary>
        /// Maskable Interrupt
        /// </summary>
        MI = 1 << 0,
        /// <summary>
        /// Non-maskable Interrupt
        /// </summary>
        NMI = 1 << 1
    }
}