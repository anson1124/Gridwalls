using System;
using Logging;
using ModWrapper;
using SimpleClient;

namespace SkeletonMod
{
    public class ModRunner : Mod
    {
        private readonly Client client;
        private readonly Logger logger;

        public ModRunner()
        {
            logger = new FileLogger("../../../Logs/", "SkeletonMod_", "css");
            client = new Client(logger);
        }

        public void ConnectToServer(string host, int port)
        {
            logger.Write<ModRunner>("Connecting to server...");
            client.OnConnected += onConnected;
            client.Connect(host, port);
        }

        private void onConnected()
        {
            client.SendMessage("Skeleton reporting!");
        }

        public void Disconnect()
        {
            client.Disconnect();
        }
    }
}