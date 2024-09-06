using Z80Sharp.Enums;
using Z80Sharp.Interfaces;
using Z80Sharp.Memory;
using Z80Sharp.Registers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80 /*: IProcessor*/
    {
        private MainMemory _memory;
        private readonly IZ80Logger _logger;

        public bool IsDebug { get; init; }

        private byte _currentInstruction;

        public ProcessorRegisters Registers = new ProcessorRegisters();

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
            InitializeInstructionHandlers();

            _memory = new MainMemory(memSize);
            _logger = logger;
            IsDebug = isDebug;
        }

        public void Run()
        {
            while (Registers.PC < _memory.Length)
            {
                _currentInstruction = Fetch();

                switch (_currentInstruction)
                {
                    case 0xDD:
                        DDInstructionTable[_currentInstruction](); break;
                    case 0xFD:
                        FDInstructionTable[_currentInstruction](); break;
                    case 0xED:
                        EDInstructionTable[_currentInstruction](); break;
                    case 0xCB:
                        CBInstructionTable[_currentInstruction](); break;

                    default:
                        instructionTable[_currentInstruction](); break;
                }
            }
            for (ushort i = 0; i < _memory.Length; i++)
            {
                Console.WriteLine("0x"+_memory.Read(i).ToString("X2"));
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

            // temp
            _memory.Write(0, 0x21);
            _memory.Write(1, 0x03);
            _memory.Write(2, 0x00);
            _memory.Write(3, 0x77);
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


        #region Fetch Operations
        /// <summary>
        /// Reads current byte at the <see cref="IRegisterSet.PC"/> then increments PC.
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