using Z80Sharp.Logging;
using Z80Sharp.Processor;
using Z80Sharp.Enums;
using Z80Sharp.Constants;
using Z80Sharp.Interfaces;
using Z80Sharp.Registers;
using static Z80Sharp.Registers.ProcessorRegisters;
using System.Diagnostics;
using Z80Sharp.Memory;
using System.Text;
using System;

namespace Z80Sharp.TestProgram
{
    public static class Program
    {
        private static bool quitRequested = false;
        private static IZ80Logger logger = new Logger(useColors: false);
        private static Z80 z80;
        private static MainMemory mainMemory;
        private static StreamWriter streamWriter;

        public static void Main(string[] args)
        {
            streamWriter = new StreamWriter("RunLogZS.txt", false);
            logger.LogGenerated += Logger_LogGenerated;

            IDataBus dataBus = new DataBus();

            mainMemory = new MainMemory(65536);

            int i = 0;
            z80 = new Z80(mainMemory, dataBus, logger, true);
            z80.Reset();
            Thread processorThread = new(() =>
            {
                while(true)
                {
                    if (z80.Halted) break;
                    z80.Step();
                    /*if (z80.Registers.HL == 0xFFFE)
                    {
                        i++;
                        if (i > 4)
                        {
                            z80.Halted = true;
                        }
                    }*/
                    if (z80._currentInstruction == 0x4E && z80.Registers.PC == 0x1621)
                    {
                        //_cpu.IsHalted = true; break;
                    }
                }

            });
            processorThread.Start();

            //ReadConsoleInput();
        }

        private static void Logger_LogGenerated(object? sender, Events.LogGeneratedEventArgs e)
        {
            streamWriter.WriteLine($"0x{z80.Registers.PC:X4}: " + e.LogData);
        }

        private static ConsoleKeyInfo _key;
        private static void ReadConsoleInput()
        {
            Console.WriteLine($"{Colors.ORANGE}Welcome to the Z80Sharp interactive console! Press 'q' to quit...{Colors.ANSI_RESET}");
            while (!quitRequested)
            {
                _key = Console.ReadKey();
                switch (_key.KeyChar)
                {
                    case 'q':
                        quitRequested = true;
                        break;
                    case 'h':
                        Console.Write($"\n{Colors.CYAN}{Colors.ANSI_BOLD}==========Help=========={Colors.ANSI_RESET}\n{Colors.WHITE}{Colors.ANSI_ITALIC}Commands:{Colors.ANSI_RESET}\n{Colors.LIGHT_YELLOW}> h{Colors.ANSI_RESET} - prints this help text.\n{Colors.LIGHT_YELLOW}> d{Colors.ANSI_RESET} - dumps the processor state.\n{Colors.LIGHT_YELLOW}> q{Colors.ANSI_RESET} - quits the emulator and console.\n{Colors.LIGHT_YELLOW}> m{Colors.ANSI_RESET} - dumps the main memory.");
                        break;
                    case 'd':
                        Console.Write($"\n{Colors.ANSI_BOLD}{Colors.GREEN}=====Processor state====={Colors.ANSI_RESET}");
                        PrintProcessorState(z80.Registers);
                        break;
                    case 'm':
                        DumpMemory();
                        break;
                    default:
                        Console.Write($"\n{Colors.RED}Invalid command. Press 'h' for help.{Colors.ANSI_RESET}");
                        break;
                }
                Console.WriteLine();
            }
        }

