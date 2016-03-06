using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using ModWrapper;

namespace ModRunnerConsole
{
    class Program
    {
        private const String Host = "127.0.0.1";
        private const int Port = 22050;

        static void Main(string[] args)
        {
            Console.WriteLine("Running mods...");

            Console.WriteLine("Running SkeletonMod...");
            Mod skeletonMod = new SkeletonMod.ModRunner();
            skeletonMod.OnDisconnect += onDisconnect;
            skeletonMod.ConnectToServer(Host, Port);
            Console.WriteLine("SkeletonMod connected.");

            Console.WriteLine("Running TurnIncrementerMod...");
            Mod turnIncrementerMod = new TurnIncrementerMod.ModRunner();
            turnIncrementerMod.OnDisconnect += onDisconnect;
            turnIncrementerMod.ConnectToServer(Host, Port);
            Console.WriteLine("TurnIncrementerMod connected.");

            exitWhenUserPressesEscape();

            Console.WriteLine("Disconnecting SkeletonMod...");
            skeletonMod.Disconnect();

            Console.WriteLine("Disconnecting TurnIncrementerMod...");
            turnIncrementerMod.Disconnect();

            Console.WriteLine("Quitting.");
        }

        private static void onDisconnect()
        {
            Console.WriteLine("SkeletonMod disconnected.");
        }

        private static void exitWhenUserPressesEscape()
        {
            while (true)
            {
                if (Console.KeyAvailable && Console.ReadKey().Key.Equals(ConsoleKey.Escape))
                {
                    return;
                }

                Thread.Sleep(1000);
            }
        }
    }
}
