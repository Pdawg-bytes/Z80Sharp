namespace Z80Sharp.Registers
{
    /*
     * Reference: https://area51.dev/asm/z80/about/flags/
     */
    public enum StatusRegisterFlag : byte
    {
        CarryFlag = 1 << 0,
        AddSubFlag = 1 << 1,
        ParityFlag = 1 << 2,
        UnusedBit3Flag = 1 << 3,
        HalfCarryFlag = 1 << 4,
        UnusedBit5Flag = 1 << 5,
        ZeroFlag = 1 << 6,
        SignFlag = 1 << 7
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