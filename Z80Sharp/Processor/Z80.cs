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
                        ExecuteIndexXInstruction(); break;
                    case 0xFD:
                        ExecuteIndexYInstruction(); break;
                    case 0xED:
                        ExecuteMiscInstruction(); break;
                    case 0xCB:
                        ExecuteBitInstruction(); break;

                    default:
                        ExecuteMainInstruction(); break;
                }
            }
            for (ushort i = 0; i < _memory.Length; i++)
            {
                //Console.WriteLine("0x"+_memory.Read(i).ToString("X2"));
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
            Registers.RegisterSet[F] = 0b01010101;
            Registers.RegisterSet[A_] = 0xFF;
            Registers.RegisterSet[F_] = 0xFF;

            Registers.BC = Registers.BC_ = 0xFFFF;
            Registers.DE = Registers.DE_ = 0xFFFF;
            Registers.HL = Registers.HL_ = 0xFFFF;

            Registers.RegisterSet[I] = 0x00;

            Registers.PC = 0;
            Registers.SP = 0xFFFF;

            Registers.InterruptMode = InterruptMode.IM0;

            Registers.IFF1 = false;
            Registers.IFF2 = false;

            // temp
            /*_memory.Write(0x0000, 0x21);
            _memory.Write(0x0001, 0x34);
            _memory.Write(0x0002, 0x12);
            _memory.Write(0x0003, 0x31);
            _memory.Write(0x0004, 0xA);
            _memory.Write(0x0005, 0x00);
            _memory.Write(0x0006, 0xE3);
            _memory.Write(0x0007, 0x00);
            _memory.Write(0xA, 0xFF);
            _memory.Write(0xB, 0xEE);*/

            _memory.Write(0x0, 0x3E);
            _memory.Write(0x1, 0xF7);
            _memory.Write(0x2, 0x27);

            /*_memory.Write(0, 0x3E);
            _memory.Write(1, 0xFF);
            _memory.Write(2, 0x47);
            _memory.Write(3, 0x00);
            _memory.Write(4, 0x00);*/

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
        #endregion


        #region Fetch Operations
        /// <summary>
        /// Reads current byte at the <see cref="ProcessorRegisters.PC"/> then increments PC.
        /// </summary>
        /// <returns>The value at the address.</returns>
        private byte Fetch()
        {
            byte val = _memory.Read(Registers.PC);
            _logger.Log(LogSeverity.Memory, $"READ at 0x{Registers.PC.ToString("X")} -> 0x{val.ToString("X")}");
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
            _logger.Log(LogSeverity.Memory, $"LREAD at 0x{((ushort)(Registers.PC - 1)).ToString("X")} -> 0x{val:X2}");
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