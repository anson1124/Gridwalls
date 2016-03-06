using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;
using ModWrapper;
using SimpleClient;

namespace TurnIncrementerMod
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
        private readonly TurnIncrementer turnIncrementer;

        public ModRunner()
        {
            logger = new FileLogger("../../../Logs/", "TurnncrementerMod_", "css");
            client = ClientFactory.Create(logger);
            turnIncrementer = new TurnIncrementer(logger, client);
        }

        public void ConnectToServer(string host, int port)
        {
            logger.Write<ModRunner>("Connecting to server...");
            client.OnConnected += onConnected;
            client.Connect(host, port);
            client.OnDisconnected += onDisconnected;
        }
        private void onConnected()
        {
            turnIncrementer.StartSendingEvents();
        }

        private void onDisconnected()
        {
            logger.Write<ModRunner>("Disconnected from server.");
        }

        public void Disconnect()
        {
            turnIncrementer.StopSendingEvents();
            client.Disconnect();
        }


    }
}
