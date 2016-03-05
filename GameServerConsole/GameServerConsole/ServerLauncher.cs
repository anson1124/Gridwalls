using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using GameServer;
using Logging;

namespace GameServerConsole
{
    class ServerLauncher
    {
        private bool clientConnected;

        static void Main(string[] args)
        {
            new ServerLauncher().Run();
        }

        private void Run()
        {
            Logger logger = new FileLogger("../../../Logs/", "GameServer_", "css");
            Launcher launcher = new Launcher(logger);
            launcher.ClientConnected += onClientConnected;
            launcher.Host(22050);
            runGameOnConnectOrContinueIfUserPressesEscape();
            launcher.Disconect();
            Thread.Sleep(1000);
        }

        private void onClientConnected()
        {
            clientConnected = true;
            Console.WriteLine("Client connected!");
        }

        private void runGameOnConnectOrContinueIfUserPressesEscape()
        {
            string connectOrEscape = waitForCLientToConnectOrUserPressesEscape();
            Console.WriteLine("What happened: " + connectOrEscape);
            if (connectOrEscape.Equals("clientConnected"))
            {
                runGame(); // Game ends after x turns or when user presses escape
            }
        }

        private string waitForCLientToConnectOrUserPressesEscape()
        {
            while (true)
            {
                Console.WriteLine("Waiting for user to connect. Press escape to cancel.");
                if (clientConnected)
                {
                    return "clientConnected";
                }

                if (Console.KeyAvailable && Console.ReadKey().Key.Equals(ConsoleKey.Escape))
                {
                    return "userPressedEscape";
                }

                Thread.Sleep(1000);
            }
        }

        private void runGame()
        {
            Console.WriteLine("Starting game which does some work...");
        }

    }
}
