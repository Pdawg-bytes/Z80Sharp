﻿using System.Runtime.CompilerServices;
using Z80Sharp.Enums;
using Z80Sharp.Interfaces;
using Z80Sharp.Memory;
using Z80Sharp.Registers;
using static Z80Sharp.Registers.ProcessorRegisters;

namespace Z80Sharp.Processor
{
    public partial class Z80 /*: IProcessor*/
    {
        private readonly IMemory _memory;
        private readonly IZ80Logger _logger;
        private readonly IDataBus _dataBus;

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
        private void UnhaltIfHalted() { if(_halted) Halted = false; }

        public Z80(IMemory memory, IDataBus dataBus, IZ80Logger logger, bool isDebug)
        {
            _memory = memory;
            _dataBus = dataBus;
            _logger = logger;
            IsDebug = isDebug;
        }

        public void Run()
        {
            while (Registers.PC < _memory.Length)
            {
                HandleInterrupts();
                if (_halted) { break; }

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
            }
        }

        public void Stop()
        {

        }

        // Reference: http://www.z80.info/zip/z80-documented.pdf (page 9, section 2.4)
        public void Reset()
        {
            Halted = false;

            Registers.RegisterSet[A] = 0xFF;
            Registers.RegisterSet[F] = 0xFF;
            Registers.RegisterSet[A_] = 0xFF;
            Registers.RegisterSet[F_] = 0xFF;

            Registers.BC = Registers.BC_ = 0xFFFF;
            Registers.DE = Registers.DE_ = 0xFFFF;
            Registers.HL = Registers.HL_ = 0xFFFF;

            Registers.RegisterSet[I] = 0x00;

            Registers.PC = 0x0000;
            Registers.SP = 0xFFFF;

            Registers.InterruptMode = InterruptMode.IM0;

            Registers.IFF1 = false;
            Registers.IFF2 = false;

            // Writes "Hello, world!" on to port 0
            // string helloWorld = "21 12 00 3E 0E D3 00 7E FE 00 28 05 D3 00 23 18 F6 76 48 65 6C 6C 6F 2C 20 77 6F 72 6C 64 21";

            // I/O Test
            //string hexString = "31 00 FF 21 4B 00 CD 3C 00 21 00 EF 06 40 CD 14 00 C3 2F 00 DB FE FE 00 CA 14 00 FE 0D C8 77 23 05 CA 27 00 C3 14 00 21 00 EF 06 40 C3 14 00 3E 0A D3 00 21 00 EF CD 3C 00 C3 46 00 7E FE 00 C8 D3 00 23 C2 3C 00 3E 0A D3 00 76 45 6E 74 65 72 20 69 6E 70 75 74 3A 20";
            string hexString = "0xDD 0x21 0xFF 0xD2";
            string[] hexBytes = hexString.Split(' ');

            ushort address = 0x0000;

            for (int i = 0; i < hexBytes.Length; i++)
            {
                byte value = Convert.ToByte(hexBytes[i], 16);
                _memory.Write(address, value);

                address++;
            }

            _logger.Log(LogSeverity.Info, "Processor reset");
        }

        #region Logging
        /// <summary>
        /// Logs the decode operation of a given instruction.
        /// </summary>
        /// <param name="instruction">The data of the instruction.</param>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        private void LogInstructionDecode(string instruction)
        {
            _logger.Log(LogSeverity.Decode, instruction);
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
        private byte Fetch()
        {
            byte val = _memory.Read(Registers.PC);
            //_logger.Log(LogSeverity.Memory, $"READ at 0x{Registers.PC.ToString("X")} -> 0x{val.ToString("X")}");
            Registers.PC++;
            return val;
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