using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;
using SimpleClient;

namespace GameClient
{
    public class Game : IDisposable
    {
        public Action OnConnected;

        private readonly FileLogger fileLogger = new FileLogger("Logs/", "gameClient_", "css");
        private Client client;

        public void ConnectToServer()
        {
            client = new Client(fileLogger);
            client.Connect("127.0.0.1", 22050);
            client.OnConnected += () => OnConnected?.Invoke();
            client.OnMessageReceived += onMessageReceived;
        }

        private void onMessageReceived(string msg)
        {
            fileLogger.Write<Game>("Received message: " + msg);
        }

        public void ManMove()
        {
            int rnd = new Random().Next(0, 100);
            client.SendMessage("Man moved to location");
            fileLogger.Write<Game>("Next turn! Random = " + rnd);
        }

        public void Quit()
        {
            client.Disconnect();
            Dispose();
        }

        public void Dispose()
        {
            fileLogger.Dispose();
        }
    }
}
