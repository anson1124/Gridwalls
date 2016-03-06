using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;
using Logging;
using Messaging;

namespace SimpleServer
{
    public class Client
    {
        public event Action OnConnected;
        public event Action<String> OnMessageReceived
        {
            add { messageListener.OnMessageReceived += value; }
            remove { messageListener.OnMessageReceived -= value; }
        }

        public event Action OnDisconnected;

        private readonly IClientNodeFactory clientNodeFactory;

        private TcpClient tcpClient;
        private readonly MessageListener messageListener;
        private Node server;
        private readonly Logger logger;

        public Client(Logger logger, IClientNodeFactory clientNodeFactory)
        {
            this.logger = logger;
            this.clientNodeFactory = clientNodeFactory;
            messageListener = new MessageListener(this.logger);
        }

        public void Connect(string host, int port)
        {
            messageListener.DoneListeningForMessages += disconnectedFromServer;

            tcpClient = new TcpClient();
            logger.Write<Client>("Connecting...");
            tcpClient.Connect(host, port);
            logger.Write<Client>("Connected");
            startListeningForMessagesInANewThread();
            OnConnected?.Invoke();
        }

        private void disconnectedFromServer(Node server)
        {
            logger.Write<Client>("Disconnected from server. Closing tcp client and notifying listeners.");
            tcpClient.Close();
            OnDisconnected?.Invoke();
        }

        private void startListeningForMessagesInANewThread()
        {
            server = clientNodeFactory.Create(logger, tcpClient);
            logger.Write<Client>("Starting listening for messages in a new thread...");
            Task listenForMessagesTask = new Task(() =>
            {
                messageListener.ListenForMessages(server);
            });
            listenForMessagesTask.Start();
            logger.Write<Client>("Starting listening for messages in a new thread... started");
        }

        public void SendMessage(string msg)
        {
            server.SendMessage(msg);
        }

        public void Disconnect()
        {
            tcpClient.Close();
        }
    }
}