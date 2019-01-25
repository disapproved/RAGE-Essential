using GTANetworkAPI;
using RAGECore;
using RAGETimers;
using System;

namespace RAGETest
{
    public class TestManager : Script, RAGEModule
    {
        public int Version { get; set; } = 1;
        public string Name { get; set; } = "Test Module";

        public void Load()
        {
            Console.WriteLine("Hello from test module.... epic victory royale...");
        }

        public void Unload()
        {

        }

        public void Start()
        {
            Console.WriteLine("Here 0");
            TestCase1();
        }

        public void TestCase1()
        {
            Console.WriteLine("Here 1");
            TimerManager timers = Core.GetModule<TimerManager>();
            Console.WriteLine("Here 2");
            if (timers == default(TimerManager))
                Console.WriteLine("Is default");
            timers.ScheduleSyncEvent(2000, new Action(() =>
            {
                Console.WriteLine("Helloo....");
            }), true);
            Console.WriteLine("Here 3");
        }
    }
}
