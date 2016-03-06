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

namespace SimpleServer
{
    public class Server
    {
        public event Action OnClientConnected;

        private readonly IConnectionListener connectionListener;
        private readonly IClientNodeFactory clientNodeFactory;
        private readonly List<Node> clientNodes = new List<Node>();
        private readonly Logger logger;

        public Server(Logger logger, IConnectionListener connectionListener, IClientNodeFactory clientNodeFactory)
        {
            this.logger = logger;
            this.connectionListener = connectionListener;
            this.clientNodeFactory = clientNodeFactory;
        }

        public void ListenForConnectionsInANewThread(int port)
        {
            connectionListener.ListenForConnectionsInANewThread(port);
            connectionListener.OnClientConnected += setupCommunicationWith;
        }

        private void setupCommunicationWith(TcpClient tcpClient)
        {
            Node clientNode = createClientNode(tcpClient);
            setupMessageListener(clientNode);
            OnClientConnected?.Invoke();
        }

        private Node createClientNode(TcpClient tcpClient)
        {
            Node client = clientNodeFactory.Create(logger, tcpClient);
            clientNodes.Add(client);
            return client;
        }

        private void setupMessageListener(Node client)
        {
            string clientName = "Server-client " + Guid.NewGuid().ToString().Substring(0, 5);
            var messageListener = new MessageListener(logger, clientName);
            messageListener.OnMessageReceived += (msg) => broadcastMessageToAllOtherClients(client, msg);
            messageListener.DoneListeningForMessages += disconnectFromClient;
            Task.Run(() => { messageListener.ListenForMessages(client); });
        }

        private void broadcastMessageToAllOtherClients(Node client, string msg)
        {
            var theClientThatTheMessageCameFrom = new List<Node> {client};

            foreach (var clientNode in clientNodes.Except(theClientThatTheMessageCameFrom))
            {
                logger.Write<Server>($"Broadcasting message to client: {msg}");
                clientNode.SendMessage(msg);
            }
        }

        private void disconnectFromClient(Node clientNode)
        {
            //clientNode.Close();
        }

        public void Shutdown()
        {
            logger.Write<Server>("Shutdown called.");
            connectionListener.StopListening();
            foreach (Node node in clientNodes)
            {
                logger.Write<Server>("Closing connection to client...");
                node.Close();
            }
        }

    }
}
