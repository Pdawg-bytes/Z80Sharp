using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using Z80Sharp.Helpers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        private void RLCA()
        {
            byte carry = (byte)((Registers.A >> 7) & 0x1);
            Registers.A = (byte)((Registers.A << 1) | carry);
            Registers.F &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagConditionally(FlagType.C, carry != 0);
            //LogInstructionExec("0x07: RLCA");
        }
        private void RLA()
        {
            byte carry = (byte)(Registers.A >> 7);
            byte aTemp = Registers.A;
            aTemp <<= 1;
            aTemp |= (byte)(Registers.F & (byte)FlagType.C);
            Registers.A = aTemp;
            Registers.F &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagBits(carry);
            //LogInstructionExec("0x17: RLA");
        }

        private void RRCA()
        {
            byte carry = (byte)(Registers.A & 0b00000001);
            Registers.A >>= 1;
            Registers.A |= (byte)(carry << 7);
            Registers.F &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagBits(carry);
            //LogInstructionExec("0x0F: RRCA");
        }
        private void RRA()
        {
            byte carry = (byte)(Registers.A & 0b00000001);
            byte aTemp = Registers.A;
            aTemp >>= 1;
            aTemp |= (byte)((Registers.F & (byte)FlagType.C) << 7);
            Registers.A = aTemp;
            Registers.F &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagBits(carry);
            //LogInstructionExec("0x1F: RRA");
        }

        private void RRD()
        {
            byte regA = Registers.A;
            byte memoryValue = _memory.Read(Registers.HL);

            _memory.Write(Registers.HL, (byte)((memoryValue >> 4) | (Registers.A << 4))); // Combine high of (HL) w/ low of A
            Registers.A = regA = (byte)((Registers.A & 0xF0) | (memoryValue & 0x0F));     // Combine high of A w/ low of (HL)

            Registers.SetFlagConditionally(FlagType.S, (regA & 0x80) > 0);  // (S) (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, regA == 0);          // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(regA)); // (PV) (Set if bit parity is even)
            Registers.F &= (byte)~(FlagType.N | FlagType.H);                // (N, H) (Unconditionally reset)
            Registers.SetFlagConditionally(FlagType.X, (regA & 0x20) > 0);  // (X)  (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (regA & 0x08) > 0);  // (Y)  (Undocumented flag)
            //LogInstructionExec("0x67: RRD");
        }
        private void RLD()
        {
            byte regA = Registers.A;
            byte memoryValue = _memory.Read(Registers.HL);

            _memory.Write(Registers.HL, (byte)((memoryValue << 4) | (regA & 0x0F)));        // Combine low of (HL) w/ low of A
            Registers.A = regA = (byte)((regA & 0xF0) | (memoryValue >> 4));   // Combine high of A w/ high of (HL)

            Registers.SetFlagConditionally(FlagType.S, (regA & 0x80) > 0);              // (S) (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, regA == 0);                      // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(regA));             // (PV) (Set if bit parity is even)
            Registers.F &= (byte)~(FlagType.N | FlagType.H);                // (N, H) (Unconditionally reset)
            Registers.SetFlagConditionally(FlagType.X, (regA & 0x20) > 0);              // (X)  (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (regA & 0x08) > 0);              // (Y)  (Undocumented flag)
            //LogInstructionExec("0x6F: RLD");
        }


        private void SLA_R(ref byte operatingRegister)
        {
            byte carry = (byte)(operatingRegister & 0x80); // Extract MSB
            operatingRegister = (byte)(operatingRegister << 1);

            SetFlagsLSH(operatingRegister);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLA R");
        }
        private void SLA_RRMEM(ref ushort operatingRegister)
        {
            byte result = _memory.Read(operatingRegister);
            byte carry = (byte)(result & 0x80); // Extract MSB
            result = (byte)(result << 1);
            _memory.Write(operatingRegister, result);

            SetFlagsLSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLA (RR)");
        }
        private void SLA_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(ird);
            byte carry = (byte)(result & 0x80); // Extract last bit
            result = (byte)(result << 1);
            _memory.Write(ird, result);

            SetFlagsLSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLA (IR + d)");
        }
        private void SLA_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort reg = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x80); // Extract last bit
            result = (byte)(result << 1);
            _memory.Write(reg, result);
            outputRegister = result;

            SetFlagsLSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLA (IR + d), R");
        }

        private void SLL_R(ref byte operatingRegister) // UNDOCUMENTED
        {
            byte carry = (byte)(operatingRegister & 0x80); // Extract MSB
            operatingRegister = (byte)((operatingRegister << 1) | 0x01); // Set LSB to 1

            SetFlagsLSH(operatingRegister);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLL R");
        }
        private void SLL_RRMEM(ref ushort operatingRegister) // UNDOCUMENTED
        {
            byte result = _memory.Read(operatingRegister);
            byte carry = (byte)(result & 0x80); // Extract MSB
            result = (byte)((result << 1) | 0x01); // Set LSB to 1
            _memory.Write(operatingRegister, result);

            SetFlagsLSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLL (RR)");
        }
        private void SLL_IRDMEM(sbyte displacement, ref ushort indexAddressingMode) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(ird);
            byte carry = (byte)(result & 0x80); // Extract MSB
            result = (byte)((result << 1) | 0x01); // Set LSB to 1
            _memory.Write(ird, result);

            SetFlagsLSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLL (IR + d)");
        }
        private void SLL_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort reg = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x80); // Extract MSB
            result = (byte)((result << 1) | 0x01); // Set LSB to 1
            _memory.Write(reg, result);
            outputRegister = result;

            SetFlagsLSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLL (IR + d), R");
        }

        private void SRA_R(ref byte operatingRegister)
        {
            byte carry = (byte)(operatingRegister & 0x01); // Extract LSB for carry
            byte msb = (byte)(operatingRegister & 0x80);   // Preserve sign

            operatingRegister = (byte)((operatingRegister >> 1) | msb);

            SetFlagsRSH(operatingRegister);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRA R");
        }
        private void SRA_RRMEM(ref ushort operatingRegister)
        {
            byte result = _memory.Read(operatingRegister);
            byte carry = (byte)(result & 0x01); // Extract LSB for carry
            byte msb = (byte)(result & 0x80);   // Preserve sign
            result = (byte)((result >> 1) | msb);
            _memory.Write(operatingRegister, result);

            SetFlagsRSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRA (RR)");
        }
        private void SRA_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort reg = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x01); // Extract LSB for carry
            byte msb = (byte)(result & 0x80);   // Preserve sign
            result = (byte)((result >> 1) | msb);
            _memory.Write(reg, result);

            SetFlagsRSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRA (IR + d)");
        }
        private void SRA_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort reg = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x01);   // Extract LSB for carry
            byte msb = (byte)(result & 0x80);     // Preserve sign
            result = (byte)((result >> 1) | msb);
            _memory.Write(reg, result);
            outputRegister = result;

            SetFlagsRSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRA (IR + d), R");
        }

        private void SRL_R(ref byte operatingRegister)
        {
            byte carry = (byte)(operatingRegister & 0x01); // Extract LSB
            operatingRegister = (byte)(operatingRegister >> 1);

            SetFlagsRSH(operatingRegister);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRL R");
        }
        private void SRL_RRMEM(ref ushort operatingRegister)
        {
            byte result = _memory.Read(operatingRegister);
            byte carry = (byte)(result & 0x01); // Extract LSB
            result = (byte)(result >> 1);
            _memory.Write(operatingRegister, result);

            SetFlagsRSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRL (RR)");
        }
        private void SRL_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(ird);
            byte carry = (byte)(result & 0x01); // Extract LSB
            result = (byte)(result >> 1);
            _memory.Write(ird, result);

            SetFlagsRSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRL (IR + d)");
        }
        private void SRL_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(ird);
            byte carry = (byte)(result & 0x01); // Extract LSB
            result = (byte)(result >> 1);
            _memory.Write(ird, result);
            outputRegister = result;

            SetFlagsRSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRL (IR + d), R");
        }


        private void RLC_R(ref byte operatingRegister)
        {
            byte carry = (byte)((operatingRegister & 0x80) >> 7); // Extract last bit and rotate it
            operatingRegister = (byte)((operatingRegister << 1) | carry);

            SetFlagsRotate(operatingRegister);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RLC R");
        }
        private void RLC_RRMEM(ref ushort operatingRegister)
        {
            byte result = _memory.Read(operatingRegister);
            byte carry = (byte)((result & 0x80) >> 7); // Extract last bit and rotate it
            result = (byte)((result << 1) | carry);
            _memory.Write(operatingRegister, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RLC (RR)");
        }
        private void RLC_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(ird);
            byte carry = (byte)((result & 0x80) >> 7); // Extract last bit and rotate it
            result = (byte)((result << 1) | carry);
            _memory.Write(ird, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RLC (IR + d)");
        }
        private void RLC_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(ird);
            byte carry = (byte)((result & 0x80) >> 7); // Extract last bit and rotate it
            result = (byte)((result << 1) | carry);
            _memory.Write(ird, result);
            outputRegister = result;

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RLC (IR + d), R");
        }

        private void RL_R(ref byte operatingRegister)
        {
            byte carry = (byte)((operatingRegister & 0x80) >> 7); // Extract last bit and rotate it
            operatingRegister = (byte)((operatingRegister << 1) | (byte)(Registers.F & 0x01)); // Rotate through old carry flag

            SetFlagsRotate(operatingRegister);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RL R");
        }
        private void RL_RRMEM(ref ushort operatingRegister)
        {
            byte result = _memory.Read(operatingRegister);
            byte carry = (byte)((result & 0x80) >> 7); // Extract last bit and rotate it
            result = (byte)((result << 1) | (byte)(Registers.F & 0x01)); // Rotate through old carry flag
            _memory.Write(operatingRegister, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RL (RR)");
        }
        private void RL_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(ird);
            byte carry = (byte)((result & 0x80) >> 7); // Extract last bit and rotate it
            result = (byte)((result << 1) | (byte)(Registers.F & 0x01)); // Rotate through old carry flag
            _memory.Write(ird, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RL (IR + d)");
        }
        private void RL_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(ird);
            byte carry = (byte)((result & 0x80) >> 7); // Extract last bit and rotate it
            result = (byte)((result << 1) | (byte)(Registers.F & 0x01)); // Rotate through old carry flag
            _memory.Write(ird, result);
            outputRegister = result;

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RL (IR + d), R");
        }


        private void RRC_R(ref byte operatingRegister)
        {
            byte carry = (byte)((operatingRegister & 0x01) << 7); // Extract first bit and rotate it
            operatingRegister = (byte)((operatingRegister >> 1) | carry);

            SetFlagsRotate(operatingRegister);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is not 0)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RRC R");
        }
        private void RRC_RRMEM(ref ushort operatingRegister)
        {
            byte result = _memory.Read(operatingRegister);
            byte carry = (byte)((result & 0x01) << 7); // Extract first bit and rotate it
            result = (byte)((result >> 1) | carry);
            _memory.Write(operatingRegister, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is not 0)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RRC (RR)");
        }
        private void RRC_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(ird);
            byte carry = (byte)((result & 0x01) << 7); // Extract first bit and rotate it
            result = (byte)((result >> 1) | carry);
            _memory.Write(ird, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RRC (IR + d)");
        }
        private void RRC_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(ird);
            byte carry = (byte)((result & 0x01) << 7); // Extract first bit and rotate it
            result = (byte)((result >> 1) | carry);
            _memory.Write(ird, result);
            outputRegister = result;

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RRC (IR + d), R");
        }

        private void RR_R(ref byte operatingRegister)
        {
            byte carry = (byte)((operatingRegister & 0x01) << 7); // Extract first bit and rotate it
            operatingRegister = (byte)((operatingRegister >> 1) | (byte)(Registers.F & 0x01)); // Rotate through old carry flag

            SetFlagsRotate(operatingRegister);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is not 0)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RR R");
        }
        private void RR_RRMEM(ref ushort operatingRegister)
        {
            byte result = _memory.Read(operatingRegister);
            byte carry = (byte)((result & 0x01) << 7); // Extract first bit and rotate it
            result = (byte)((result >> 1) | (byte)(Registers.F & 0x01)); // Rotate through old carry flag
            _memory.Write(operatingRegister, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is not 0)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RR (RR)");
        }
        private void RR_IRDMEM(sbyte displacement, ref ushort indexAddressingMode)
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(ird);
            byte carry = (byte)((result & 0x01) << 7); // Extract first bit and rotate it
            result = (byte)((result >> 1) | (byte)(Registers.F & 0x01)); // Rotate through old carry flag
            _memory.Write(ird, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry > 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RR (IR + d)");
        }
        private void RR_IRDMEM_R(sbyte displacement, ref ushort indexAddressingMode, ref byte outputRegister) // UNDOCUMENTED
        {
            ushort ird = (ushort)(indexAddressingMode + displacement);

            byte result = _memory.Read(ird);
            byte carry = (byte)((result & 0x01) << 7); // Extract first bit and rotate it
            result = (byte)((result >> 1) | (byte)(Registers.F & 0x01)); // Rotate through old carry flag
            _memory.Write(ird, result);
            outputRegister = result;

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RR (IR + d), R");
        }
    }
}