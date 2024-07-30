﻿using Z80Sharp.Logging;
using Z80Sharp.Processor;
using Z80Sharp.Enums;
using Z80Sharp.Constants;
using Z80Sharp.Interfaces;
using Z80Sharp.Registers;
using static Z80Sharp.Registers.ProcessorRegisters;
using System.Diagnostics;

namespace Z80Sharp
{
    public static class Program
    {
        private static bool quitRequested = false;
        private static IZ80Logger logger = new Logger(useColors: true);
        private static IProcessor z80 = new DebugZ80(5, logger);

        public static void Main(string[] args)
        {
            logger.LogGenerated += Logger_LogGenerated;

            logger.Log(LogSeverity.Debug, "Debug");
            logger.Log(LogSeverity.Info, "Information");
            logger.Log(LogSeverity.Memory, "Memory");
            logger.Log(LogSeverity.Interrupt, "Interrupt");
            logger.Log(LogSeverity.Decode, "Processor decode");
            logger.Log(LogSeverity.Execution, "Processor execute");
            logger.Log(LogSeverity.Warning, "Warning");
            logger.Log(LogSeverity.Fatal, "Fatal");

            z80.Reset();
            Thread processorThread = new(() => z80.Run());
            processorThread.Start();

            ReadConsoleInput();
        }

        private static void Logger_LogGenerated(object? sender, Events.LogGeneratedEventArgs e)
        {
            Console.WriteLine(e.LogData);
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
                        Console.Write($"\n{Colors.CYAN}{Colors.ANSI_BOLD}==========Help=========={Colors.ANSI_RESET}\n{Colors.WHITE}{Colors.ANSI_ITALIC}Commands:{Colors.ANSI_RESET}\n{Colors.LIGHT_YELLOW}> h{Colors.ANSI_RESET} - prints this help text.\n{Colors.LIGHT_YELLOW}> d{Colors.ANSI_RESET} - dumps the processor state.\n{Colors.LIGHT_YELLOW}> q{Colors.ANSI_RESET} - quits the emulator and console.");
                        break;
                    case 'd':
                        Console.Write($"\n{Colors.ANSI_BOLD}{Colors.GREEN}=====Processor state===={Colors.ANSI_RESET}");
                        PrintProcessorState(z80.Registers);
                        break;
                    default:
                        Console.Write($"\n{Colors.RED}Invalid command. Press 'h' for help.{Colors.ANSI_RESET}");
                        break;
                }
                Console.WriteLine();
            }
        }

        public static void PrintProcessorState(IRegisterSet registers)
        {
            Console.WriteLine($"\n{Colors.LIGHT_BLUE}General-purpose registers{Colors.ANSI_RESET}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}B:{Colors.ANSI_RESET} 0x{registers.RegisterSet[B].ToString("X").PadLeft(2, '0')}    {Colors.LIGHT_YELLOW}C:{Colors.ANSI_RESET} 0x{registers.RegisterSet[C].ToString("X").PadLeft(2, '0')}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}D:{Colors.ANSI_RESET} 0x{registers.RegisterSet[D].ToString("X").PadLeft(2, '0')}    {Colors.LIGHT_YELLOW}E:{Colors.ANSI_RESET} 0x{registers.RegisterSet[E].ToString("X").PadLeft(2, '0')}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}H:{Colors.ANSI_RESET} 0x{registers.RegisterSet[H].ToString("X").PadLeft(2, '0')}    {Colors.LIGHT_YELLOW}L:{Colors.ANSI_RESET} 0x{registers.RegisterSet[L].ToString("X").PadLeft(2, '0')}");

            Console.WriteLine($"\n{Colors.LIGHT_BLUE}Data registers{Colors.ANSI_RESET}");
            Console.WriteLine($"{Colors.LIGHT_YELLOW}A:{Colors.ANSI_RESET} 0x{registers.RegisterSet[A].ToString("X").PadLeft(2, '0')}");
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
    }
}