using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata.Ecma335;
using System.Text;
using System.Threading.Tasks;
using Z80Sharp.Enums;
using Z80Sharp.Interfaces;
using Z80Sharp.Logging;
using Z80Sharp.Memory;
using Z80Sharp.Registers;

namespace Z80Sharp.Processor
{
    public abstract class Z80Base : IProcessor
    {
        protected MainMemory _memory;
        protected readonly IZ80Logger _logger;

        public virtual bool IsDebug() { return false; }

        public IRegisterSet Registers { get; set; } = new ProcessorRegisters();

        public bool Halted { get; set; }

        public Z80Base(ushort memSize, IZ80Logger logger)
        {
            _memory = new MainMemory(memSize);
            _logger = logger;
        }

        public virtual void Run()
        {
            while (Registers.PC < _memory.Length)
            {
                byte currentInstruction = Fetch();
                switch (currentInstruction)
                {
                    case 0xDD:
                        ParseIXInstruction();
                        break;
                    case 0xFD:
                        ParseIYInstruction();
                        break;
                    case 0xED:
                        ParseMiscInstruction();
                        break;
                    case 0xCB:
                        ParseBitInstruction();
                        break;
                    case 0x00:
                        if (IsDebug()) LogInstructionDecode("NOP (0x00)");
                        break;
                }
            }
        }

        public virtual void Stop()
        {

        }

        public virtual void Reset()
        {
            for (ushort i = 0; i < _memory.Length; i++)
            {
                _memory.Write(i, 0x00);
            }

            Registers.IFF1 = false;
            Registers.IFF2 = false;
            Registers.PC = 0;
            Registers.A = 0xFF;
            Registers.SP = 0xFFFF;
            Registers.F = 0xFF;

            _memory.Write(0, 0xDD);
            _memory.Write(1, 0xCB);
            _memory.Write(2, 0x03);
            _memory.Write(3, 0xCE);
            _memory.Write(4, 0x00);

            _logger.Log(Enums.LogSeverity.Info, "Processor reset");
        }

        /// <summary>
        /// Reads current byte at the <see cref="IRegisterSet.PC"/>.
        /// </summary>
        /// <returns>The value at the address.</returns>
        public abstract byte Fetch();

        protected void LogInstructionDecode(string instruction)
        {
            _logger.Log(LogSeverity.Decode, instruction);
        }

        #region Instructions
        /// <summary>
        /// Parses instructions with the 0xCB prefix.
        /// </summary>
        /// <remarks>Also used as loopback for xDD\xCB; xFD\xCB.</remarks>
        /// <param name="addressingMode">The addressing mode that the bit instruction is handling.</param>
        public virtual void ParseBitInstruction(AddressingMode addressingMode = AddressingMode.Regular)
        {
            if (IsDebug()) LogInstructionDecode("Bit (0xCB)");

            sbyte displacement = 0;
            if (addressingMode != AddressingMode.Regular) displacement = (sbyte)Fetch();

            byte instruction = Fetch();
        }

        /// <summary>
        /// Parses instructions with the 0xDD prefix, relating to the Index X register.
        /// </summary>
        public virtual void ParseIXInstruction()
        {
            if (IsDebug()) LogInstructionDecode("Index X (0xDD)");

            byte instruction = Fetch();
            switch (instruction)
            {
                case 0xCB:
                    ParseBitInstruction(AddressingMode.IndexX);
                    break;
            }
        }

        /// <summary>
        /// Parses instructions with the 0xFD prefix, relating to the Index Y Register.
        /// </summary>
        public virtual void ParseIYInstruction()
        {
            if (IsDebug()) LogInstructionDecode("Index Y (0xFD)");

            byte instruction = Fetch();
            switch (instruction)
            {

            }
        }

        /// <summary>
        /// Parses instructions with the 0xED prefix, all miscellaneous instructions.
        /// </summary>
        public virtual void ParseMiscInstruction()
        {
            if (IsDebug()) LogInstructionDecode("Misc (0xED)");

            byte instruction = Fetch();
            switch (instruction)
            {

            }
        }
        #endregion
    }
}