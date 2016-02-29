using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Logging;

namespace GameClient
{
    public class Game : IDisposable
    {
        private readonly FileLogger fileLogger = new FileLogger("Logs/", "gameClient_", "css");

        public void NextTurn()
        {
            int rnd = new Random().Next(0, 100);
            fileLogger.Write<Game>("Next turn! Random = " + rnd);
        }

        public void Dispose()
        {
            fileLogger.Dispose();
        }
    }
}
