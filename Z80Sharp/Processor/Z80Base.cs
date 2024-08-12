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
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public abstract partial class Z80Base : IProcessor
    {
        protected MainMemory _memory;
        protected readonly IZ80Logger _logger;

        public virtual bool IsDebug() { return false; }

        public IRegisterSet Registers { get; set; } = new ProcessorRegisters();

        private bool _halted;
        public bool Halted 
        {
            get => _halted;
            set
            {
                _halted = value;
                if(value)
                {
                    _logger.Log(LogSeverity.Info, "Processor halted");
                }
                else
                {
                    _logger.Log(LogSeverity.Info, "Processor unhalted");
                }
            }
        }

        public Z80Base(ushort memSize, IZ80Logger logger)
        {
            _memory = new MainMemory(memSize);
            _logger = logger;
        }

        public void Run()
        {
            bool isDebug = IsDebug();
            while (Registers.PC < _memory.Length)
            {
                byte currentInstruction = Fetch();
                byte operatingRegister = (byte)((currentInstruction >> 3) & 0x07);
                switch (currentInstruction)
                {
                    // NOP
                    case 0x00:
                        if (isDebug) LogInstructionDecode("0x00: NOP"); break;

                    // Special instructions
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

                    // LD dd, nnnn
                    case 0x01:
                    case 0x11:
                    case 0x21:
                        {
                            Registers.RegisterSet[operatingRegister + 1] = Fetch();
                            Registers.RegisterSet[operatingRegister] = Fetch();

                            if (isDebug) LogInstructionExec($"0x{currentInstruction:X2}: LD DD, 0x{Registers.RegisterSet[operatingRegister]:X2}{Registers.RegisterSet[operatingRegister + 1]:X2}");
                            break;
                        }
                    // LD SP, nnnn
                    case 0x31:
                        {
                            ushort word = FetchImmediateWord();
                            Registers.SP = word;
                            if (isDebug) LogInstructionExec($"0x31: LD SP, 0x{word:X4}");
                            break;
                        }

                    // LD r, C
                    case 0x41:
                    case 0x51:
                    case 0x61:
                        break;
                }
            }
            Console.WriteLine(Registers.BC);
        }

        public virtual void Stop()
        {

        }

        public virtual void Reset()
        {
            Halted = false;

            // temp
            for (ushort i = 0; i < _memory.Length; i++)
            {
                _memory.Write(i, 0x00);
            }

            Registers.RegisterSet[A] = 0xFF;
            Registers.RegisterSet[F] = 0xFF;
            Registers.RegisterSet[A_] = 0xFF;
            Registers.RegisterSet[F_] = 0xFF;

            Registers.RegisterSet[I] = 0x00;

            Registers.PC = 0;
            Registers.SP = 0xFFFF;

            Registers.InterruptMode = InterruptMode.IM0;

            Registers.IFF1 = false;
            Registers.IFF2 = false;


            _memory.Write(0, 0x01);
            _memory.Write(1, 0xFF);
            _memory.Write(2, 0xEF);
            _memory.Write(3, 0xCE);
            _memory.Write(4, 0x00);

            _logger.Log(LogSeverity.Info, "Processor reset");
        }


        #region Logging
        /// <summary>
        /// Logs a the decode operation of a given instruction.
        /// </summary>
        /// <param name="instruction">The data of the instruction.</param>
        protected void LogInstructionDecode(string instruction)
        {
            _logger.Log(LogSeverity.Decode, instruction);
        }

        /// <summary>
        /// Logs a the execution operation of a given instruction.
        /// </summary>
        /// <param name="instruction">The data of the instruction.</param>
        protected void LogInstructionExec(string instruction)
        {
            _logger.Log(LogSeverity.Execution, instruction);
        }
        #endregion


        #region Instructions
        /// <summary>
        /// Parses instructions with the 0xCB prefix.
        /// </summary>
        /// <remarks>Also used as loopback for xDD\xCB; xFD\xCB.</remarks>
        /// <param name="addressingMode">The addressing mode that the bit instruction is handling.</param>
        public virtual void ParseBitInstruction(AddressingMode addressingMode = AddressingMode.Regular)
        {
            if (IsDebug()) LogInstructionDecode("0xCB: Bit");

            sbyte displacement = 0;
            if (addressingMode != AddressingMode.Regular) displacement = (sbyte)Fetch();

            byte instruction = Fetch();
        }

        /// <summary>
        /// Parses instructions with the 0xDD prefix, relating to the Index X register.
        /// </summary>
        public virtual void ParseIXInstruction()
        {
            if (IsDebug()) LogInstructionDecode("0xDD: Index X");

            byte currentInstruction = Fetch();
            switch (currentInstruction)
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
            if (IsDebug()) LogInstructionDecode("0xFD: Index Y");

            byte currentInstruction = Fetch();
            switch (currentInstruction)
            {
                case 0xCB:
                    ParseBitInstruction(AddressingMode.IndexY);
                    break;
            }
        }

        /// <summary>
        /// Parses instructions with the 0xED prefix, all miscellaneous instructions.
        /// </summary>
        public virtual void ParseMiscInstruction()
        {
            if (IsDebug()) LogInstructionDecode("0xED: Misc");

            byte currentInstruction = Fetch();
            switch (currentInstruction)
            {

            }
        }
        #endregion


        #region Fetch Operations
        /// <summary>
        /// Reads current byte at the <see cref="IRegisterSet.PC"/>.
        /// </summary>
        /// <returns>The value at the address.</returns>
        public abstract byte Fetch();

        /// <summary>
        /// Fetches the value at the <see cref="IRegisterSet.PC", and the value at the next address ahead to create a word./>
        /// </summary>
        /// <returns>The word (<see cref="ushort")./></returns>
        protected ushort FetchImmediateWord()
        {
            return (ushort)(Fetch() | (Fetch() << 8));
        }
        #endregion
    }
}