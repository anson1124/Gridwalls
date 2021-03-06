﻿using System;
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

            List<Mod> mods = new List<Mod>();

            //Console.WriteLine("Running SkeletonMod...");
            //Mod skeletonMod = new SkeletonMod.ModRunner();
            //skeletonMod.OnDisconnect += onDisconnect;
            //skeletonMod.ConnectToServer(Host, Port);
            //mods.Add(skeletonMod);
            //Console.WriteLine("SkeletonMod connected.");

            //Console.WriteLine("Running TurnIncrementerMod...");
            //Mod turnIncrementerMod = new TurnIncrementerMod.ModRunner();
            //turnIncrementerMod.OnDisconnect += onDisconnect;
            //turnIncrementerMod.ConnectToServer(Host, Port);
            //mods.Add(skeletonMod);
            //Console.WriteLine("TurnIncrementerMod connected.");

            Console.WriteLine("Running TurnIncrementerMod...");
            Mod performanceMod = new PerfomanceTestMod.ModRunner();
            performanceMod.OnDisconnect += onDisconnect;
            performanceMod.ConnectToServer(Host, Port);
            Console.WriteLine("TurnIncrementerMod connected.");

            exitWhenUserPressesEscape();

            performanceMod.Disconnect();
            foreach (var mod in mods)
            {
                Console.WriteLine("Disconnecting " + mod.GetType());
                mod.Disconnect();
            }

            Console.WriteLine("Quitting.");
        }

        private static void onDisconnect()
        {
            Console.WriteLine("Mod disconnected.");
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
