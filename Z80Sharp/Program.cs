using System;
using Z80Sharp.Registers;

namespace Z80Sharp
{
    public static class Program
    {
        public static void Main(string[] args)
        {
            byte srVal = 0;
            Console.WriteLine(srVal);
            SRUtils.SetFlag(ref srVal, StatusRegisterFlag.AddSubFlag);
            SRUtils.SetFlag(ref srVal, StatusRegisterFlag.CarryFlag);
            Console.WriteLine(SRUtils.IsFlagSet(srVal, StatusRegisterFlag.AddSubFlag));
            Console.WriteLine(srVal);
            SRUtils.ClearFlag(ref srVal, StatusRegisterFlag.AddSubFlag);
            Console.WriteLine(SRUtils.IsFlagSet(srVal, StatusRegisterFlag.AddSubFlag));
            Console.WriteLine(srVal);
        }
    }
}