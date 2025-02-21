using Z80Sharp.Enums;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        /// <summary>
        /// Performs a comparison on A with <paramref name="operand"/> and sets flags accordingly.
        /// </summary>
        /// <param name="operand">The value to subtract from A.</param>
        /// <remarks>A is not modified during this instruction.</remarks>
        private void Compare8(byte operand)
        {
            byte regA = Registers.A;
            int diff = regA - operand;

            Registers.F = (byte)(0x28 & operand); // (Y | X) & operand
            Registers.SetFlagConditionally(FlagType.S, (diff & 0x80) != 0);
            Registers.SetFlagConditionally(FlagType.Z, diff == 0);
            Registers.SetFlagConditionally(FlagType.H, (regA & 0xF) < (operand & 0xF));

            Registers.SetFlagConditionally(FlagType.PV,
                ((regA ^ operand) & 0x80) != 0 &&
                ((operand ^ (byte)diff) & 0x80) == 0);

            Registers.SetFlagConditionally(FlagType.C, operand > regA);
            Registers.SetFlag(FlagType.N);
        }

        /// <summary>
        /// Performs a bitwise operation on A with <paramref name="operand"/>, given the <see cref="BitOperation"/>.
        /// </summary>
        /// <param name="operand">The operand to modify A with.</param>
        /// <param name="operation">The operation to perform on A and the <paramref name="operand"/>.</param>
        private void Bitwise8(byte operand, BitOperation operation)
        {
            byte result = Registers.A = operation switch
            {
                BitOperation.Xor => (byte)(Registers.A ^ operand),
                BitOperation.Or => (byte)(Registers.A | operand),
                BitOperation.And => (byte)(Registers.A & operand),
                _ => Registers.A
            };

            Registers.F = (byte)(0xA8 & result); // (S | Y | X) & result
            Registers.F &= (byte)~(FlagType.N | FlagType.C);
            Registers.SetFlagConditionally(FlagType.Z, result == 0);
            Registers.SetFlagConditionally(FlagType.H, operation == BitOperation.And);
            Registers.SetFlagConditionally(FlagType.PV, CheckParity(result));
        }

        /// <summary>
        /// Increments or decrements the <paramref name="operand"/>, sets flags accordingly, and returns the result.
        /// </summary>
        /// <param name="operand">The value to modify.</param>
        /// <param name="decrement"><c>true</c> if the value is being decremented; <c>false</c> if otherwise.</param>
        /// <returns></returns>
        private byte IncDec8(byte operand, bool decrement)
        {
            byte result = (byte)(operand + (decrement ? -1 : 1));

            Registers.F &= 0b00000001;            // Preserve carry
            Registers.F |= (byte)(0xA8 & result); // (S | Y | X) & result

            Registers.SetFlagConditionally(FlagType.Z, result == 0);
            Registers.SetFlagConditionally(FlagType.H, (operand & 0x0F) == (decrement ? 0x00 : 0x0F));
            Registers.SetFlagConditionally(FlagType.PV, operand == (decrement ? 0x80 : 0x7F));
            Registers.SetFlagConditionally(FlagType.N, decrement);

            return result;
        }

        /// <summary>
        /// Adds or subtracts <paramref name="operand"/> and the 
        /// <see cref="FlagType.C"/> flag from <see cref="Registers.ProcessorRegisters.A"/> 
        /// depending on <paramref name="subtract"/> and <paramref name="carry"/>.
        /// </summary>
        /// <param name="operand">The operand to modify A with.</param>
        /// <param name="subtract"><c>true</c> if a subtraction is taking place; <c>false</c> otherwise.</param>
        /// <param name="carry"><c>true</c> if the carry flag is included in the operation; <c>false</c> otherwise.</param>
        private void AddSub8WithCarry(byte operand, bool subtract, bool carry)
        {
            byte regA = Registers.A;
            int opResult = subtract
                ? (regA - operand - (carry ? (byte)(Registers.F & 0b00000001) : 0))
                : (regA + operand + (carry ? (byte)(Registers.F & 0b00000001) : 0));

            byte result = (byte)opResult;

            Registers.F = (byte)(0xA8 & result); // (S | Y | X) & result
            Registers.SetFlagConditionally(FlagType.Z, result == 0);
            Registers.SetFlagConditionally(FlagType.N, subtract);

            Registers.SetFlagConditionally(FlagType.H, (byte)((regA ^ operand ^ result) & 0x10) != 0);
            Registers.SetFlagConditionally(FlagType.C, subtract ? opResult < 0 : opResult > 0xFF);

            Registers.SetFlagConditionally(FlagType.PV,
                (((regA ^ operand) & 0x80) == 0) == !subtract &&
                ((regA ^ result) & 0x80) != 0);

            Registers.A = result;
        }


        /// <summary>
        /// Adds or subtracts <paramref name="operand"/> and the carry flag from <see cref="Registers.ProcessorRegisters.HL"/>.
        /// </summary>
        /// <param name="operand">The value to modify HL with.</param>
        /// <param name="subtract"><c>true</c> if a subtraction is taking place; <c>false</c> otherwise.</param>
        private void AddSub16CarryHL(ushort operand, bool subtract)
        {
            ushort regHL = Registers.HL;
            byte carry = (byte)(Registers.F & 0b00000001);
            int newHL = subtract ? (regHL - operand - carry) : (regHL + operand + carry);
            ushort result = (ushort)newHL;

            Registers.F = subtract ? (byte)FlagType.N : (byte)0;
            Registers.SetFlagConditionally(FlagType.S, (result & 0x8000) != 0);
            Registers.SetFlagConditionally(FlagType.Z, result == 0);

            Registers.SetFlagConditionally(FlagType.H, ((regHL ^ operand ^ result) & 0x1000) != 0);

            Registers.SetFlagConditionally(FlagType.PV,
                (((regHL ^ operand) & 0x8000) == 0) == !subtract &&
                ((regHL ^ result) & 0x8000) != 0);

            Registers.SetFlagConditionally(FlagType.C, subtract ? (newHL < 0) : (newHL > 0xFFFF));
            Registers.F |= (byte)((byte)(result >> 8) & (byte)(FlagType.Y | FlagType.X));

            Registers.MEMPTR = (ushort)(regHL + 1);

            Registers.HL = result;
        }

        /// <summary>
        /// Adds two 16-bit addends together and sets flags accordingly.
        /// </summary>
        /// <param name="left">The first right.</param>
        /// <param name="right">The second right.</param>
        /// <returns>The sum of the addends.</returns>
        private ushort Add16(ushort left, ushort right)
        {
            int sum = left + right;
            ushort result = (ushort)sum;

            Registers.F &= (byte)(FlagType.S | FlagType.Z | FlagType.PV);

            Registers.SetFlagConditionally(FlagType.H,
                ((left & 0x0FFF) + (right & 0x0FFF))
                > 0x0FFF);

            Registers.SetFlagConditionally(FlagType.C, sum > 0xFFFF);
            Registers.ClearFlag(FlagType.N);

            Registers.F |= (byte)((result >> 8) & (byte)(FlagType.Y | FlagType.X));

            Registers.MEMPTR = (ushort)(left + 1);

            return result;
        }
    }
}