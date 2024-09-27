using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using static Z80Sharp.Registers.ProcessorRegisters;
using static Z80Sharp.Helpers.FlagHelpers;
using System.Xml.Serialization;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        /// <summary>
        /// Increments the input value and sets the flags register accordingly.
        /// </summary>
        /// <param name="increment">The value that will be incremented.</param>
        /// <returns>The incremented value.</returns>
        private byte INCWithFlags(byte increment)
        {
            // https://www.zilog.com/docs/z80/um0080.pdf Flags detailed on page 179.
            byte sum = (byte)(increment + 1);

            Registers.SetFlagConditionally(FlagType.S, (sum & 0x80) > 0);           // (S) (check 7th bit for sign)
            Registers.SetFlagConditionally(FlagType.Z, sum == 0);                   // (Z) (set if result is zero)
            Registers.SetFlagConditionally(FlagType.H, (increment & 0x0F) == 0x0F); // (H) (set if carry from bit 3 to bit 4)
            Registers.SetFlagConditionally(FlagType.PV, increment == 0x7F);         // (P/V) (set on signed overflow)
            Registers.ClearFlag(FlagType.N); // Clear the N flag (as INC is not a subtraction)

            return sum;
        }
        /// <summary>
        /// Decrements the input value and sets the flags register accordingly.
        /// </summary>
        /// <param name="decrement">The value that will be decremented.</param>
        /// <returns>The decremented value.</returns>
        private byte DECWithFlags(byte decrement)
        {
            // https://www.zilog.com/docs/z80/um0080.pdf Flags detailed on page 185.
            byte diff = (byte)(decrement - 1);

            Registers.SetFlagConditionally(FlagType.PV, decrement == 0x80);      // (P/V) (set on signed overflow -128 -> 127)
            Registers.SetFlagConditionally(FlagType.S, (diff & 0x80) > 0);       // (S) (check 7th bit for sign)
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);               // (Z) (set if result is zero)
            Registers.SetFlagConditionally(FlagType.H, (decrement & 0x0F) == 0); // (H) (borrow from bit 4)
            Registers.SetFlag(FlagType.N); // Set the N flag (subtract flag)

            return diff;
        }

        /// <summary>
        /// Performs a bitwise OR on the Accumulator with <paramref name="operand"/>.
        /// </summary>
        /// <param name="operand">The operand which we OR A with.</param>
        private void ORAny(byte operand)
        {
            Registers.RegisterSet[A] |= operand;
            byte result = Registers.RegisterSet[A];
            Registers.SetFlagConditionally(FlagType.S, (result & 0x80) > 0);  // (S) (check 7th bit for sign)
            Registers.SetFlagConditionally(FlagType.Z, result == 0);          // (Z) (set if result is zero)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result)); // (PV) (set if parity)
            Registers.RegisterSet[F] &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            // H, N, and C are unconditionally reset as per page 174 of https://www.zilog.com/docs/z80/um0080.pdf
        }
        /// <summary>
        /// Performs a bitwise XOR on the Accumulator with <paramref name="operand"/>.
        /// </summary>
        /// <param name="operand">The operand which we XOR A with.</param>
        private void XORAny(byte operand)
        {
            Registers.RegisterSet[A] ^= operand;
            byte result = Registers.RegisterSet[A];
            Registers.SetFlagConditionally(FlagType.S, (result & 0x80) > 0);  // (S) (check 7th bit for sign)
            Registers.SetFlagConditionally(FlagType.Z, result == 0);          // (Z) (set if result is zero)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result)); // (PV) (set if parity)
            Registers.RegisterSet[F] &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
            // H, N, and C are unconditionally reset as per page 175 of https://www.zilog.com/docs/z80/um0080.pdf
        }

        /// <summary>
        /// Performs a bitwise AND on the Accumulator with <paramref name="operand"/>.
        /// </summary>
        /// <param name="operand">The operand which we AND A with.</param>
        private void ANDAny(byte operand)
        {
            Registers.RegisterSet[A] &= operand;
            byte result = Registers.RegisterSet[A];
            Registers.SetFlagConditionally(FlagType.S, (result & 0x80) > 0);  // (S) (check 7th bit for sign)
            Registers.SetFlagConditionally(FlagType.Z, result == 0);          // (Z) (set if result is zero)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result)); // (PV) (set if parity)
            Registers.RegisterSet[F] &= (byte)~(FlagType.N | FlagType.C);
            Registers.SetFlag(FlagType.H);
            // H is unconditionally set; N and C are unconditionally reset as per page 172 of https://www.zilog.com/docs/z80/um0080.pdf
        }

        /// <summary>
        /// Performs a comparison on A with <paramref name="operand"/> and sets flags accordingly.
        /// </summary>
        /// <param name="operand">The value to subtract from A.</param>
        /// <remarks>A is not modified during this instruction.</remarks>
        private void CMPAny(byte operand)
        {
            byte regA = Registers.RegisterSet[A];
            byte diff = (byte)(regA - operand);

            Registers.SetFlagConditionally(FlagType.S, (diff & 0x80) != 0);             // (S) (check 7th bit for sign)
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);                      // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.H, (regA & 0xF) < (operand & 0xF)); // (H) (Set if borrow occurs from bit 4)

            Registers.SetFlagConditionally(FlagType.PV,
                (((regA ^ operand) & 0x80) == 0) // Check if regA and operand have the same sign (both positive or both negative)
                && 
                (((regA ^ diff) & 0x80) != 0));  // Check if diff. has a different sign from regA

            Registers.SetFlagConditionally(FlagType.C, (diff & 0x100) != 0); // (C) (Set if borrow occurs from bit 8)
            Registers.SetFlag(FlagType.N);
            // N is unconditionally set as per page 178 of https://www.zilog.com/docs/z80/um0080.pdf
        }

        /// <summary>
        /// Adds <paramref name="operand"/> to A and sets flags accordingly.
        /// </summary>
        /// <param name="operand">The value to add to A.</param>
        private void ADDAny(byte operand)
        {
            byte regA = Registers.RegisterSet[A];
            int sum = regA + operand;
            Registers.RegisterSet[A] = (byte)sum;

            Registers.SetFlagConditionally(FlagType.S, (sum & 0x80) != 0);                      // (S) (check 7th bit for sign)
            Registers.SetFlagConditionally(FlagType.Z, sum == 0);                               // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.H, ((regA & 0xF) + (operand & 0xF)) > 0xF); // (H) (Set if borrow occurs from bit 4)

            Registers.SetFlagConditionally(FlagType.PV,
                (((regA ^ operand) & 0x80) == 0) // Check if regA and operand have the same sign (both positive or both negative)
                &&
                (((regA ^ sum) & 0x80) != 0));   // Check if sum has a different sign from regA

            Registers.SetFlagConditionally(FlagType.C, sum > 0xFF); // (C) (Set if borrow occurs from bit 8)
            Registers.ClearFlag(FlagType.N);
            // N is unconditionally reset as per page 160 of https://www.zilog.com/docs/z80/um0080.pdf
        }
        /// <summary>
        /// Adds two 16-bit addends together and sets flags accordingly.
        /// </summary>
        /// <param name="augend">The first addend.</param>
        /// <param name="addend">The second addend.</param>
        /// <returns>The sum of the addends.</returns>
        private ushort ADDWord(ushort augend, ushort addend)
        {
            int sum = (ushort)(augend + addend);

            Registers.SetFlagConditionally(FlagType.H, 
                ((augend & 0x0FFF) + (addend & 0x0FFF))  // Add lower 12 bits of each addend together.
                > 0x0FFF);                               // Check if add overflowed into top 4 bits.

            Registers.SetFlagConditionally(FlagType.C, sum > 0xFFFF); // (C) (Set if sum exceeds ushort.MaxValue)

            Registers.ClearFlag(FlagType.N);
            // N is unconditionally reset as per page 202 of https://www.zilog.com/docs/z80/um0080.pdf

            return (ushort)sum;
        }
        /// <summary>
        /// Adds <paramref name="operand"/> and <see cref="FlagType.C"/> to A and sets flags accordingly.
        /// </summary>
        /// <param name="operand">The value to add to A.</param>
        private void ADCAny(byte operand)
        {
            byte regA = Registers.RegisterSet[A];
            byte carry = (byte)(Registers.RegisterSet[F] & 0b00000001);
            int sum = regA + operand + carry;
            Registers.RegisterSet[A] = (byte)sum;

            Registers.SetFlagConditionally(FlagType.S, (sum & 0x80) != 0);                              // (S) (check 7th bit for sign)
            Registers.SetFlagConditionally(FlagType.Z, (byte)sum == 0);                                 // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.H, ((regA & 0xF) + (operand & 0xF) + carry) > 0xF); // (H) (Set if borrow occurs from bit 4)

            Registers.SetFlagConditionally(FlagType.PV,
                (((regA ^ operand) & 0x80) == 0) // Check if regA and operand have the same sign (both positive or both negative)
                && 
                (((regA ^ sum) & 0x80) != 0));   // Check if sum has a different sign from regA  

            Registers.SetFlagConditionally(FlagType.C, sum > 0xFF); // (C) (Carry if sum exceeds 8 bits)
            Registers.ClearFlag(FlagType.N);
            // N is unconditionally reset as per page 166 of https://www.zilog.com/docs/z80/um0080.pdf
        }

        /// <summary>
        /// Subtracts <paramref name="operand"/> from A and sets flags accordingly.
        /// </summary>
        /// <param name="operand">The value to subtract from A.</param>
        private void SUBAny(byte operand)
        {
            byte regA = Registers.RegisterSet[A];
            int diff = regA - operand;
            Registers.RegisterSet[A] = (byte)diff;

            Registers.SetFlagConditionally(FlagType.S, (diff & 0x80) != 0);                   // (S) (check 7th bit for sign)
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);                            // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.H, ((regA & 0xF) - (operand & 0xF)) < 0); // (H) (Set if borrow occurs from bit 4)

            Registers.SetFlagConditionally(FlagType.PV,
                (((regA ^ operand) & 0x80) != 0) // Check if regA and operand have the same sign (both positive or both negative)
                && 
                (((regA ^ diff) & 0x80) != 0));  // Check if diff. has a different sign from regA 

            Registers.SetFlagConditionally(FlagType.C, regA < operand); // (C) (Set if no borrow from bit 8)
            Registers.ClearFlag(FlagType.N);
            // N is unconditionally reset as per page 160 of https://www.zilog.com/docs/z80/um0080.pdf
        }
        /// <summary>
        /// Subtracts <paramref name="operand"/> and Carry flag from A and sets flags accordingly.
        /// </summary>
        /// <param name="operand">The value to subtract from A.</param>
        private void SBCAny(byte operand)
        {
            byte regA = Registers.RegisterSet[A];
            byte carry = (byte)(Registers.RegisterSet[F] & 0b00000001);
            int diff = regA - operand - carry;
            Registers.RegisterSet[A] = (byte)diff;

            Registers.SetFlagConditionally(FlagType.S, (diff & 0x80) != 0);                           // (S) (check 7th bit for sign)
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);                                    // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.H, ((regA & 0xF) - (operand & 0xF) - carry) < 0); // (H) (Set if borrow occurs from bit 4)

            Registers.SetFlagConditionally(FlagType.PV,
                (((regA ^ operand) & 0x80) != 0) // Check if regA and operand have the same sign (both positive or both negative)
                && 
                (((regA ^ diff) & 0x80) != 0));  // Check if diff. has a different sign from regA

            Registers.SetFlagConditionally(FlagType.C, regA < (operand + carry)); // (C) (Set if no borrow from bit 8)
            Registers.ClearFlag(FlagType.N);
            // N is unconditionally reset as per page 160 of https://www.zilog.com/docs/z80/um0080.pdf
        }
    }
}