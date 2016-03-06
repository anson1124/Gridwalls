using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Messaging;
using SimpleClient;
using SimpleTcpServer;

namespace SimpleClient
{
    public class Server
    {
        public event Action OnClientConnected;

        private readonly Logger logger;

        private readonly IConnectionListener connectionListener;
        private readonly ICommunicator communicator;

        public Server(Logger logger, IConnectionListener connectionListener, ICommunicator communicator)
        {
            this.logger = logger;
            this.connectionListener = connectionListener;
            this.communicator = communicator;
        }

        public void ListenForConnectionsInANewThread(int port)
        {
            connectionListener.ListenForConnectionsInANewThread(port);
            connectionListener.OnClientConnected += tcpClient =>
            {
                communicator.SetupCommunicationWith(tcpClient);
                OnClientConnected?.Invoke();
            };
        }

        public void Shutdown()
        {
            logger.Write<Server>("Shutting down server.");
            connectionListener.StopListening();
            communicator.CloseConnectionToAllClients();
        }

    }
}
