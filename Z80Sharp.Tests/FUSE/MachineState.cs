namespace Z80Sharp.Tests.FUSE
{
    public class MachineState
    {
        public string name { get; set; }
        public State state { get; set; }
        public List<Memory> memory { get; set; }
    }

    public class State
    {
        public int af { get; set; }
        public int bc { get; set; }
        public int de { get; set; }
        public int hl { get; set; }
        public int afDash { get; set; }
        public int bcDash { get; set; }
        public int deDash { get; set; }
        public int hlDash { get; set; }
        public int ix { get; set; }
        public int iy { get; set; }
        public int sp { get; set; }
        public int pc { get; set; }
        public int memptr { get; set; }
        public int i { get; set; }
        public int r { get; set; }
        public bool iff1 { get; set; }
        public bool iff2 { get; set; }
        public int im { get; set; }
        public bool halted { get; set; }
        public int tStates { get; set; }
    }

    public class Memory
    {
        public int address { get; set; }
        public List<int> data { get; set; }
    }
}
