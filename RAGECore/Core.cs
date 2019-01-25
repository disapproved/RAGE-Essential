using GTANetworkAPI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.Loader;

namespace RAGECore
{
    public interface RAGEModule
    {
        int Version { get; set; }
        string Name { get; set; }
        void Load();
        void Unload();
        void Start();
    }

    public class Core : Script
    {
        public static List<object> LoadedModules = new List<object>();
        public static List<MethodInfo> HookedEvents = new List<MethodInfo>();
        public static Dictionary<MethodInfo, object> HookedClasses = new Dictionary<MethodInfo, object>();

        public static void InvokeElevatedAction(Action act)
        {
            act.Invoke();
        }

        public static T GetModule<T>()
        {
            return (T)LoadedModules.FirstOrDefault(x => x.GetType() == typeof(T));
        }

        [ServerEvent(Event.Update)]
        public void OnUpdate()
        {
            try
            {
                foreach (MethodInfo method in HookedEvents)
                {
                    ServerEventAttribute attr = (ServerEventAttribute)method.GetCustomAttribute(typeof(ServerEventAttribute), false);
                    if (attr.EventId == Event.Update)
                        method.Invoke(HookedClasses[method], null);
                }
            }
            catch(Exception e)
            {
                Console.WriteLine(e.ToString());
            }
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
                var methods = ass.GetTypes().SelectMany(t => t.GetMethods())
                    .Where(m => m.GetCustomAttributes(typeof(ServerEventAttribute), false).Length > 0)
                    .ToList();
                HookedEvents.AddRange(methods);
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
                    foreach (var meth in methods)
                        HookedClasses[meth] = c;
                    LoadedModules.Add(c);
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

            foreach(object c in LoadedModules)
            {
                Console.ForegroundColor = ConsoleColor.White; //in case noob devs forget to switch color in their loads...
                (c as RAGEModule).Load();
            }

            foreach (object c in LoadedModules)
            {
                Console.ForegroundColor = ConsoleColor.White;
                (c as RAGEModule).Start();
            }

            Console.ForegroundColor = ConsoleColor.Green;
            Console.WriteLine("-<RAGE Modular Core>- has loaded!");
            Console.ForegroundColor = ConsoleColor.White;
        }
    }
}
