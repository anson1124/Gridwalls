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

        private readonly ConnectionListener _connectionListener;
        private readonly List<ClientNode> _clientNodes = new List<ClientNode>();
        private readonly Logger _logger;

        public Server(Logger logger)
        {
            _logger = logger;
            _connectionListener = new ConnectionListener(_logger);
        }

        public void ListenForConnectionsInANewThread(int port)
        {
            _connectionListener.ListenForConnectionsInANewThread(port);
            _connectionListener.OnClientConnected += setupCommunicationWith;
        }

        private void setupCommunicationWith(TcpClient tcpClient)
        {
            var clientNode = createClientNode(tcpClient);
            setupMessageListener(clientNode);
            OnClientConnected?.Invoke();
        }

        private ClientNode createClientNode(TcpClient tcpClient)
        {
            ClientNode client = new ClientNode(_logger, tcpClient);
            _clientNodes.Add(client);
            return client;
        }

        private void setupMessageListener(ClientNode client)
        {
            var messageListener = new MessageListener(_logger, "Server-client " + Guid.NewGuid().ToString().Substring(0, 5));
            messageListener.OnMessageReceived += (msg) => broadcastMessageToAllOtherClients(client, msg);
            Task.Run(() => { messageListener.ListenForMessages(client); });
        }

        private void broadcastMessageToAllOtherClients(ClientNode client, string msg)
        {
            var theClientThatTheMessageCameFrom = new List<ClientNode>();
            theClientThatTheMessageCameFrom.Add(client);

            foreach (var clientNode in _clientNodes.Except(theClientThatTheMessageCameFrom))
            {
                _logger.Write<Server>($"Broadcasting message to client: {msg}");
                clientNode.SendMessage(msg);
            }
        }
        
        public void Shutdown()
        {
            _connectionListener.StopListening();
        }

    }
}
