using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;
using SimpleServer;

namespace GameServer
{
    public class Launcher
    {
        public event Action ClientConnected;

        private readonly Logger logger;
        private Server server;

        public Launcher(Logger logger)
        {
            this.logger = logger;
        }

        public void Host(int port)
        {
            server = new Server(logger);
            server.ListenForConnectionsInANewThread(port);
            server.OnClientConnected += () => ClientConnected?.Invoke();
        }

        public void Disconnect()
        {
            logger.Write<Launcher>("Shutting down server.");
            server.Shutdown();
        }
    }
}
