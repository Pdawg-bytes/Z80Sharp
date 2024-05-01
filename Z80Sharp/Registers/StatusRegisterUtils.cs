namespace Z80Sharp.Registers
{
    /*
     * Reference: https://area51.dev/asm/z80/about/flags/
     */
    public enum StatusRegisterFlag : byte
    {
        /*C*/   CarryFlag = 1 << 0,
        /*N*/   AddSubFlag = 1 << 1,
        /*P/V*/ ParityOverflowFlag = 1 << 2,
        /*F3*/  UnusedBit3Flag = 1 << 3,
        /*H*/   HalfCarryFlag = 1 << 4,
        /*F5*/  UnusedBit5Flag = 1 << 5,
        /*Z*/   ZeroFlag = 1 << 6,
        /*S*/   SignFlag = 1 << 7
    }

    public static class SRUtils
    {
        public static void SetFlag(ref byte statusRegister, StatusRegisterFlag flag)
        {
            statusRegister |= (byte)flag;
        }

        public static void ClearFlag(ref byte statusRegister, StatusRegisterFlag flag)
        {
            statusRegister &= (byte)~flag;
        }

        public static bool IsFlagSet(byte statusRegister, StatusRegisterFlag flag)
        {
            return (statusRegister & (byte)flag) == (byte)flag;
        }
    }
}