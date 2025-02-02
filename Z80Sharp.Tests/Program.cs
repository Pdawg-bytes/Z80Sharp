using Z80Sharp.Interfaces;
using Z80Sharp.Logging;
using Z80Sharp.Processor;
using Z80Sharp.Tests.FUSE;
using Z80Sharp.Tests.Prelim;
using Z80Sharp.Tests.Zex;

namespace Z80Sharp.Tests
{
    internal class Program
    {
        static void Main(string[] args)
        {
            ZexRunner zexRunner = new();
            PrelimRunner prelimRunner = new();
            FUSERunner fuseRunner = new(false);

            Console.WriteLine("Enter the test # you'd like to execute:\n    1. Zexdoc\n    2. Zexall\n    3. Prelim\n    4. FUSE");
            int test = Console.ReadKey().KeyChar - '0';
            Console.WriteLine();

            switch (test)
            {
                case 1:
                    zexRunner.RunZex(ZexType.Zexdoc);
                    break;
                case 2:
                    zexRunner.RunZex(ZexType.Zexall);
                    break;
                case 3:
                    prelimRunner.RunPrelim();
                    break;
                case 4:
                    fuseRunner.RunFUSETests();
                    break;

                default:
                    Console.WriteLine("Invalid test code, please restart.");
                    return;
            }
        }
    }
}
