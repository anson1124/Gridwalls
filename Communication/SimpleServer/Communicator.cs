using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Logging;
using Messaging;
using SimpleServer;

namespace SimpleServer
{
    public class Communicator : ICommunicator
    {
        private readonly Logger logger;
        private readonly IClientNodeFactory clientNodeFactory;
        private readonly MessageDispatcher messageDispatcher;

        private readonly List<Node> clientNodes = new List<Node>();

        public Communicator(Logger logger, IClientNodeFactory clientNodeFactory, MessageDispatcher messageDispatcher)
        {
            this.logger = logger;
            this.clientNodeFactory = clientNodeFactory;
            this.messageDispatcher = messageDispatcher;
        }

        public void SetupCommunicationWith(TcpClient tcpClient)
        {
            Node client = clientNodeFactory.Create(logger, tcpClient);

            clientNodes.Add(client);
            messageDispatcher.Add(client);

            messageDispatcher.SetUpCommunicationWith(client);
        }

        public void CloseConnectionToAllClients()
        {
            foreach (Node node in clientNodes)
            {
                logger.Write<Communicator>("Closing connection to client.");
                node.Disconnect();
            }
        }
    }
}