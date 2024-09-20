﻿using System;
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
        /// <summary>
        /// Increments the input value and sets the flags register accordingly.
        /// </summary>
        /// <param name="increment">The value that will be incremented.</param>
        /// <returns>The incremented value.</returns>
        private byte INCWithFlags(byte increment)
        {
            // https://www.zilog.com/docs/z80/um0080.pdf Flags detailed on page 179.
            byte sum = (byte)(increment + 1);

            // Sign flag
            if ((sum & 0x80) != 0)
                Registers.SetFlag(StatusRegisterFlag.S);
            else
                Registers.ClearFlag(StatusRegisterFlag.S);

            // Zero flag
            if (sum == 0)
                Registers.SetFlag(StatusRegisterFlag.Z);
            else
                Registers.ClearFlag(StatusRegisterFlag.Z);

            // Half Carry flag (carry from bit 3 to bit 4)
            if ((increment & 0xF) == 0xF)
                Registers.SetFlag(StatusRegisterFlag.H);
            else
                Registers.ClearFlag(StatusRegisterFlag.H);

            // Overflow flag (P/V) (signed overflow)
            if (increment == 0x7F)
                Registers.SetFlag(StatusRegisterFlag.PV);
            else
                Registers.ClearFlag(StatusRegisterFlag.PV);

            Registers.ClearFlag(StatusRegisterFlag.N);
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

            // Set or clear the Sign flag (S) (check 7th bit for sign)
            if ((diff & 0x80) != 0)
                Registers.SetFlag(StatusRegisterFlag.S);
            else
                Registers.ClearFlag(StatusRegisterFlag.S);

            // Set or clear the Zero flag (Z)
            if (diff == 0)
                Registers.SetFlag(StatusRegisterFlag.Z);
            else
                Registers.ClearFlag(StatusRegisterFlag.Z);

            // Set or clear the Half Carry flag (H) (borrow from bit 4)
            if ((decrement & 0x0F) == 0)
                Registers.SetFlag(StatusRegisterFlag.H);
            else
                Registers.ClearFlag(StatusRegisterFlag.H);

            // Set or clear the Overflow flag (P/V) (signed overflow on decreasing from -128 to 127)
            if (decrement == 0x80)
                Registers.SetFlag(StatusRegisterFlag.PV);
            else
                Registers.ClearFlag(StatusRegisterFlag.PV);

            // Set the N flag (subtract flag)
            Registers.SetFlag(StatusRegisterFlag.N);
            return diff;
        }

    }
}