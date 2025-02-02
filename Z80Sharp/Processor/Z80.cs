using System.Timers;
using Z80Sharp.Data;
using Z80Sharp.Enums;
using Z80Sharp.Registers;
using Z80Sharp.Interfaces;
using System.Runtime.CompilerServices;

namespace Z80Sharp.Processor
{
    public partial class Z80
    {
        private readonly MainMemory _memory;
        private readonly Clock _clock;
        private readonly IZ80Logger _logger;
        private readonly DataBus _dataBus;

        private ulong _instrsExecuted;
        private ulong _instrsExecutedLastSecond;
        private System.Timers.Timer _cycleTimer = new(1000);
        private byte _currentInstruction;

        public ProcessorRegisters Registers = new();

        private bool _halted;
        public bool Halted
        {
            get => _halted;
            set
            {
                _halted = value;
                _logger.Log(value ? LogSeverity.Info : LogSeverity.Info, value ? "Processor halted" : "Processor unhalted");
            }
        }

        public ulong CyclesExecuted => _clock.TotalTStates;

        public Z80(MainMemory memory, DataBus dataBus, IZ80Logger logger, double clockSpeed)
        {
            _memory = memory ?? throw new ArgumentNullException(nameof(memory));
            _dataBus = dataBus ?? throw new ArgumentNullException(nameof(dataBus));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _clock = new Clock(clockSpeed);
        }



        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void UnhaltIfHalted()
        {
            if (_halted)
            {
                Registers.PC++;
                Halted = false;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Step() => ExecuteOnce();

        public void Run()
        {
            while (true) ExecuteOnce();
        }

        public void RunUntil(ulong tStates)
        {
            while (_clock.TotalTStates < tStates)
            {
                ExecuteOnce();
                if (_halted) return;
            }
        }

        public void RunUntilHalt()
        {
            // We don't call `ExecuteOnce` because it has a redundant check for _halted.
            while (!_halted)
            {
                HandleInterrupts();

                Registers.IncrementRefresh();
                _currentInstruction = Fetch();

                switch (_currentInstruction)
                {
                    case 0xDD: ExecuteIndexRInstruction(ref Registers.IX, ref Registers.IXhi, ref Registers.IXlo); break;
                    case 0xFD: ExecuteIndexRInstruction(ref Registers.IY, ref Registers.IYhi, ref Registers.IYlo); break;
                    case 0xED: ExecuteMiscInstruction(); break;
                    case 0xCB: ExecuteBitInstruction(); break;
                    default: ExecuteMainInstruction(); break;
                }

                _clock.Wait();
            }
        }


        private void ExecuteOnce()
        {
            HandleInterrupts();
            if (_halted) return;

            Registers.IncrementRefresh();
            _currentInstruction = Fetch();

            switch (_currentInstruction)
            {
                case 0xDD: ExecuteIndexRInstruction(ref Registers.IX, ref Registers.IXhi, ref Registers.IXlo); break;
                case 0xFD: ExecuteIndexRInstruction(ref Registers.IY, ref Registers.IYhi, ref Registers.IYlo); break;
                case 0xED: ExecuteMiscInstruction(); break;
                case 0xCB: ExecuteBitInstruction(); break;
                default: ExecuteMainInstruction(); break;
            }

            _clock.Wait();
        }

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

            Registers.MEMPTR = 0x0000;

            _clock.Reset();
        }

        public void Reset(ProcessorRegisters state)
        {
            Halted = false;

            Registers.AF = state.AF;
            Registers.BC = state.BC;
            Registers.DE = state.DE;
            Registers.HL = state.HL;
            Registers.AF_ = state.AF_;
            Registers.BC_ = state.BC_;
            Registers.DE_ = state.DE_;
            Registers.HL_ = state.HL_;
            Registers.IX = state.IX;
            Registers.IY = state.IY;
            Registers.PC = state.PC;
            Registers.SP = state.SP;

            Registers.IFF1 = state.IFF1;
            Registers.IFF2 = state.IFF2;
            Registers.InterruptMode = state.InterruptMode;
            Registers.I = state.I;
            Registers.R = state.R;

            Registers.MEMPTR = state.MEMPTR;

            _clock.Reset();
        }

        private void ReportCyclesPerSecond(object sender, ElapsedEventArgs e)
        {
            _instrsExecutedLastSecond = _instrsExecuted;
        }

        #region Logging

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LogInstructionDecode(string instruction)
            => _logger.Log(LogSeverity.Decode, instruction);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LogInstructionExec(string instruction)
            => _logger.Log(LogSeverity.Execution, instruction);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LogInterrupt(string interruptName)
            => _logger.Log(LogSeverity.Interrupt, interruptName);

        #endregion

        #region Fetch Operations

        /// <summary>
        /// Reads current byte at the <see cref="ProcessorRegisters.PC"/> then increments PC.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte Fetch() => _memory.Read(Registers.PC++);

        /// <summary>
        /// Reads the last byte fetched at <c>--PC</c>.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private byte FetchLast() => _memory.Read((ushort)(Registers.PC - 1));

        /// <summary>
        /// Fetches a 16-bit word from memory at the current PC address.
        /// </summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private ushort FetchImmediateWord() => (ushort)(Fetch() | (Fetch() << 8));

        #endregion
    }
}