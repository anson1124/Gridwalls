using System;
using Logging;
using ModWrapper;
using SimpleClient;

namespace SkeletonMod
{
    public class ModRunner : Mod
    {
        public event Action OnDisconnect
        {
            add { client.OnDisconnected += value; }
            remove { client.OnDisconnected -= value; }
        }

        private readonly Client client;
        private readonly Logger logger;

        public ModRunner()
        {
            logger = new FileLogger("../../../Logs/", "SkeletonMod_", "css");
            client = ClientFactory.Create(logger);
        }

        public void ConnectToServer(string host, int port)
        {
            logger.Write<ModRunner>("Connecting to server...");
            client.OnConnected += onConnected;
            client.OnMessageReceived += messageReceived;
            client.Connect(host, port);
            client.OnDisconnected += onDisconnected;
        }

        private void onConnected()
        {
            client.SendMessage("Skeleton reporting!");
        }

        private DateTime last = DateTime.Now;

        private void messageReceived(string msg)
        {
            TimeSpan duration = DateTime.Now - last;
            last = DateTime.Now;
            logger.Write<ModRunner>("Time since last message: " + duration);
        }

        private void onDisconnected()
        {
            logger.Write<ModRunner>("Disconnected from server.");
        }

        public void Disconnect()
        {
            client.Disconnect();
        }
    }
}