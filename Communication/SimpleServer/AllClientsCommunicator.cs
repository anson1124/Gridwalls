using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Sockets;
using System.Threading.Tasks;
using Logging;
using Messaging;
using SimpleClient;

namespace SimpleServer
{
    public class AllClientsCommunicator : ICommunicator
    {
        private readonly Logger logger;
        private readonly IClientNodeFactory clientNodeFactory;
        private readonly ClientCommunicationInitializer clientCommunicationInitializer;

        private readonly List<Node> clients = new List<Node>();

        public AllClientsCommunicator(Logger logger, IClientNodeFactory clientNodeFactory, ClientCommunicationInitializer clientCommunicationInitializer)
        {
            this.logger = logger;
            this.clientNodeFactory = clientNodeFactory;
            this.clientCommunicationInitializer = clientCommunicationInitializer;
            clientCommunicationInitializer.OnDisconnectFromClient += clientDisconnected;
        }

        private void clientDisconnected(Node client)
        {
            clients.Remove(client);
            logger.Write<AllClientsCommunicator>($"Client {client} disconnected.");
        }

        public void SetupCommunicationWith(TcpClient tcpClient)
        {
            //tcpClient.NoDelay = true;
            Node client = clientNodeFactory.Create(logger, tcpClient);

            clients.Add(client);
            clientCommunicationInitializer.SetUpCommunicationWith(client);
        }

        public void CloseConnectionToAllClients()
        {
            foreach (Node client in clients)
            {
                logger.Write<AllClientsCommunicator>($"Closing connection to client {client}.");
                client.Disconnect();
            }
        }
    }
}