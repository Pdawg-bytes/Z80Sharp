using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private void RLCA()
        {
            byte carry = (byte)(Registers.RegisterSet[A] >> 7);
            Registers.RegisterSet[A] <<= 1;
            Registers.RegisterSet[A] |= (byte)(carry << 7);
            Registers.RegisterSet[F] &= (byte)~(StatusRegisterFlag.N | StatusRegisterFlag.H | StatusRegisterFlag.C);
            Registers.SetFlagBits(carry);
            LogInstructionExec("0x07: RLCA");
        }
        private void RLA()
        {
            byte carry = (byte)(Registers.RegisterSet[A] >> 7);
            byte aTemp = Registers.RegisterSet[A];
            aTemp <<= 1;
            aTemp |= (byte)(Registers.RegisterSet[F] & (byte)StatusRegisterFlag.C);
            Registers.RegisterSet[A] = aTemp;
            Registers.RegisterSet[F] &= (byte)~(StatusRegisterFlag.N | StatusRegisterFlag.H | StatusRegisterFlag.C);
            Registers.SetFlagBits(carry);
            LogInstructionExec("0x17: RLA");
        }

        private void RRCA()
        {
            byte carry = (byte)(Registers.RegisterSet[A] & 0b00000001);
            Registers.RegisterSet[A] >>= 1;
            Registers.RegisterSet[A] |= (byte)(carry << 7);
            Registers.RegisterSet[F] &= (byte)~(StatusRegisterFlag.N | StatusRegisterFlag.H | StatusRegisterFlag.C);
            Registers.SetFlagBits(carry);
            LogInstructionExec("0x0F: RRCA");
        }
        private void RRA()
        {
            byte carry = (byte)(Registers.RegisterSet[A] & 0b00000001);
            byte aTemp = Registers.RegisterSet[A];
            aTemp >>= 1;
            aTemp |= (byte)((Registers.RegisterSet[F] & (byte)StatusRegisterFlag.C) << 7);
            Registers.RegisterSet[A] = aTemp;
            Registers.RegisterSet[F] &= (byte)~(StatusRegisterFlag.N | StatusRegisterFlag.H | StatusRegisterFlag.C);
            Registers.SetFlagBits(carry);
            LogInstructionExec("0x17: RRA");
        }
    }
}
