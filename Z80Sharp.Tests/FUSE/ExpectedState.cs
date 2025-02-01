namespace Z80Sharp.Tests.FUSE
{
    public class BusActivity
    {
        public int time { get; set; }
        public string type { get; set; }
        public int address { get; set; }
        public int? value { get; set; }
    }

    public class ExpectedState
    {
        public string name { get; set; }
        public List<BusActivity> busActivity { get; set; }
        public State state { get; set; }
    }
}
