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
        public event Action<string> OnMessageReceived;

        public event Action<string> OnSubscribed;
        public event Action OnDisconnected;

        private readonly IClientNodeFactory clientNodeFactory;

        private TcpClient tcpClient;
        private MessageListener messageListener;
        private Node serverNode;
        private readonly Logger logger;

        public Client(Logger logger, IClientNodeFactory clientNodeFactory)
        {
            this.logger = logger;
            this.clientNodeFactory = clientNodeFactory;
        }

        public void Connect(string host, int port)
        {
            messageListener = new MessageListener(this.logger);
            messageListener.DoneListeningForMessages += disconnectedFromServer;

            tcpClient = new TcpClient();
            //tcpClient.NoDelay = true;
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
            messageListener.OnMessageReceived += onMessageReceived;

            serverNode = clientNodeFactory.Create(logger, tcpClient);
            logger.Write<Client>("Starting listening for messages in a new thread...");
            Task.Factory.StartNew(() =>
            {
                messageListener.ListenForMessages(serverNode);
            });
            logger.Write<Client>("Starting listening for messages in a new thread... started");
        }

        private void onMessageReceived(string msg)
        {
            if (msg.StartsWith(SubscribedEvent.Name))
            {
                SubscribedEvent subscribedEvent = SubscribedEvent.Deserialize(msg);
                logger.Write<Client>("I am now subscribed to " + subscribedEvent.Tag);
                OnSubscribed?.Invoke(subscribedEvent.Tag);
            }
            else
            {
                OnMessageReceived?.Invoke(msg);
            }
        }

        public void SendMessage(string msg)
        {
            serverNode.SendMessage(msg);
        }

        public void Disconnect()
        {
            logger.Write<Client>("Disconnecting from server.");
            tcpClient.Close();
        }

        public void SubscribeTo(string tag)
        {
            logger.Write<Client>($"Sending message to server that I want to subscribe to tag {tag}.");
            serverNode.SendMessage(new SubscribeEvent(tag).Serialize());
        }
    }
}