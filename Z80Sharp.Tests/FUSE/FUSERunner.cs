using System.Text.Json;
using Z80Sharp.Constants;
using Z80Sharp.Enums;
using Z80Sharp.Data;
using Z80Sharp.Processor;
using Z80Sharp.Registers;
using Z80Sharp.Logging;
using Z80Sharp.Interfaces;

namespace Z80Sharp.Tests.FUSE
{
    internal class FUSERunner
    {
        readonly Z80 z80;
        readonly IZ80Logger logger = new Logger(useColors: false);
        readonly MainMemory memory = new MainMemory(65536);

        private readonly bool _undocumentedTests;

        internal FUSERunner(bool undocumentedTests)
        {
            _undocumentedTests = undocumentedTests;

            var dataBus = new DataBus
            {
                ReadPort = (port) => 0x00,
                WritePort = (port, data) => Console.Write((char)data)
            };

            z80 = new(memory, dataBus, logger, 0);
            z80.Reset();
        }

        internal void RunFUSETests()
        {
            var machineStates = LoadJson<List<MachineState>>(@"tests.in.json");
            var expectedStates = LoadJson<List<ExpectedState>>(@"tests.expected.json");

            if (!_undocumentedTests) machineStates.RemoveAll(state => Constants.UndocumentedInstructions.Contains(state.name.Split('_')[0]));

            int passed = ExecuteTest(machineStates, expectedStates);
            Console.WriteLine($"{Colors.GREEN}{passed}{Colors.ANSI_RESET} tests passed, {Colors.RED}{machineStates.Count - passed}{Colors.ANSI_RESET} tests failed.");
        }

        private static T LoadJson<T>(string path) where T : class
        {
            return JsonSerializer.Deserialize<T>(File.ReadAllText(path)) ?? throw new Exception($"Failed to load {path}");
        }

        private int ExecuteTest(List<MachineState> machineStates, List<ExpectedState> expectedStates)
        {
            int passed = 0;

            foreach (var machineState in machineStates)
            {
                var expected = expectedStates.First(e => e.name == machineState.name);
                ExecuteTest(machineState, (ulong)expected.state.tStates);
                var (pass, badRegisters) = EvaluateResult(expected);

                Console.WriteLine($"{(pass ? Colors.GREEN : Colors.RED)}{(pass ? "PASS" : "FAIL")}{Colors.ANSI_RESET}: Test {machineState.name}{(pass ? "" : $", bad registers: {string.Join(", ", badRegisters)}")}");
                if (pass) passed++;
            }

            return passed;
        }

        private void ExecuteTest(MachineState emulatorState, ulong tStates)
        {
            var state = InitializeRegisters(emulatorState.state);
            z80.Reset(state);
            ClearMemory();
            LoadMemory(emulatorState.memory);
            z80.RunUntil(tStates);
        }

        private static ProcessorRegisters InitializeRegisters(State stateData)
        {
            return new ProcessorRegisters
            {
                AF = (ushort)stateData.af,
                BC = (ushort)stateData.bc,
                DE = (ushort)stateData.de,
                HL = (ushort)stateData.hl,
                AF_ = (ushort)stateData.afDash,
                BC_ = (ushort)stateData.bcDash,
                DE_ = (ushort)stateData.deDash,
                HL_ = (ushort)stateData.hlDash,
                IX = (ushort)stateData.ix,
                IY = (ushort)stateData.iy,
                SP = (ushort)stateData.sp,
                PC = (ushort)stateData.pc,
                IFF1 = stateData.iff1,
                IFF2 = stateData.iff2,
                InterruptMode = stateData.im switch { 0 => InterruptMode.IM0, 1 => InterruptMode.IM1, 2 => InterruptMode.IM2, _ => InterruptMode.IM0 },
                I = (byte)stateData.i,
                R = (byte)stateData.r
            };
        }

        private void ClearMemory()
        {
            for (int i = 0; i < memory.Length; i++)
            {
                memory.Write((ushort)i, 0);
            }
        }

        private void LoadMemory(List<Memory> memoryBlocks)
        {
            foreach (var block in memoryBlocks)
            {
                for (int i = block.address; i < block.address + block.data.Count; i++)
                {
                    memory.Write((ushort)i, (byte)block.data[i - block.address]);
                }
            }
        }

        private (bool passed, string[] badRegisters) EvaluateResult(ExpectedState expectedState)
        {
            var badRegisters = new List<string>();
            void CompareUshort(string name, ushort actual, ushort expected) { if (actual != expected) badRegisters.Add($"{name} (0x{actual:X4}) : (0x{expected:X4})"); }
            void CompareByte(string name, byte actual, byte expected) { if (actual != expected) badRegisters.Add($"{name} (0x{actual:X2}) : (0x{expected:X2})"); }

            CompareByte("A", z80.Registers.A, (byte)(expectedState.state.af >> 8));
            CompareByte("F", z80.Registers.F, (byte)(expectedState.state.af & 0xFF));
            CompareByte("B", z80.Registers.B, (byte)(expectedState.state.bc >> 8));
            CompareByte("C", z80.Registers.C, (byte)expectedState.state.bc);
            CompareByte("D", z80.Registers.D, (byte)(expectedState.state.de >> 8));
            CompareByte("E", z80.Registers.E, (byte)expectedState.state.de);
            CompareByte("H", z80.Registers.H, (byte)(expectedState.state.hl >> 8));
            CompareByte("L", z80.Registers.L, (byte)expectedState.state.hl);
            CompareUshort("AF'", z80.Registers.AF_, (ushort)expectedState.state.afDash);
            CompareUshort("BC'", z80.Registers.BC_, (ushort)expectedState.state.bcDash);
            CompareUshort("DE'", z80.Registers.DE_, (ushort)expectedState.state.deDash);
            CompareUshort("HL'", z80.Registers.HL_, (ushort)expectedState.state.hlDash);
            CompareUshort("IX", z80.Registers.IX, (ushort)expectedState.state.ix);
            CompareUshort("IY", z80.Registers.IY, (ushort)expectedState.state.iy);
            CompareUshort("SP", z80.Registers.SP, (ushort)expectedState.state.sp);
            CompareUshort("PC", z80.Registers.PC, (ushort)expectedState.state.pc);
            CompareByte("I", z80.Registers.I, (byte)expectedState.state.i);
            CompareByte("R", z80.Registers.R, (byte)expectedState.state.r);

            return (badRegisters.Count == 0, badRegisters.ToArray());
        }
    }
}