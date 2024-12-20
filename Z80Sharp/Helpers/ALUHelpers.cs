using System.Runtime.CompilerServices;
using Z80Sharp.Enums;
using Z80Sharp.Helpers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80
    {
        /// <summary>
        /// Increments the input value and sets the flags register accordingly.
        /// </summary>
        /// <param name="increment">The value that will be incremented.</param>
        /// <returns>The incremented value.</returns>
        private byte INCAny(byte increment)
        {
            // https://www.zilog.com/docs/z80/um0080.pdf Flags detailed on page 179.
            byte sum = (byte)(increment + 1);

            Registers.SetFlagConditionally(FlagType.S, (sum & 0x80) > 0);                         // (S) (check 7th bit for sign)
            Registers.SetFlagConditionally(FlagType.Z, sum == 0);                                 // (Z) (set if result is zero)
            Registers.SetFlagConditionally(FlagType.H, (increment & 0x0F) + (0x01 & 0x0F) > 0xF); // (H) (set if carry from bit 3 to bit 4)
            Registers.SetFlagConditionally(FlagType.PV, increment == 0x7F);                       // (P/V) (set on signed overflow)
            Registers.ClearFlag(FlagType.N); // Clear the N flag (as INC is not a subtraction)

            return sum;
        }
        /// <summary>
        /// Decrements the input value and sets the flags register accordingly.
        /// </summary>
        /// <param name="decrement">The value that will be decremented.</param>
        /// <returns>The decremented value.</returns>
        private byte DECAny(byte decrement)
        {
            // https://www.zilog.com/docs/z80/um0080.pdf Flags detailed on page 185.
            byte diff = (byte)(decrement - 1);

            Registers.SetFlagConditionally(FlagType.PV, decrement == 0x80);                 // (P/V) (set on signed overflow -128 -> 127)
            Registers.SetFlagConditionally(FlagType.S, (diff & 0x80) > 0);                  // (S) (check 7th bit for sign)
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);                          // (Z) (set if result is zero)
            Registers.SetFlagConditionally(FlagType.H, (decrement & 0x0F) < (0x01 & 0x0F)); // (H) (borrow from bit 4)
            Registers.SetFlag(FlagType.N); // Set the N flag (subtract flag)

            return diff;
        }

        /// <summary>
        /// Performs a bitwise OR on the Accumulator with <paramref name="operand"/>.
        /// </summary>
        /// <param name="operand">The operand which we OR A with.</param>
        private void ORAny(byte operand)
        {
            Registers.A |= operand;
            byte result = Registers.A;
            Registers.F = (byte)((byte)(FlagType.S | FlagType.Y | FlagType.X) & result); // (S, Y, X) (set based on respective bits of result)
            Registers.SetFlagConditionally(FlagType.Z, result == 0);                     // (Z) (set if result is zero)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result));            // (P/V) (set if parity)
            Registers.F &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
        }
        /// <summary>
        /// Performs a bitwise XOR on the Accumulator with <paramref name="operand"/>.
        /// </summary>
        /// <param name="operand">The operand which we XOR A with.</param>
        private void XORAny(byte operand)
        {
            Registers.A ^= operand;
            byte result = Registers.A;
            Registers.F = (byte)((byte)(FlagType.S | FlagType.Y | FlagType.X) & result); // (S, Y, X) (set based on respective bits of result)
            Registers.SetFlagConditionally(FlagType.Z, result == 0);                     // (Z) (set if result is zero)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result));            // (P/V) (set if parity)
            Registers.F &= (byte)~(FlagType.N | FlagType.H | FlagType.C);
        }

        /// <summary>
        /// Performs a bitwise AND on the Accumulator with <paramref name="operand"/>.
        /// </summary>
        /// <param name="operand">The operand which we AND A with.</param>
        private void ANDAny(byte operand)
        {
            Registers.A &= operand;
            byte result = Registers.A;
            Registers.F = (byte)((byte)(FlagType.S | FlagType.Y | FlagType.X) & result); // (S, Y, X) (set based on respective bits of result)
            Registers.SetFlagConditionally(FlagType.Z, result == 0);                     // (Z) (set if result is zero)
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result));            // (PV) (set if parity)
            Registers.F &= (byte)~(FlagType.N | FlagType.C);
            Registers.SetFlag(FlagType.H);
        }

        /// <summary>
        /// Performs a comparison on A with <paramref name="operand"/> and sets flags accordingly.
        /// </summary>
        /// <param name="operand">The value to subtract from A.</param>
        /// <remarks>A is not modified during this instruction.</remarks>
        private void CMPAny(byte operand)
        {
            byte regA = Registers.A;
            int diff = regA - operand;

            Registers.F = (byte)((byte)(FlagType.X | FlagType.Y) & operand);            // (X, Y) (Set based on respective bits of input)
            Registers.SetFlagConditionally(FlagType.S, (diff & 0x80) != 0);             // (S) (Set if sign is 1)
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);                      // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.H, (regA & 0xF) < (operand & 0xF)); // (H) (Set if borrow occurs from bit 4)

            Registers.SetFlagConditionally(FlagType.PV,
                ((regA ^ operand) & 0x80) != 0 &&      // regA and operand have different signs
                ((operand ^ (byte)diff) & 0x80) == 0); // operand and result have same signs

            Registers.SetFlagConditionally(FlagType.C, operand > regA);
            Registers.SetFlag(FlagType.N);
        }

        /// <summary>
        /// Adds <paramref name="operand"/> to A and sets flags accordingly.
        /// </summary>
        /// <param name="operand">The value to add to A.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ADDAny(byte operand)
        {
            byte regA = Registers.A;
            int sum = regA + operand;
            byte result = (byte)sum;
            Registers.A = result;

            Registers.F = (byte)((byte)(FlagType.S | FlagType.Y | FlagType.X) & result);        // (S, Y, X) (set based on respective bits of result)
            Registers.SetFlagConditionally(FlagType.Z, result == 0);                            // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.H, ((regA & 0xF) + (operand & 0xF)) > 0xF); // (H) (Set if borrow occurs from bit 4)

            Registers.SetFlagConditionally(FlagType.PV,
                (((regA ^ operand) & 0x80) == 0) && // Check if regA and operand have the same sign
                (((regA ^ sum) & 0x80) != 0));      // Check if sum has a different sign from regA

            Registers.SetFlagConditionally(FlagType.C, sum > 0xFF); // (C) (Set if borrow occurs from bit 8)
            Registers.ClearFlag(FlagType.N);
        }
        /// <summary>
        /// Adds two 16-bit addends together and sets flags accordingly.
        /// </summary>
        /// <param name="augend">The first addend.</param>
        /// <param name="addend">The second addend.</param>
        /// <returns>The sum of the addends.</returns>
        private ushort ADDWord(ushort augend, ushort addend)
        {
            int sum = augend + addend;
            ushort result = (ushort)sum;


            Registers.F &= (byte)(FlagType.S | FlagType.Z | FlagType.PV);

            Registers.SetFlagConditionally(FlagType.H,
                ((augend & 0x0FFF) + (addend & 0x0FFF))  // Add lower 12 bits of each addend together.
                > 0x0FFF);                               // Check if add overflowed into top 4 bits.

            Registers.SetFlagConditionally(FlagType.C, sum > 0xFFFF); // (C) (Set if sum exceeds 16 bits)
            Registers.ClearFlag(FlagType.N);

            Registers.F |= (byte)((result >> 8) & (byte)(FlagType.Y | FlagType.X));

            return result;
        }
        /// <summary>
        /// Adds <paramref name="operand"/> and <see cref="FlagType.C"/> to A and sets flags accordingly.
        /// </summary>
        /// <param name="operand">The value to add to A.</param>
        private void ADCAny(byte operand)
        {
            byte regA = Registers.A;
            byte carry = (byte)(Registers.F & 0b00000001);
            int sum = regA + operand + carry;
            Registers.A = (byte)sum;

            Registers.F = (byte)((byte)(FlagType.S | FlagType.Y | FlagType.X) & (byte)sum);             // (S, Y, X) (set based on respective bits of result)
            Registers.SetFlagConditionally(FlagType.Z, (byte)sum == 0);                                 // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.H, ((regA & 0xF) + (operand & 0xF) + carry) > 0xF); // (H) (Set if borrow occurs from bit 4)

            Registers.SetFlagConditionally(FlagType.PV,
                (((regA ^ operand) & 0x80) == 0) && // Check if regA and operand have the same sign
                (((regA ^ sum) & 0x80) != 0));      // Check if sum has a different sign from regA  

            Registers.SetFlagConditionally(FlagType.C, sum > 0xFF); // (C) (Carry if sum exceeds 8 bits)
            Registers.ClearFlag(FlagType.N);
        }
        /// <summary>
        /// Adds <paramref name="operand"/> and <see cref="FlagType.C"/> to HL and sets flags accordingly.
        /// </summary>
        /// <param name="operand">The value to add to HL.</param>
        private void ADCHL(ushort operand)
        {
            ushort regHL = Registers.HL;
            byte carry = (byte)(Registers.F & 0b00000001);
            int sum = regHL + operand + carry;
            ushort result = (ushort)sum;
            Registers.HL = result;

            Registers.F = (byte)((byte)(FlagType.S | FlagType.Y | FlagType.X) & sum >> 8);                      // (S, Y, X) (set based on respective bits of result)
            Registers.SetFlagConditionally(FlagType.Z, result == 0);                                            // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.H, (regHL & 0x0FFF) + (operand & 0x0FFF) + carry > 0x0FFF); // (H) (Set if borrow occurs from bit 11)

            Registers.SetFlagConditionally(FlagType.PV,
                (((regHL ^ operand) & 0x8000) == 0) && // Check if regHL and operand have the same sign
                (((regHL ^ sum) & 0x8000) != 0));      // Check if the sum has a different sign from regHL

            Registers.SetFlagConditionally(FlagType.C, sum > 0xFFFF); // (C) (Set if carry from bit 15)
            Registers.ClearFlag(FlagType.N);
        }


        /// <summary>
        /// Subtracts <paramref name="operand"/> from A and sets flags accordingly.
        /// </summary>
        /// <param name="operand">The value to subtract from A.</param>
        private void SUBAny(byte operand)
        {
            byte regA = Registers.A;
            int diff = regA - operand;
            Registers.A = (byte)diff;

            Registers.F = (byte)((byte)(FlagType.S | FlagType.Y | FlagType.X) & (byte)diff); // (S, Y, X) (set based on respective bits of result)
            Registers.SetFlagConditionally(FlagType.S, ((byte)diff & 0x80) != 0);            // (S) (check 7th bit for sign)
            Registers.SetFlagConditionally(FlagType.Z, (byte)diff == 0);                     // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.H, (regA & 0xF) < (operand & 0xF));      // (H) (Set if borrow occurs from bit 4)

            Registers.SetFlagConditionally(FlagType.PV,
                (((regA ^ operand) & 0x80) != 0) &&   // Check if regA and operand have different signs
                (((regA ^ (byte)diff) & 0x80) != 0)); // Check if diff. has a different sign from regA 

            Registers.SetFlagConditionally(FlagType.C, regA < operand); // (C) (Set if borrow from bit 8)
            Registers.SetFlag(FlagType.N);
        }
        /// <summary>
        /// Subtracts <paramref name="operand"/> and Carry flag from A and sets flags accordingly.
        /// </summary>
        /// <param name="operand">The value to subtract from A.</param>
        private void SBCAny(byte operand)
        {
            byte regA = Registers.A;
            byte carry = (byte)(Registers.F & 0b00000001);
            int diff = regA - operand - carry;
            Registers.A = (byte)diff;

            Registers.F = (byte)((byte)(FlagType.S | FlagType.Y | FlagType.X) & (byte)diff);      // (S, Y, X) (set based on respective bits of result)
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);                                // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.H, (regA & 0xF) < ((operand + carry) & 0xF)); // (H) (Set if borrow occurs from bit 4)

            Registers.SetFlagConditionally(FlagType.PV,
                (((regA ^ (operand + carry)) & 0x80) != 0) && // Check if regA and operand have different signs
                ((((operand + carry) ^ diff) & 0x80) == 0));  // Check if diff. has a different sign from regA

            Registers.SetFlagConditionally(FlagType.C, diff < 0); // (C) (Set if borrow from bit 8)
            Registers.SetFlag(FlagType.N);
        }
        /// <summary>
        /// Subtracts <paramref name="operand"/> and Carry flag from HL and sets flags accordingly.
        /// </summary>
        /// <param name="operand">The value to subtract from HL.</param>
        private void SBCHL(ushort operand)
        {
            ushort regHL = Registers.HL;
            byte carry = (byte)(Registers.F & 0b00000001);
            int diff = Registers.HL - operand - carry;
            Registers.HL = (ushort)diff;

            Registers.F = (byte)((byte)(FlagType.S | FlagType.Y | FlagType.X) & diff >> 8);            // (S, Y, X) (set based on respective bits of result)
            Registers.SetFlagConditionally(FlagType.S, ((ushort)diff & 0x8000) > 0);                   // (S) (check 15th bit for sign)
            Registers.SetFlagConditionally(FlagType.Z, (ushort)diff == 0);                             // (Z) (Set if result is 0)
            Registers.SetFlagConditionally(FlagType.H, (regHL & 0x0FFF) < (operand & 0x0FFF) + carry); // (H) (Set if borrow occurs from bit 11)

            Registers.SetFlagConditionally(FlagType.PV, 
                (((regHL ^ (operand + carry)) & 0x8000) != 0) &&
                ((((operand + carry) ^ (ushort)diff) & 0x8000) == 0));

            Registers.SetFlagConditionally(FlagType.C, diff < 0); // (C) (Set if borrow from bit 15)
            Registers.SetFlag(FlagType.N);
        }
    }
}