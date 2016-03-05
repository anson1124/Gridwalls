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

        private readonly ConnectionListener connectionListener;
        private readonly List<ClientNode> clientNodes = new List<ClientNode>();
        private readonly Logger logger;

        public Server(Logger logger)
        {
            this.logger = logger;
            connectionListener = new ConnectionListener(this.logger);
        }

        public void ListenForConnectionsInANewThread(int port)
        {
            connectionListener.ListenForConnectionsInANewThread(port);
            connectionListener.OnClientConnected += setupCommunicationWith;
        }

        private void setupCommunicationWith(TcpClient tcpClient)
        {
            ClientNode clientNode = createClientNode(tcpClient);
            setupMessageListener(clientNode);
            OnClientConnected?.Invoke();
        }

        private ClientNode createClientNode(TcpClient tcpClient)
        {
            ClientNode client = new ClientNode(logger, tcpClient);
            clientNodes.Add(client);
            return client;
        }

        private void setupMessageListener(ClientNode client)
        {
            var messageListener = new MessageListener(logger, "Server-client " + Guid.NewGuid().ToString().Substring(0, 5));
            messageListener.OnMessageReceived += (msg) => broadcastMessageToAllOtherClients(client, msg);
            Task.Run(() => { messageListener.ListenForMessages(client); });
        }

        private void broadcastMessageToAllOtherClients(ClientNode client, string msg)
        {
            var theClientThatTheMessageCameFrom = new List<ClientNode>();
            theClientThatTheMessageCameFrom.Add(client);

            foreach (var clientNode in clientNodes.Except(theClientThatTheMessageCameFrom))
            {
                logger.Write<Server>($"Broadcasting message to client: {msg}");
                clientNode.SendMessage(msg);
            }
        }
        
        public void Shutdown()
        {
            logger.Write<Server>("Shutdown called.");
            connectionListener.StopListening();
            foreach (ClientNode node in clientNodes)
            {
                logger.Write<Server>("Closing connection to client...");
                node.Close();
            }
        }

    }
}
