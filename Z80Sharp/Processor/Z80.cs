using System.Diagnostics;
using System.Net;
using System.Runtime.CompilerServices;
using System.Timers;
using Z80Sharp.Enums;
using Z80Sharp.Interfaces;
using Z80Sharp.Memory;
using Z80Sharp.Registers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public sealed partial class Z80 /*: IProcessor*/
    {
        //private readonly IMemory _memory;
        private readonly MainMemory _memory;
        private readonly IZ80Logger _logger;
        private readonly IDataBus _dataBus;

        public bool IsDebug { get; init; }

        private ulong InstrsExecuted;
        private ulong InstrsExecutedLastSecond;
        private System.Timers.Timer _cycleTimer;

        public byte _currentInstruction;

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
        private void UnhaltIfHalted() { if(_halted) Halted = false; }

        public Z80(MainMemory memory, IDataBus dataBus, IZ80Logger logger, bool isDebug)
        {
            _memory = memory;
            _dataBus = dataBus;
            _logger = logger;
            IsDebug = isDebug;
        }

        public void Step()
        {
            ExecuteOnce();
        }
        public void Run()
        {
            _cycleTimer = new System.Timers.Timer(1000);
            _cycleTimer.Elapsed += ReportCyclesPerSecond;
            _cycleTimer.AutoReset = true;
            _cycleTimer.Start();

            while (!_halted)
            {
                ExecuteOnce();
            }
        }
        private void ReportCyclesPerSecond(object sender, ElapsedEventArgs e)
        {
            //Console.WriteLine($"{InstrsExecuted - InstrsExecutedLastSecond} instr/s");
            InstrsExecutedLastSecond = InstrsExecuted;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void ExecuteOnce()
        {
            HandleInterrupts();
            if (_halted) { return; }

            _currentInstruction = Fetch();

            switch (_currentInstruction)
            {
                case 0xDD:
                    ExecuteIndexRInstruction(AddressingMode.IndexX); break;
                case 0xFD:
                    ExecuteIndexRInstruction(AddressingMode.IndexY); break;
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

            Registers.RegisterSet[A] = 0x00;
            Registers.RegisterSet[F] = 0x00;
            Registers.RegisterSet[A_] = 0x00;
            Registers.RegisterSet[F_] = 0x00;

            Registers.BC = Registers.BC_ = 0x0000;
            Registers.DE = Registers.DE_ = 0x0000;
            Registers.HL = Registers.HL_ = 0x0000;

            Registers.RegisterSet[I] = 0x00;
            Registers.RegisterSet[R] = 0x00;

            Registers.PC = 0x0000;
            Registers.SP = 0x0000;

            Registers.InterruptMode = InterruptMode.IM0;

            Registers.IFF1 = false;
            Registers.IFF2 = false;

            // Writes "Hello, world!" on to port 0
            // string helloWorld = "21 12 00 3E 0E D3 00 7E FE 00 28 05 D3 00 23 18 F6 76 48 65 6C 6C 6F 2C 20 77 6F 72 6C 64 21";

            // I/O Test
            //string hexString = "31 00 FF 21 4B 00 CD 3C 00 21 00 EF 06 40 CD 14 00 C3 2F 00 DB FE FE 00 CA 14 00 FE 0D C8 77 23 05 CA 27 00 C3 14 00 21 00 EF 06 40 C3 14 00 3E 0A D3 00 21 00 EF CD 3C 00 C3 46 00 7E FE 00 C8 D3 00 23 C2 3C 00 3E 0A D3 00 76 45 6E 74 65 72 20 69 6E 70 75 74 3A 20";
            //string hexString = "DD 21 00 00 3E C2 32 10 00 DD CB 10 26 76";
            /*string hexString = "16 05 1E 00 0E FE 46 23 ED 78 A6 20 01 37 CB 13 23 15 20 F2 7B A7 C9 CD 24 00 A7 20 FA CD 24 00 A7 28 DE C9 21 42 00 16 08 0E FE 46 23 ED 78 E6 1F 1E 05 CB 3F 30 09 23 1D 20 F8 15 20 ED A7 C9 7E C9 FE 23 5A 58 43 56 FD 41 53 44 46 47 FB 51 57 45 52 54 F7 31 32 33 34 35 EF 30 39 38 37 36 DF 50 4F 49 55 59 BF 23 4C 4B 4A 48 7F 20 23 4D 4E 42 FB 01 FD 01 DF 02 DF 01 7F 01";
            string[] hexBytes = hexString.Split(' ');

            ushort address = 0x0000;

            for (int i = 0; i < hexBytes.Length; i++)
            {
                byte value = Convert.ToByte(hexBytes[i], 16);
                _memory.Write(address, value);

                address++;
            }*/

            //byte[] rom = File.ReadAllBytes(@"..\..\ROM\48.rom");
            /*byte[] rom = File.ReadAllBytes(@"48.rom");

            ushort address = 0x0000;

            for (int i = 0; i < rom.Length; i++)
            {
                _memory.Write(address, rom[i]);

                address++;
            }*/

            _logger.Log(LogSeverity.Info, "Processor reset");
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
            Registers.RegisterSet[I] = state.RegisterSet[I];
            Registers.RegisterSet[R] = state.RegisterSet[R];
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
            //_logger.Log(LogSeverity.Interrupt, interruptName);
        }
        #endregion


        #region Fetch Operations
        /// <summary>
        /// Reads current byte at the <see cref="ProcessorRegisters.PC"/> then increments PC.
        /// </summary>
        /// <returns>The value at the address.</returns>
        private byte Fetch()
        {
            return _memory.Read(Registers.PC++);
        }

        /// <summary>
        /// Reads the last byte fetched at --<see cref="ProcessorRegisters.PC"/>.
        /// </summary>
        /// <returns>The byte before the current PC.</returns>
        private byte FetchLast()
        {
            byte val = _memory.Read((ushort)(Registers.PC - 1));
            //_logger.Log(LogSeverity.Memory, $"LREAD at 0x{((ushort)(Registers.PC - 1)).ToString("X")} -> 0x{val:X2}");
            return val;
        }

        /// <summary>
        /// Fetches the value at the <see cref="IRegisterSet.PC", and the value at the next address ahead to create a word./>
        /// </summary>
        /// <returns>The word (<see cref="ushort")./></returns>
        private ushort FetchImmediateWord() => (ushort)(Fetch() | (Fetch() << 8));
        #endregion
    }
}