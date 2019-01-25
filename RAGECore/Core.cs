using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;

namespace RAGECore
{
    public interface RAGEModule
    {
        int Version { get; set; }
        string Name { get; set; }
        void Load();
        void Unload();
    }

    public class Core : Script
    {
        List<object> LoadedModules = new List<object>();

        public T GetModule<T>()
        {
            return (T)LoadedModules.FirstOrDefault(x => x.GetType() == typeof(T));
        }

        [ServerEvent(Event.ResourceStart)]
        public void OnResourceStart()
        {
            Console.ForegroundColor = ConsoleColor.White;
            Console.WriteLine("-<RAGE Modular Core>- is starting...");

            Console.ForegroundColor = ConsoleColor.Yellow;
            Console.WriteLine("Loading modules...");

            /*TODO: SUPPORT FOR DLL DE-LOADING (CUSTOM APPDOMAIN)*/
            string path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            foreach(string dll in Directory.GetFiles(path + "\\Modules\\", "*.dll"))
            {
                Assembly ass = Assembly.LoadFile(dll);
                Type type = ass.GetExportedTypes().FirstOrDefault(x => typeof(RAGEModule).IsAssignableFrom(x)); //check later for epic fail.....
                if(type == default(Type)) //epic fail....
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error Loading Module -<{type.ToString()}>-: Missing RAGEModule Interface");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                    continue;
                }
                else
                {
                    Console.WriteLine($"Loading Module -<{type.ToString()}>-...");
                }
                try
                {
                    object c = Activator.CreateInstance(type);
                    LoadedModules.Add(c);

                    Console.ForegroundColor = ConsoleColor.White; //in case noob devs forget to switch color in their loads...
                    (c as RAGEModule).Load();

                    Console.ForegroundColor = ConsoleColor.Green;
                    Console.WriteLine("Success!");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
                catch(Exception e)
                {
                    Console.ForegroundColor = ConsoleColor.Red;
                    Console.WriteLine($"Error Loading Module -<{type.ToString()}>-: {e.ToString()}");
                    Console.ForegroundColor = ConsoleColor.Yellow;
                }
            }
            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-<RAGE Modular Core>- has loaded!");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
