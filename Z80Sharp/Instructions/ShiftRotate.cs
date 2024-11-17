using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
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
            byte carry = (byte)((Registers.RegisterSet[A] >> 7) & 0x1);
            Registers.RegisterSet[A] = (byte)((Registers.RegisterSet[A] << 1) | carry);
            Registers.RegisterSet[F] &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagConditionally(FlagType.C, carry != 0);
            //LogInstructionExec("0x07: RLCA");
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
            //LogInstructionExec("0x17: RLA");
        }

        private void RRCA()
        {
            byte carry = (byte)(Registers.RegisterSet[A] & 0b00000001);
            Registers.RegisterSet[A] >>= 1;
            Registers.RegisterSet[A] |= (byte)(carry << 7);
            Registers.RegisterSet[F] &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            Registers.SetFlagBits(carry);
            //LogInstructionExec("0x0F: RRCA");
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
            //LogInstructionExec("0x1F: RRA");
        }

        private void RRD()
        {
            byte regA = Registers.RegisterSet[A];
            byte memoryValue = _memory.Read(Registers.HL);

            _memory.Write(Registers.HL, (byte)((memoryValue >> 4) | (Registers.RegisterSet[A] << 4)));          // Combine high of (HL) w/ low of A
            Registers.RegisterSet[A] = regA = (byte)((Registers.RegisterSet[A] & 0xF0) | (memoryValue & 0x0F)); // Combine high of A w/ low of (HL)

            Registers.SetFlagConditionally(FlagType.S, (regA & 0x80) > 0);              // (S) (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, regA == 0);                      // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(regA)); // (PV) (Set if bit parity is even)
            Registers.RegisterSet[F] = (byte)~(FlagType.N | FlagType.H);                // (N, H) (Unconditionally reset)
            Registers.SetFlagConditionally(FlagType.X, (regA & 0x20) > 0);              // (X)  (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (regA & 0x08) > 0);              // (Y)  (Undocumented flag)
            //LogInstructionExec("0x67: RRD");
        }
        private void RLD()
        {
            byte regA = Registers.RegisterSet[A];
            byte memoryValue = _memory.Read(Registers.HL);

            _memory.Write(Registers.HL, (byte)((memoryValue << 4) | (regA & 0x0F)));        // Combine low of (HL) w/ low of A
            Registers.RegisterSet[A] = regA = (byte)((regA & 0xF0) | (memoryValue >> 4));   // Combine high of A w/ high of (HL)

            Registers.SetFlagConditionally(FlagType.S, (regA & 0x80) > 0);              // (S) (Set if negative)
            Registers.SetFlagConditionally(FlagType.Z, regA == 0);                      // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(regA));             // (PV) (Set if bit parity is even)
            Registers.RegisterSet[F] = (byte)~(FlagType.N | FlagType.H);                // (N, H) (Unconditionally reset)
            Registers.SetFlagConditionally(FlagType.X, (regA & 0x20) > 0);              // (X)  (Undocumented flag)
            Registers.SetFlagConditionally(FlagType.Y, (regA & 0x08) > 0);              // (Y)  (Undocumented flag)
            //LogInstructionExec("0x6F: RLD");
        }


        private void SLA_R(byte operatingRegister)
        {
            byte reg = Registers.RegisterSet[operatingRegister];
            byte carry = (byte)(reg & 0x80); // Extract MSB
            Registers.RegisterSet[operatingRegister] = reg = (byte)(reg << 1);

            SetFlagsLSH(reg);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLA {Registers.RegisterName(operatingRegister)}");
        }
        private void SLA_RRMEM(byte operatingRegister)
        {
            ushort reg = Registers.GetR16FromHighIndexer(operatingRegister);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x80); // Extract last bit
            result = (byte)(result << 1);
            _memory.Write(reg, result);

            SetFlagsLSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLA ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void SLA_IRDMEM(sbyte displacement, byte indexAddressingMode)
        {
            ushort reg = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x80); // Extract last bit
            result = (byte)(result << 1);
            _memory.Write(reg, result);

            SetFlagsLSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLA ({Registers.RegisterName(indexAddressingMode, true)} + d)");
        }
        private void SLA_IRDMEM_R(sbyte displacement, byte indexAddressingMode, byte outputRegister) // UNDOCUMENTED
        {
            ushort reg = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x80); // Extract last bit
            result = (byte)(result << 1);
            _memory.Write(reg, result);
            Registers.RegisterSet[outputRegister] = result;

            SetFlagsLSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLA ({Registers.RegisterName(indexAddressingMode, true)} + d), {Registers.RegisterName(outputRegister)}");
        }

        private void SLL_R(byte operatingRegister) // UNDOCUMENTED
        {
            byte reg = Registers.RegisterSet[operatingRegister];
            byte carry = (byte)(reg & 0x80); // Extract MSB
            Registers.RegisterSet[operatingRegister] = reg = (byte)((reg << 1) | 0x01); // Set LSB to 1

            SetFlagsLSH(reg);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLL {Registers.RegisterName(operatingRegister)}");
        }
        private void SLL_RRMEM(byte operatingRegister) // UNDOCUMENTED
        {
            ushort reg = Registers.GetR16FromHighIndexer(operatingRegister);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x80); // Extract MSB
            result = (byte)((result << 1) | 0x01); // Set LSB to 1
            _memory.Write(reg, result);

            SetFlagsLSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLL ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void SLL_IRDMEM(sbyte displacement, byte indexAddressingMode) // UNDOCUMENTED
        {
            ushort reg = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x80); // Extract MSB
            result = (byte)((result << 1) | 0x01); // Set LSB to 1
            _memory.Write(reg, result);

            SetFlagsLSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLL ({Registers.RegisterName(indexAddressingMode, true)} + d)");
        }
        private void SLL_IRDMEM_R(sbyte displacement, byte indexAddressingMode, byte outputRegister) // UNDOCUMENTED
        {
            ushort reg = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x80); // Extract MSB
            result = (byte)((result << 1) | 0x01); // Set LSB to 1
            _memory.Write(reg, result);
            Registers.RegisterSet[outputRegister] = result;

            SetFlagsLSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SLL ({Registers.RegisterName(indexAddressingMode, true)} + d), {Registers.RegisterName(outputRegister)}");
        }

        private void SRA_R(byte operatingRegister)
        {
            byte reg = Registers.RegisterSet[operatingRegister];
            byte carry = (byte)(reg & 0x01); // Extract LSB for carry
            byte msb = (byte)(reg & 0x80);   // Preserve sign

            Registers.RegisterSet[operatingRegister] = reg = (byte)((reg >> 1) | msb);

            SetFlagsRSH(reg);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRA {Registers.RegisterName(operatingRegister)}");
        }
        private void SRA_RRMEM(byte operatingRegister)
        {
            ushort reg = Registers.GetR16FromHighIndexer(operatingRegister);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x01); // Extract LSB for carry
            byte msb = (byte)(result & 0x80);   // Preserve sign
            result = (byte)((result >> 1) | msb);
            _memory.Write(reg, result);

            SetFlagsRSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRA {Registers.RegisterName(operatingRegister)}");
        }
        private void SRA_IRDMEM(sbyte displacement, byte indexAddressingMode)
        {
            ushort reg = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x01); // Extract LSB for carry
            byte msb = (byte)(result & 0x80);   // Preserve sign
            result = (byte)((result >> 1) | msb);
            _memory.Write(reg, result);

            SetFlagsRSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRA ({Registers.RegisterName(indexAddressingMode, true)} + d)");
        }
        private void SRA_IRDMEM_R(sbyte displacement, byte indexAddressingMode, byte outputRegister) // UNDOCUMENTED
        {
            ushort reg = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x01);   // Extract LSB for carry
            byte msb = (byte)(result & 0x80);     // Preserve sign
            result = (byte)((result >> 1) | msb);
            _memory.Write(reg, result);
            Registers.RegisterSet[outputRegister] = result;

            SetFlagsRSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRA ({Registers.RegisterName(indexAddressingMode, true)} + d), {Registers.RegisterName(outputRegister)}");
        }

        private void SRL_R(byte operatingRegister)
        {
            byte reg = Registers.RegisterSet[operatingRegister];
            byte carry = (byte)(reg & 0x01); // Extract LSB
            Registers.RegisterSet[operatingRegister] = reg = (byte)(reg >> 1);

            SetFlagsRSH(reg);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRL {Registers.RegisterName(operatingRegister)}");
        }
        private void SRL_RRMEM(byte operatingRegister)
        {
            ushort reg = Registers.GetR16FromHighIndexer(operatingRegister);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x01); // Extract LSB
            result = (byte)(result >> 1);
            _memory.Write(reg, result);

            SetFlagsRSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRL ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void SRL_IRDMEM(sbyte displacement, byte indexAddressingMode)
        {
            ushort reg = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x01); // Extract LSB
            result = (byte)(result >> 1);
            _memory.Write(reg, result);

            SetFlagsRSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRL ({Registers.RegisterName(indexAddressingMode, true)} + d)");
        }
        private void SRL_IRDMEM_R(sbyte displacement, byte indexAddressingMode, byte outputRegister) // UNDOCUMENTED
        {
            ushort reg = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(reg);
            byte carry = (byte)(result & 0x01); // Extract LSB
            result = (byte)(result >> 1);
            _memory.Write(reg, result);
            Registers.RegisterSet[outputRegister] = result;

            SetFlagsRSH(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: SRL ({Registers.RegisterName(indexAddressingMode, true)} + d), {Registers.RegisterName(outputRegister)}");
        }


        private void RLC_R(byte operatingRegister)
        {
            byte reg = Registers.RegisterSet[operatingRegister];
            byte carry = (byte)((reg & 0x80) >> 7); // Extract last bit and rotate it
            Registers.RegisterSet[operatingRegister] = reg = (byte)((reg << 1) | carry);

            SetFlagsRotate(reg);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RLC {Registers.RegisterName(operatingRegister)}");
        }
        private void RLC_RRMEM(byte operatingRegister)
        {
            ushort reg = Registers.GetR16FromHighIndexer(operatingRegister);

            byte result = _memory.Read(reg);
            byte carry = (byte)((result & 0x80) >> 7); // Extract last bit and rotate it
            result = (byte)((result << 1) | carry);
            _memory.Write(reg, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RLC ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void RLC_IRDMEM(sbyte displacement, byte indexAddressingMode)
        {
            ushort irdMem = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(irdMem);
            byte carry = (byte)((result & 0x80) >> 7); // Extract last bit and rotate it
            result = (byte)((result << 1) | carry);
            _memory.Write(irdMem, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RLC ({Registers.RegisterName(indexAddressingMode, true)})");
        }
        private void RLC_IRDMEM_R(sbyte displacement, byte indexAddressingMode, byte outputRegister) // UNDOCUMENTED
        {
            ushort irdMem = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(irdMem);
            byte carry = (byte)((result & 0x80) >> 7); // Extract last bit and rotate it
            result = (byte)((result << 1) | carry);
            _memory.Write(irdMem, result);
            Registers.RegisterSet[outputRegister] = result;

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RLC ({Registers.RegisterName(indexAddressingMode, true)})");
        }

        private void RL_R(byte operatingRegister)
        {
            byte reg = Registers.RegisterSet[operatingRegister];
            byte carry = (byte)((reg & 0x80) >> 7); // Extract last bit and rotate it
            Registers.RegisterSet[operatingRegister] = reg = (byte)((reg << 1) | (byte)(Registers.RegisterSet[F] & 0x01)); // Rotate through old carry flag

            SetFlagsRotate(reg);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RL {Registers.RegisterName(operatingRegister)}");
        }
        private void RL_RRMEM(byte operatingRegister)
        {
            ushort reg = Registers.GetR16FromHighIndexer(operatingRegister);

            byte result = _memory.Read(reg);
            byte carry = (byte)((result & 0x80) >> 7); // Extract last bit and rotate it
            result = (byte)((result << 1) | (byte)(Registers.RegisterSet[F] & 0x01)); // Rotate through old carry flag
            _memory.Write(reg, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RL ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void RL_IRDMEM(sbyte displacement, byte indexAddressingMode)
        {
            ushort irdMem = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(irdMem);
            byte carry = (byte)((result & 0x80) >> 7); // Extract last bit and rotate it
            result = (byte)((result << 1) | (byte)(Registers.RegisterSet[F] & 0x01)); // Rotate through old carry flag
            _memory.Write(irdMem, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RL ({Registers.RegisterName(indexAddressingMode, true)} + d)");
        }
        private void RL_IRDMEM_R(sbyte displacement, byte indexAddressingMode, byte outputRegister) // UNDOCUMENTED
        {
            ushort irdMem = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(irdMem);
            byte carry = (byte)((result & 0x80) >> 7); // Extract last bit and rotate it
            result = (byte)((result << 1) | (byte)(Registers.RegisterSet[F] & 0x01)); // Rotate through old carry flag
            _memory.Write(irdMem, result);
            Registers.RegisterSet[outputRegister] = result;

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RL ({Registers.RegisterName(indexAddressingMode, true)} + d), {Registers.RegisterName(outputRegister)}");
        }


        private void RRC_R(byte operatingRegister)
        {
            byte reg = Registers.RegisterSet[operatingRegister];
            byte carry = (byte)((reg & 0x01) << 7); // Extract first bit and rotate it
            Registers.RegisterSet[operatingRegister] = reg = (byte)((reg >> 1) | carry);

            SetFlagsRotate(reg);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is not 0)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RRC {Registers.RegisterName(operatingRegister)}");
        }
        private void RRC_RRMEM(byte operatingRegister)
        {
            ushort reg = Registers.GetR16FromHighIndexer(operatingRegister);

            byte result = _memory.Read(reg);
            byte carry = (byte)((result & 0x01) << 7); // Extract first bit and rotate it
            result = (byte)((result >> 1) | carry);
            _memory.Write(reg, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is not 0)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RRC ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void RRC_IRDMEM(sbyte displacement, byte indexAddressingMode)
        {
            ushort irdMem = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(irdMem);
            byte carry = (byte)((result & 0x01) << 7); // Extract first bit and rotate it
            result = (byte)((result >> 1) | carry);
            _memory.Write(irdMem, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RRC ({Registers.RegisterName(indexAddressingMode, true)} + d)");
        }
        private void RRC_IRDMEM_R(sbyte displacement, byte indexAddressingMode, byte outputRegister) // UNDOCUMENTED
        {
            ushort irdMem = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(irdMem);
            byte carry = (byte)((result & 0x01) << 7); // Extract first bit and rotate it
            result = (byte)((result >> 1) | carry);
            _memory.Write(irdMem, result);
            Registers.RegisterSet[outputRegister] = result;

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RRC ({Registers.RegisterName(indexAddressingMode, true)} + d), {Registers.RegisterName(outputRegister)}");
        }

        private void RR_R(byte operatingRegister)
        {
            byte reg = Registers.RegisterSet[operatingRegister];
            byte carry = (byte)((reg & 0x01) << 7); // Extract first bit and rotate it
            Registers.RegisterSet[operatingRegister] = reg = (byte)((reg >> 1) | (byte)(Registers.RegisterSet[F] & 0x01)); // Rotate through old carry flag

            SetFlagsRotate(reg);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is not 0)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RR {Registers.RegisterName(operatingRegister)}");
        }
        private void RR_RRMEM(byte operatingRegister)
        {
            ushort reg = Registers.GetR16FromHighIndexer(operatingRegister);

            byte result = _memory.Read(reg);
            byte carry = (byte)((result & 0x01) << 7); // Extract first bit and rotate it
            result = (byte)((result >> 1) | (byte)(Registers.RegisterSet[F] & 0x01)); // Rotate through old carry flag
            _memory.Write(reg, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry != 0); // (C) (Set if carried LSB is not 0)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RR ({Registers.RegisterName(operatingRegister, true)})");
        }
        private void RR_IRDMEM(sbyte displacement, byte indexAddressingMode)
        {
            ushort irdMem = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(irdMem);
            byte carry = (byte)((result & 0x01) << 7); // Extract first bit and rotate it
            result = (byte)((result >> 1) | (byte)(Registers.RegisterSet[F] & 0x01)); // Rotate through old carry flag
            _memory.Write(irdMem, result);

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RR ({Registers.RegisterName(indexAddressingMode, true)} + d)");
        }
        private void RR_IRDMEM_R(sbyte displacement, byte indexAddressingMode, byte outputRegister) // UNDOCUMENTED
        {
            ushort irdMem = (ushort)(Registers.GetR16FromHighIndexer(indexAddressingMode) + displacement);

            byte result = _memory.Read(irdMem);
            byte carry = (byte)((result & 0x01) << 7); // Extract first bit and rotate it
            result = (byte)((result >> 1) | (byte)(Registers.RegisterSet[F] & 0x01)); // Rotate through old carry flag
            _memory.Write(irdMem, result);
            Registers.RegisterSet[outputRegister] = result;

            SetFlagsRotate(result);
            Registers.SetFlagConditionally(FlagType.C, carry == 1); // (C) (Set if carried MSB is 1)

            //LogInstructionExec($"0x{_currentInstruction:X2}: RR ({Registers.RegisterName(indexAddressingMode, true)} + d), {Registers.RegisterName(outputRegister)}");
        }
    }
}