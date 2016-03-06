using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Logging;
using Messaging;
using SimpleClient;

namespace SimpleClient
{
    public class Communicator : ICommunicator
    {
        private readonly Logger logger;
        private readonly IClientNodeFactory clientNodeFactory;
        private readonly MessageDispatcher messageDispatcher;

        private readonly List<Node> clients = new List<Node>();

        public Communicator(Logger logger, IClientNodeFactory clientNodeFactory, MessageDispatcher messageDispatcher)
        {
            this.logger = logger;
            this.clientNodeFactory = clientNodeFactory;
            this.messageDispatcher = messageDispatcher;
            messageDispatcher.OnDisconnectFromClient += clientDisconnected;
        }

        public void SetupCommunicationWith(TcpClient tcpClient)
        {
            Node client = clientNodeFactory.Create(logger, tcpClient);

            clients.Add(client);
            messageDispatcher.SetUpCommunicationWith(client);
        }

        public void CloseConnectionToAllClients()
        {
            foreach (Node client in clients)
            {
                logger.Write<Communicator>($"Closing connection to client ${client}.");
                client.Disconnect();
            }
        }

        private void clientDisconnected(Node client)
        {
            clients.Remove(client);
            logger.Write<Communicator>($"Client {client} disconnected.");
        }
    }
}