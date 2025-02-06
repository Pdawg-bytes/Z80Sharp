using Z80Sharp.Data;
using Z80Sharp.Enums;
using Z80Sharp.Logging;
using System.Text.Json;
using Z80Sharp.Constants;
using Z80Sharp.Processor;
using Z80Sharp.Registers;
using Z80Sharp.Interfaces;

namespace Z80Sharp.Tests.FUSE
{
    internal class FUSERunner
    {
        readonly Z80 z80;
        readonly IZ80Logger logger = new Logger(useColors: false);
        readonly Z80Sharp.Data.Memory memory = new Z80Sharp.Data.Memory(65536);

        internal FUSERunner()
        {
            var dataBus = new DataBus()
            {
                ReadPort = (port) => (byte)(port >> 8)
            };

            z80 = new(memory, dataBus, logger, 0);
            z80.Reset();
        }

        internal void RunFUSETests()
        {
            var machineStates = LoadJson<List<MachineState>>(@"tests.in.json");
            var expectedStates = LoadJson<List<ExpectedState>>(@"tests.expected.json");

            int passed = ExecuteTests(machineStates, expectedStates);
            Console.WriteLine($"{Colors.GREEN}{passed}{Colors.ANSI_RESET} tests passed, {Colors.RED}{machineStates.Count - passed}{Colors.ANSI_RESET} tests failed.");
        }

        private static T LoadJson<T>(string path) where T : class
        {
            return JsonSerializer.Deserialize<T>(File.ReadAllText(path)) ?? throw new Exception($"Failed to load {path}");
        }

        private int ExecuteTests(List<MachineState> machineStates, List<ExpectedState> expectedStates)
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
                MEMPTR = (ushort)stateData.memptr,
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

        private List<string> _mismatchedBytes = new();
        (bool memPass, List<string> mismatches) CompareMemory(List<BusActivity> busActivity)
        {
            _mismatchedBytes.Clear();

            foreach (var activity in busActivity)
            {
                if (activity.type == "MW" && activity.value.HasValue)
                {
                    int addr = activity.address;
                    byte expectedValue = (byte)activity.value.Value;
                    byte actualValue = memory.Read(addr);

                    if (actualValue != expectedValue)
                        _mismatchedBytes.Add($"Mismatch at 0x{addr:X4} ? 0x{actualValue:X2} : 0x{expectedValue:X2}");
                }
            }

            return (_mismatchedBytes.Count == 0, _mismatchedBytes);
        }

        private List<string> _badRegisters = new();
        private (bool passed, string[] badRegisters) EvaluateResult(ExpectedState expectedState)
        {
            _badRegisters.Clear();

            Compare("A", z80.Registers.A, (byte)(expectedState.state.af >> 8));
            if (z80.Registers.F != ((byte)(expectedState.state.af & 0xFF)))
            {
                string actualBinary = Convert.ToString(z80.Registers.F, 2).PadLeft(8, '0');
                string expectedBinary = Convert.ToString((byte)(expectedState.state.af & 0xFF), 2).PadLeft(8, '0');
                _badRegisters.Add($"F (0b{actualBinary}) : (0b{expectedBinary})");
            }
            Compare("B", z80.Registers.B, (byte)(expectedState.state.bc >> 8));
            Compare("C", z80.Registers.C, (byte)expectedState.state.bc);
            Compare("D", z80.Registers.D, (byte)(expectedState.state.de >> 8));
            Compare("E", z80.Registers.E, (byte)expectedState.state.de);
            Compare("H", z80.Registers.H, (byte)(expectedState.state.hl >> 8));
            Compare("L", z80.Registers.L, (byte)expectedState.state.hl);
            Compare("AF'", z80.Registers.AF_, (ushort)expectedState.state.afDash);
            Compare("BC'", z80.Registers.BC_, (ushort)expectedState.state.bcDash);
            Compare("DE'", z80.Registers.DE_, (ushort)expectedState.state.deDash);
            Compare("HL'", z80.Registers.HL_, (ushort)expectedState.state.hlDash);
            Compare("IX", z80.Registers.IX, (ushort)expectedState.state.ix);
            Compare("IY", z80.Registers.IY, (ushort)expectedState.state.iy);
            Compare("SP", z80.Registers.SP, (ushort)expectedState.state.sp);
            Compare("PC", z80.Registers.PC, (ushort)expectedState.state.pc);
            Compare("I", z80.Registers.I, (byte)expectedState.state.i);
            Compare("R", z80.Registers.R, (byte)expectedState.state.r);
            Compare("MEMPTR", z80.Registers.MEMPTR, (ushort)expectedState.state.memptr);

            int im = z80.Registers.InterruptMode switch
            {
                InterruptMode.IM0 => 0,
                InterruptMode.IM1 => 1,
                InterruptMode.IM2 => 2,
            };
            Compare("IM", im, expectedState.state.im);

            Compare("IFF1", z80.Registers.IFF1, expectedState.state.iff1);
            Compare("IFF2", z80.Registers.IFF2, expectedState.state.iff2);

            var memResults = CompareMemory(expectedState.busActivity);
            _badRegisters.AddRange(memResults.mismatches);

            return (_badRegisters.Count == 0 && memResults.memPass, _badRegisters.ToArray());
        }

        void Compare<T>(string name, T actual, T expected) where T : IComparable<T>
        {
            if (!actual.Equals(expected))
            {
                string actualStr = actual switch
                {
                    byte b => $"0x{b:X2}",
                    ushort us => $"0x{us:X4}",
                    int i => $"0x{i:X4}",
                    _ => actual.ToString()
                };

                string expectedStr = expected switch
                {
                    byte b => $"0x{b:X2}",
                    ushort us => $"0x{us:X4}",
                    int i => $"0x{i:X4}",
                    _ => expected.ToString()
                };

                _badRegisters.Add($"{name} ({actualStr}) : ({expectedStr})");
            }
        }
    }
}