        public static void PrintProcessorState(ProcessorRegisters registers)
        {
            Console.WriteLine($"\n{Colors.LIGHT_BLUE}General-purpose registers{Colors.ANSI_RESET}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}B:{Colors.ANSI_RESET} 0x{registers.RegisterSet[B].ToString("X").PadLeft(2, '0')}    {Colors.LIGHT_YELLOW}C:{Colors.ANSI_RESET} 0x{registers.RegisterSet[C].ToString("X").PadLeft(2, '0')}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}D:{Colors.ANSI_RESET} 0x{registers.RegisterSet[D].ToString("X").PadLeft(2, '0')}    {Colors.LIGHT_YELLOW}E:{Colors.ANSI_RESET} 0x{registers.RegisterSet[E].ToString("X").PadLeft(2, '0')}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}H:{Colors.ANSI_RESET} 0x{registers.RegisterSet[H].ToString("X").PadLeft(2, '0')}    {Colors.LIGHT_YELLOW}L:{Colors.ANSI_RESET} 0x{registers.RegisterSet[L].ToString("X").PadLeft(2, '0')}");

            Console.WriteLine($"\n{Colors.LIGHT_BLUE}Data registers{Colors.ANSI_RESET}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}A:{Colors.ANSI_RESET} 0x{registers.RegisterSet[A].ToString("X").PadLeft(2, '0')}\n");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}F:{Colors.LIGHT_ORANGE} 0fSZYHXPNC{Colors.ANSI_RESET}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}F:{Colors.ANSI_RESET} 0b{Convert.ToString(registers.RegisterSet[F], 2).PadLeft(8, '0')}");

            Console.WriteLine($"\n{Colors.LIGHT_BLUE}Index registers{Colors.ANSI_RESET}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}IX:{Colors.ANSI_RESET} 0x{registers.IX.ToString("X").PadLeft(4, '0')}  {Colors.LIGHT_YELLOW}IY:{Colors.ANSI_RESET} 0x{registers.IY.ToString("X").PadLeft(4, '0')}");

            Console.WriteLine($"\n{Colors.LIGHT_BLUE}Special registers{Colors.ANSI_RESET}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}IFF1:{Colors.ANSI_RESET} 0b{(registers.IFF1 ? "1" : "0")}  {Colors.LIGHT_YELLOW}IFF2:{Colors.ANSI_RESET} 0b{(registers.IFF2 ? "1" : "0")}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}I:{Colors.ANSI_RESET} 0x{registers.RegisterSet[I].ToString("X").PadLeft(2, '0').PadRight(4)}  {Colors.LIGHT_YELLOW}R:{Colors.ANSI_RESET} 0x{registers.RegisterSet[R].ToString("X").PadLeft(2, '0')}");

            Console.WriteLine($"\n{Colors.LIGHT_BLUE}Operating registers{Colors.ANSI_RESET}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}PC:{Colors.ANSI_RESET} 0x{registers.PC.ToString("X").PadLeft(4, '0')}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}SP:{Colors.ANSI_RESET} 0x{registers.SP.ToString("X").PadLeft(4, '0')}");
        }
        public static void DumpMemory()
        {
            int bytesPerLine = 16;
            ushort length = mainMemory.Length;
            int addrWidth = 4;

            Console.WriteLine();

            for (int addr = 0; addr < length; addr += bytesPerLine)
            {
                Console.Write($"{Colors.LIGHT_BLUE}0x{addr.ToString($"X{addrWidth}")}: {Colors.ANSI_RESET}");

                for (int i = 0; i < bytesPerLine && addr + i < length; i++)
                {
                    byte b = mainMemory.Read((ushort)(addr + i));
                    Console.Write($"{b:X2} ");
                }

                Console.Write($" {Colors.LIGHT_PINK}|{Colors.ANSI_RESET} ");
                for (int i = 0; i < bytesPerLine && addr + i < length; i++)
                {
                    byte b = mainMemory.Read((ushort)(addr + i));
                    char c = (b >= 32 && b <= 126) ? (char)b : '.';
                    Console.Write(c);
                }

                Console.WriteLine();
            }
        }
        private class DataBus : IDataBus
        {
            public bool MI { get; set; } = false;
            public bool NMI { get; set; } = false;
            public byte Data { get; set; }

            public byte ReadPort(ushort port)
            {
                //Console.Write($"IN 0x{port:X4}");
                return 0;
            }
            public void WritePort(ushort port, byte data)
            {
                //Console.Write($"OUT 0x{port:X4}, 0x{data:X2}");
            }
        }
    }
}