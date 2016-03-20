using System;
using System.Collections.Generic;
using Logging;
using Messaging;

namespace SimpleServer
{
    public class ClientCommunicationInitializer
    {
        public event Action<Node> OnDisconnectFromClient;

        private readonly Logger logger;
        private readonly IMessageListenerFactory messageListenerFactory;
        private readonly IMessageDispatcher messageDispatcher;
        private readonly TaskRunner taskRunner;


        public ClientCommunicationInitializer(Logger logger, IMessageListenerFactory messageListenerFactory, IMessageDispatcher messageDispatcher, TaskRunner taskRunner)
        {
            this.logger = logger;
            this.messageListenerFactory = messageListenerFactory;
            this.messageDispatcher = messageDispatcher;
            this.taskRunner = taskRunner;
        }

        public void SetUpCommunicationWith(Node client)
        {
            messageDispatcher.AddClient(client);
            IMessageListener messageListener = messageListenerFactory.Create(logger);
            messageListener.OnMessageReceived += (msg) => messageDispatcher.OnMessageReceived(client, msg);
            messageListener.DoneListeningForMessages += onDisconnectFromClient;

            taskRunner.Run(() => messageListener.ListenForMessages(client));
        }

        private void onDisconnectFromClient(Node client)
        {
            messageDispatcher.RemoveClient(client);
            client.Disconnect();
            OnDisconnectFromClient?.Invoke(client);
        }
    }
}