namespace Z80Sharp.Constants
{
    internal static class ConditionCodes
    {
        /// <summary>
        /// The non-zero condition.
        /// </summary>
        internal const byte NZ_C = 0b000;
        /// <summary>
        /// The zero condition.
        /// </summary>
        internal const byte Z_C = 0b001;
        /// <summary>
        /// The non-carry condition.
        /// </summary>
        internal const byte NC_C = 0b010;
        /// <summary>
        /// The carry condition.
        /// </summary>
        internal const byte C_C = 0b011;
        /// <summary>
        /// The parity/overflow unset condition.
        /// </summary>
        internal const byte PO_C = 0b100;
        /// <summary>
        /// The parity/overflow set condition.
        /// </summary>
        internal const byte PE_C = 0b101;
        /// <summary>
        /// The sign unset condition.
        /// </summary>
        internal const byte P_C = 0b110;
        /// <summary>
        /// The sign set condition.
        /// </summary>
        internal const byte M_C = 0b111;
    }
}