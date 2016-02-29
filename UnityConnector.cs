using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;

namespace GameClient
{
    public class UnityConnector : IDisposable
    {
        private readonly FileLogger fileLogger = new FileLogger(typeof (UnityConnector), "Logs/", "gameClient_", "css");

        public void ConnectToServer()
        {
            var client = new Client();
            
        }

        public void NextTurn()
        {
            int rnd = new Random().Next(0, 100);
            fileLogger.Write("Next turn! Random = " + rnd);
        }

        public void Dispose()
        {
            fileLogger.Dispose();
        }
    }
}
