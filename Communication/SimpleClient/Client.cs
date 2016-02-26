using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Logging;
using Messaging;

namespace SimpleClient
{
    public class Client
    {
        public event Action OnConnected;
        public event Action<String> OnMessageReceived
        {
            add { _messageListener.OnMessageReceived += value; }
            remove { _messageListener.OnMessageReceived -= value; }
        }

        private TcpClient _tcpClient;
        private readonly MessageListener _messageListener;
        private Node _server;
        private readonly Logger _logger;

        public Client(Logger logger)
        {
            _logger = logger;
            _messageListener = new MessageListener(_logger, "Client " + Guid.NewGuid().ToString().Substring(1, 5));
        }

        public void Connect(string host, int port)
        {
            _tcpClient = new TcpClient();
            _logger.Write<Client>("Connecting...");
            _tcpClient.Connect(host, port);
            _logger.Write<Client>("Connected");
            startListeningForMessagesInANewThread();
            OnConnected?.Invoke();
        }

        private void startListeningForMessagesInANewThread()
        {
            _server = new ClientNode(_logger, _tcpClient);
            _logger.Write<Client>("Starting listening for messages in a new thread...");
            Task listenForMessagesTask = new Task(() =>
            {
                _messageListener.ListenForMessages(_server);
            });
            listenForMessagesTask.Start();
            _logger.Write<Client>("Starting listening for messages in a new thread... started");
        }

        public void SendMessage(string msg)
        {
            _server.SendMessage(msg);
        }

        public void Disconnect()
        {
            _tcpClient.Close();
        }
    }
}