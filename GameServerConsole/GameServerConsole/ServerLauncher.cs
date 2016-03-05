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
        private Launcher launcher;

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
            launcher.Disconnect();
            Thread.Sleep(1000);
        }

        private void onClientConnected()
        {
            Console.WriteLine("Client connected!");
        }

        private void runGameOnConnectOrContinueIfUserPressesEscape()
        {
            string connectOrEscape = waitForCLientToConnectOrUserPressesEscape();
            Console.WriteLine("What happened: " + connectOrEscape);
        }

        private string waitForCLientToConnectOrUserPressesEscape()
        {
            while (true)
            {
                Console.WriteLine("Press escape to shutdown server.");
                if (Console.KeyAvailable && Console.ReadKey().Key.Equals(ConsoleKey.Escape))
                {
                    return "userPressedEscape";
                }

                Thread.Sleep(1000);
            }
        }
    }
}
