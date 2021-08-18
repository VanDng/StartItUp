using CommonImplementation.WindowsAPI;
using System;

namespace ExtensionTester
{
    class Program
    {
        static void Main(string[] args)
        {
            Worksnaps.Main m = new Worksnaps.Main();
            m.Start();

            Console.ReadKey();
        }
    }
}
