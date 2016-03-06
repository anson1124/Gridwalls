using System;
using System.Collections.Generic;
using Logging;
using Messaging;

namespace SimpleServer
{
    public class MessageDispatcher
    {
        public event Action<Node> OnDisconnectFromClient;

        private readonly Logger logger;
        private readonly IMessageListenerFactory messageListenerFactory;
        private readonly IBroadcaster broadcaster;
        private readonly TaskRunner taskRunner;

        private readonly List<Node> clients = new List<Node>();

        public MessageDispatcher(Logger logger, IMessageListenerFactory messageListenerFactory, IBroadcaster broadcaster, TaskRunner taskRunner)
        {
            this.logger = logger;
            this.messageListenerFactory = messageListenerFactory;
            this.broadcaster = broadcaster;
            this.taskRunner = taskRunner;
        }

        public void SetUpCommunicationWith(Node client)
        {
            clients.Add(client);

            IMessageListener messageListener = messageListenerFactory.Create(logger);
            messageListener.OnMessageReceived += (msg) => broadcaster.BroadcastMessageToAllClientsExceptSource(new List<Node>(clients), client, msg);
            messageListener.DoneListeningForMessages += onDisconnectFromClient;

            taskRunner.Run(() => messageListener.ListenForMessages(client));
        }

        private void onDisconnectFromClient(Node client)
        {
            client.Disconnect();
            clients.Remove(client);
            OnDisconnectFromClient?.Invoke(client);
        }
    }
}