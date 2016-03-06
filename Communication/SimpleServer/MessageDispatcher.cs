using System;
using System.Collections.Generic;
using Logging;
using Messaging;

namespace SimpleServer
{
    public class MessageDispatcher
    {
        private readonly Logger logger;
        private readonly IMessageListenerFactory messageListenerFactory;
        private readonly IBroadcaster broadcaster;
        private readonly TaskRunner taskRunner;

        private readonly List<Node> clientNodes = new List<Node>();

        public MessageDispatcher(Logger logger, IMessageListenerFactory messageListenerFactory, IBroadcaster broadcaster, TaskRunner taskRunner)
        {
            this.logger = logger;
            this.messageListenerFactory = messageListenerFactory;
            this.broadcaster = broadcaster;
            this.taskRunner = taskRunner;
        }

        public void Add(Node client)
        {
            clientNodes.Add(client);
        }

        public void SetUpCommunicationWith(Node client)
        {
            IMessageListener messageListener = messageListenerFactory.Create(logger, createClientName());
            messageListener.OnMessageReceived += (msg) => broadcaster.BroadcastMessageToAllClientsExceptSource(new List<Node>(clientNodes), client, msg);
            messageListener.DoneListeningForMessages += onDisconnectFromClient;

            taskRunner.Run(() => messageListener.ListenForMessages(client));
        }

        private string createClientName()
        {
            return "Server-client " + Guid.NewGuid().ToString().Substring(0, 5);
        }

        private void onDisconnectFromClient(Node client)
        {
            client.Disconnect();
        }
    }
}