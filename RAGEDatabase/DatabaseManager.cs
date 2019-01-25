using GTANetworkAPI;
using RAGECore;
using System;

namespace RAGEDatabase
{
    public class DatabaseManager : Script, RAGEModule
    {
        public int Version { get; set; } = 1;
        public string Name { get; set; } = "Database Module";

        public void Load()
        {
            Console.WriteLine("Hello from database module.... epic victory royale...");
        }
        
        public void Unload()
        {

        }
    }
}
