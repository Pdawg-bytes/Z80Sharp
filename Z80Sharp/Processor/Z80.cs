using Z80Sharp.Enums;
using Z80Sharp.Interfaces;
using Z80Sharp.Memory;
using Z80Sharp.Registers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public class Z80 : IProcessor
    {
        protected MainMemory _memory;
        protected readonly IZ80Logger _logger;

        public bool IsDebug { get; init; }

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

        public Z80(ushort memSize, IZ80Logger logger, bool isDebug)
        {
            _memory = new MainMemory(memSize);
            _logger = logger;
            IsDebug = isDebug;
        }

        public void Run()
        {
            while (Registers.PC < _memory.Length)
            {
                byte currentInstruction = Fetch();
                byte operatingRegister = (byte)((currentInstruction >> 3) & 0x07);
                switch (currentInstruction)
                {
                    // NOP
                    case 0x00:
                        break;

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

                            break;
                        }
                    // LD SP, nnnn
                    case 0x31:
                        {
                            ushort word = FetchImmediateWord();
                            Registers.SP = word;
                            break;
                        }

                    // LD r, C
                    case 0x41:
                    case 0x51:
                    case 0x61:
                        break;
                }
            }
        }

        public void Stop()
        {

        }

        public void Reset()
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

        #region Instructions
        /// <summary>
        /// Parses instructions with the 0xCB prefix.
        /// </summary>
        /// <remarks>Also used as loopback for xDD\xCB; xFD\xCB.</remarks>
        /// <param name="addressingMode">The addressing mode that the bit instruction is handling.</param>
        public void ParseBitInstruction(AddressingMode addressingMode = AddressingMode.Regular)
        {
            sbyte displacement = 0;
            if (addressingMode != AddressingMode.Regular) displacement = (sbyte)Fetch();

            byte instruction = Fetch();
        }

        /// <summary>
        /// Parses instructions with the 0xDD prefix, relating to the Index X register.
        /// </summary>
        public void ParseIXInstruction()
        {
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
        public void ParseIYInstruction()
        {
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
        public void ParseMiscInstruction()
        {
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
        private byte Fetch()
        {
            byte val = _memory.Read(Registers.PC);
            _logger.Log(Enums.LogSeverity.Memory, $"READ at 0x{Registers.PC.ToString("X")} -> 0x{val.ToString("X")}");
            Registers.PC++;
            return val;
        }

        /// <summary>
        /// Fetches the value at the <see cref="IRegisterSet.PC", and the value at the next address ahead to create a word./>
        /// </summary>
        /// <returns>The word (<see cref="ushort")./></returns>
        private ushort FetchImmediateWord()
        {
            return (ushort)(Fetch() | (Fetch() << 8));
        }
        #endregion
    }
}