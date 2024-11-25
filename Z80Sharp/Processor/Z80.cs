using System.Timers;
using Z80Sharp.Enums;
using Z80Sharp.Registers;
using Z80Sharp.Interfaces;
using System.Runtime.CompilerServices;
using static Z80Sharp.Registers.ProcessorRegisters;
using Z80Sharp.Memory;
using System.Diagnostics;

namespace Z80Sharp.Processor
{
    public unsafe partial class Z80 /*: IProcessor*/
    {
        private static MainMemory _memory;
        private readonly IZ80Logger _logger;
        private static IDataBus _dataBus;

        public bool IsDebug { get; init; }

        private static ulong InstrsExecuted;
        private static ulong InstrsExecutedLastSecond;
        private static System.Timers.Timer _cycleTimer = new System.Timers.Timer(1000);

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
                    //_logger.Log(LogSeverity.Info, "Processor halted");
                }
                else
                {
                    //_logger.Log(LogSeverity.Info, "Processor unhalted");
                }
            }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnhaltIfHalted() { if(_halted) Halted = false; }

        public Z80(MainMemory memory, IDataBus dataBus, IZ80Logger logger, bool isDebug)
        {
            _memory = memory;
            _dataBus = dataBus;
            _logger = logger;
            IsDebug = isDebug;

            if (_memory == null || _dataBus == null || _logger == null) throw new ArgumentNullException();
        }

        public void Step()
        {
            ExecuteOnce();
        }
        public void Run()
        {
            _cycleTimer.Elapsed += ReportCyclesPerSecond;
            _cycleTimer.AutoReset = true;
            _cycleTimer.Start();
            while (true)
            {
                ExecuteOnce();
            }
        }
        private void ReportCyclesPerSecond(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine($"{InstrsExecuted - InstrsExecutedLastSecond:n0} instr/s");
            InstrsExecutedLastSecond = InstrsExecuted;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExecuteOnce()
        {
            HandleInterrupts();
            if (_halted) 
            { 
                return; 
            }

            _currentInstruction = Fetch();

            switch (_currentInstruction)
            {
                case 0xDD:
                    ExecuteIndexRInstruction(ref Registers.IX, ref Registers.IXhi, ref Registers.IXlo); break;
                case 0xFD:
                    ExecuteIndexRInstruction(ref Registers.IY, ref Registers.IYhi, ref Registers.IYlo); break;
                case 0xED:
                    ExecuteMiscInstruction(); break;
                case 0xCB:
                    ExecuteBitInstruction(); break;

                default:
                    ExecuteMainInstruction(); break;
            }
            InstrsExecuted++;
        }

        // Reference: http://www.z80.info/zip/z80-documented.pdf (page 9, section 2.4)
        public void Reset()
        {
            Halted = false;

            Registers.A = 0x00;
            Registers.F = 0x00;
            Registers.AF_ = 0x0000;

            Registers.BC = Registers.BC_ = 0x0000;
            Registers.DE = Registers.DE_ = 0x0000;
            Registers.HL = Registers.HL_ = 0x0000;

            Registers.I = 0x00;
            Registers.R = 0x00;

            Registers.PC = 0x0000;
            Registers.SP = 0x0000;

            Registers.InterruptMode = InterruptMode.IM0;

            Registers.IFF1 = false;
            Registers.IFF2 = false;


            //_logger.Log(LogSeverity.Info, "Processor reset");
        }
        public void Reset(ProcessorRegisters state)
        {
            // General registers
            Registers.AF = state.AF;
            Registers.BC = state.BC;
            Registers.DE = state.DE;
            Registers.HL = state.HL;

            // Alternate general registers
            Registers.AF_ = state.AF_;
            Registers.BC_ = state.BC_;
            Registers.DE_ = state.DE_;
            Registers.HL_ = state.HL_;

            // Index registers
            Registers.IX = state.IX;
            Registers.IY = state.IY;

            // Utility registers
            Registers.PC = state.PC;
            Registers.SP = state.SP;

            // Special flags
            Registers.IFF1 = state.IFF1;
            Registers.IFF2 = state.IFF2;
            Registers.InterruptMode = state.InterruptMode;
            Registers.I = state.I;
            Registers.R = state.R;
        }


        #region Logging
        /// <summary>
        /// Logs the decode operation of a given instruction.
        /// </summary>
        /// <param name="instruction">The data of the instruction.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LogInstructionDecode(string instruction)
        {
            //_logger.Log(LogSeverity.Decode, instruction);
        }

        /// <summary>
        /// Logs the execution operation of a given instruction.
        /// </summary>
        /// <param name="instruction">The data of the instruction.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LogInstructionExec(string instruction)
        {
            _logger.Log(LogSeverity.Execution, instruction);
        }

        /// <summary>
        /// Logs an interrupt.
        /// </summary>
        /// <param name="interruptName">The type of interrupt that was triggered.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LogInterrupt(string interruptName)
        {
            _logger.Log(LogSeverity.Interrupt, interruptName);
        }
        #endregion


        #region Fetch Operations
        /// <summary>
        /// Reads current byte at the <see cref="ProcessorRegisters.PC"/> then increments PC.
        /// </summary>
        /// <returns>The value at the address.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte Fetch() => _memory.Read(Registers.PC++);

        /// <summary>
        /// Reads the last byte fetched at --<see cref="ProcessorRegisters.PC"/>.
        /// </summary>
        /// <returns>The byte before the current PC.</returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte FetchLast() => _memory.Read((ushort)(Registers.PC - 1));

        /// <summary>
        /// Fetches the value at the <see cref="IPC", and the value at the next address ahead to create a word./>
        /// </summary>
        /// <returns>The word (<see cref="ushort")./></returns>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort FetchImmediateWord() => (ushort)(Fetch() | (Fetch() << 8));
        #endregion
    }
}