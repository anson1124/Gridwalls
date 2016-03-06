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
        private readonly IMessageListenerFactory messageListenerFactory;
        private readonly Broadcaster broadcaster;
        private readonly TaskRunner taskRunner;
        private readonly ClientMessageListener clientMessageListener;

        private readonly List<Node> clientNodes = new List<Node>();

        public Communicator(
            Logger logger,
            IClientNodeFactory clientNodeFactory,
            Broadcaster broadcaster,
            TaskRunner taskRunner,
            IMessageListenerFactory messageListenerFactory,
            ClientMessageListener clientMessageListener)
        {
            this.logger = logger;
            this.clientNodeFactory = clientNodeFactory;
            this.broadcaster = broadcaster;
            this.taskRunner = taskRunner;
            this.messageListenerFactory = messageListenerFactory;
            this.clientMessageListener = clientMessageListener;
        }

        public void SetupCommunicationWith(TcpClient tcpClient)
        {
            Node client = createClientNode(tcpClient);
            clientMessageListener.ListenForMessages(client, clientNodes);
        }

        private Node createClientNode(TcpClient tcpClient)
        {
            Node client = clientNodeFactory.Create(logger, tcpClient);
            clientNodes.Add(client);
            return client;
        }

        public void CloseConnectionToAllClients()
        {
            foreach (Node node in clientNodes)
            {
                logger.Write<Server>("Closing connection to client...");
                node.Close();
            }
        }
    }
}