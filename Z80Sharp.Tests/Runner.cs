using System.Text.Json;
using Z80Sharp.Constants;
using Z80Sharp.Enums;
using Z80Sharp.Memory;
using Z80Sharp.Processor;
using Z80Sharp.Registers;

namespace Z80Sharp.Tests
{
    internal class Runner
    {
        private readonly Z80 z80;
        private readonly MainMemory memory;
        static ProcessorRegisters state = new ProcessorRegisters();

        internal Runner(Z80 z80, MainMemory memory, bool undoc)
        {
            this.z80 = z80;
            this.memory = memory;

            string testInput = File.ReadAllText(@"tests.in.json");
            var machineStates = JsonSerializer.Deserialize<List<MachineState>>(testInput);

            string expectedOutput = File.ReadAllText(@"tests.expected.json");
            var expectedStates = JsonSerializer.Deserialize<List<ExpectedState>>(expectedOutput);

            if (!undoc) machineStates.RemoveAll(state => Constants.UndocumentedInstructions.Contains(state.name.Split("_")[0]));

            int passed = 0;
            foreach (var machineState in machineStates)
            {
                ExpectedState expected = expectedStates.First(e => e.name == machineState.name);
                ExecuteTest(machineState, expected.state.tStates);
                var (pass, badRegisters) = EvaluateResult(expected);

                string status = pass ? "PASS" : "FAIL";
                string color = pass ? Colors.GREEN : Colors.RED;
                string failedRegs = pass ? "" : $", bad registers: {string.Join(", ", badRegisters)}";
                passed += pass ? 1 : 0;

                Console.WriteLine($"{color}{status}{Colors.ANSI_RESET}: Test {machineState.name}{failedRegs}");
            }
            Console.WriteLine($"{Colors.GREEN}{passed}{Colors.ANSI_RESET} tests passed, {Colors.RED}{machineStates.Count - passed}{Colors.ANSI_RESET} tests failed.");
        }

        private void ExecuteTest(MachineState emulatorState, long tStates)
        {
            state.AF = (ushort)emulatorState.state.af;
            state.BC = (ushort)emulatorState.state.bc;
            state.DE = (ushort)emulatorState.state.de;
            state.HL = (ushort)emulatorState.state.hl;

            state.AF_ = (ushort)emulatorState.state.afDash;
            state.BC_ = (ushort)emulatorState.state.bcDash;
            state.DE_ = (ushort)emulatorState.state.deDash;
            state.HL_ = (ushort)emulatorState.state.hlDash;

            state.IX = (ushort)emulatorState.state.ix;
            state.IY = (ushort)emulatorState.state.iy;

            state.SP = (ushort)emulatorState.state.sp;
            state.PC = (ushort)emulatorState.state.pc;

            state.IFF1 = emulatorState.state.iff1;
            state.IFF2 = emulatorState.state.iff2;
            state.InterruptMode = emulatorState.state.im switch
            {
                0 => InterruptMode.IM0,
                1 => InterruptMode.IM1,
                2 => InterruptMode.IM2,
                _ => InterruptMode.IM0
            };

            state.I = (byte)emulatorState.state.i;
            state.R = (byte)emulatorState.state.r;

            z80.Reset(state);

            for (int i = 0; i < memory.Length; i++)
            {
                memory.Write((ushort)i, 0);
            }

            foreach (var list in emulatorState.memory)
            {
                for (int i = list.address; i < list.address + list.data.Count; i++)
                {
                    memory.Write((ushort)i, (byte)list.data[i - list.address]);
                }
            }

            z80.RunUntil(tStates);
        }

        private (bool passed, string[] badRegisters) EvaluateResult(ExpectedState expectedState)
        {
            List<string> badRegisters = new List<string>();


            if (z80.Registers.A != (byte)(expectedState.state.af >> 8))
                badRegisters.Add($"A (0x{z80.Registers.A:X2}) : (0x{(expectedState.state.af >> 8) & 0xFF:X2})");
            //if (z80.Registers.F != (byte)expectedState.state.af)
            //    badRegisters.Add($"F (0x{z80.Registers.F:X2}) : (0x{expectedState.state.af & 0xFF:X2})");

            if (z80.Registers.B != (byte)(expectedState.state.bc >> 8))
                badRegisters.Add($"B (0x{z80.Registers.B:X2}) : (0x{(expectedState.state.bc >> 8) & 0xFF:X2})");
            if (z80.Registers.C != (byte)expectedState.state.bc)
                badRegisters.Add($"C (0x{z80.Registers.C:X2}) : (0x{expectedState.state.bc & 0xFF:X2})");

            if (z80.Registers.D != (byte)(expectedState.state.de >> 8))
                badRegisters.Add($"D (0x{z80.Registers.D:X2}) : (0x{(expectedState.state.de >> 8) & 0xFF:X2})");
            if (z80.Registers.E != (byte)expectedState.state.de)
                badRegisters.Add($"E (0x{z80.Registers.E:X2}) : (0x{expectedState.state.de & 0xFF:X2})");

            if (z80.Registers.H != (byte)(expectedState.state.hl >> 8))
                badRegisters.Add($"H (0x{z80.Registers.H:X2}) : (0x{(expectedState.state.hl >> 8) & 0xFF:X2})");
            if (z80.Registers.L != (byte)expectedState.state.hl)
                badRegisters.Add($"L (0x{z80.Registers.L:X2}) : (0x{expectedState.state.hl & 0xFF:X2})");

            if (z80.Registers.AF_ != expectedState.state.afDash)
                badRegisters.Add($"AF' (0x{z80.Registers.AF_:X4}) : (0x{expectedState.state.afDash:X4})");
            if (z80.Registers.BC_ != expectedState.state.bcDash)
                badRegisters.Add($"BC' (0x{z80.Registers.BC_:X4}) : (0x{expectedState.state.bcDash:X4})");
            if (z80.Registers.DE_ != expectedState.state.deDash)
                badRegisters.Add($"DE' (0x{z80.Registers.DE_:X4}) : (0x{expectedState.state.deDash:X4})");
            if (z80.Registers.HL_ != expectedState.state.hlDash)
                badRegisters.Add($"HL' (0x{z80.Registers.HL_:X4}) : (0x{expectedState.state.hlDash:X4})");

            if (z80.Registers.IX != expectedState.state.ix)
                badRegisters.Add($"IX (0x{z80.Registers.IX:X4}) : (0x{expectedState.state.ix:X4})");
            if (z80.Registers.IY != expectedState.state.iy)
                badRegisters.Add($"IY (0x{z80.Registers.IY:X4}) : (0x{expectedState.state.iy:X4})");

            if (z80.Registers.SP != expectedState.state.sp)
                badRegisters.Add($"SP (0x{z80.Registers.SP:X4}) : (0x{expectedState.state.sp:X4})");
            //if (z80.Registers.PC != expectedState.state.pc)
            //    badRegisters.Add($"PC (0x{z80.Registers.PC:X4}) : (0x{expectedState.state.pc:X4})");

            if (z80.Registers.I != expectedState.state.i)
                badRegisters.Add($"I (0x{z80.Registers.I:X2}) : (0x{expectedState.state.i:X2})");
            //if (z80.Registers.R != expectedState.state.r)
            //    badRegisters.Add($"R (0x{z80.Registers.R:X2}) : (0x{expectedState.state.r:X2})");

            bool passed = badRegisters.Count == 0;

            return (passed, badRegisters.ToArray());
        }

    }
}
