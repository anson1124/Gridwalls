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
            add { messageListener.OnMessageReceived += value; }
            remove { messageListener.OnMessageReceived -= value; }
        }

        public event Action OnDisconnected;

        private TcpClient tcpClient;
        private readonly MessageListener messageListener;
        private Node server;
        private readonly Logger logger;

        public Client(Logger logger)
        {
            this.logger = logger;
            messageListener = new MessageListener(this.logger, "Client " + Guid.NewGuid().ToString().Substring(1, 5));
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

        private void disconnectedFromServer()
        {
            logger.Write<Client>("Disconnected from server. Closing tcp client and notifying listeners.");
            tcpClient.Close();
            OnDisconnected?.Invoke();
        }

        private void startListeningForMessagesInANewThread()
        {
            server = new ClientNode(logger, tcpClient);
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