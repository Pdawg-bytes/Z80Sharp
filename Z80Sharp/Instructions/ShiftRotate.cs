using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using Z80Sharp.Helpers;
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
            Registers.RegisterSet[F] &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagBits(carry);
            LogInstructionExec("0x07: RLCA");
        }
        private void RLA()
        {
            byte carry = (byte)(Registers.RegisterSet[A] >> 7);
            byte aTemp = Registers.RegisterSet[A];
            aTemp <<= 1;
            aTemp |= (byte)(Registers.RegisterSet[F] & (byte)FlagType.C);
            Registers.RegisterSet[A] = aTemp;
            Registers.RegisterSet[F] &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagBits(carry);
            LogInstructionExec("0x17: RLA");
        }

        private void RRCA()
        {
            byte carry = (byte)(Registers.RegisterSet[A] & 0b00000001);
            Registers.RegisterSet[A] >>= 1;
            Registers.RegisterSet[A] |= (byte)(carry << 7);
            Registers.RegisterSet[F] &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagBits(carry);
            LogInstructionExec("0x0F: RRCA");
        }
        private void RRA()
        {
            byte carry = (byte)(Registers.RegisterSet[A] & 0b00000001);
            byte aTemp = Registers.RegisterSet[A];
            aTemp >>= 1;
            aTemp |= (byte)((Registers.RegisterSet[F] & (byte)FlagType.C) << 7);
            Registers.RegisterSet[A] = aTemp;
            Registers.RegisterSet[F] &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagBits(carry);
            LogInstructionExec("0x17: RRA");
        }

        private void RRD()
        {
            byte memoryValue = _memory.Read(Registers.HL);

            _memory.Write(Registers.HL, (byte)((memoryValue >> 4) | (Registers.RegisterSet[A] << 4)));   // Combine high of (HL) w/ low of A
            Registers.RegisterSet[A] = (byte)((Registers.RegisterSet[A] & 0xF0) | (memoryValue & 0x0F)); // Combine high of A w/ low of (HL)
            byte regA = Registers.RegisterSet[A];

            Registers.SetFlagConditionally(FlagType.S, (regA & 0x80) > 0);              // (S) (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, regA == 0);                      // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.PV, FlagHelpers.CheckParity(regA)); // (PV) (Set if bit parity is even)
            Registers.RegisterSet[F] = (byte)~(FlagType.N | FlagType.H);                // (N, H) (Unconditionally reset)
            Registers.SetFlagConditionally(FlagType.X, (regA & 0x20) > 0);              // (X)  (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (regA & 0x08) > 0);              // (Y)  (Undocumented flag)
            LogInstructionExec("0x67: RRD");
        }
        private void RLD()
        {
            byte memoryValue = _memory.Read(Registers.HL);

            _memory.Write(Registers.HL, (byte)((memoryValue << 4) | (Registers.RegisterSet[A] & 0x0F))); // Combine low of (HL) w/ low of A
            Registers.RegisterSet[A] = (byte)((Registers.RegisterSet[A] & 0xF0) | (memoryValue >> 4));   // Combine high of A w/ high of (HL)
            byte regA = Registers.RegisterSet[A];

            Registers.SetFlagConditionally(FlagType.S, (regA & 0x80) > 0);              // (S) (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, regA == 0);                      // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.PV, FlagHelpers.CheckParity(regA)); // (PV) (Set if bit parity is even)
            Registers.RegisterSet[F] = (byte)~(FlagType.N | FlagType.H);                // (N, H) (Unconditionally reset)
            Registers.SetFlagConditionally(FlagType.X, (regA & 0x20) > 0);              // (X)  (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (regA & 0x08) > 0);              // (Y)  (Undocumented flag)
            LogInstructionExec("0x6F: RLD");
        }
    }
}