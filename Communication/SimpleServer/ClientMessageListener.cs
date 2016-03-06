using System;
using System.Collections.Generic;
using Logging;
using Messaging;

namespace SimpleServer
{
    public class ClientMessageListener
    {
        private readonly Logger logger;
        private readonly IMessageListenerFactory messageListenerFactory;
        private readonly Broadcaster broadcaster;
        private readonly TaskRunner taskRunner;

        internal void ListenForMessages(Node client, List<Node> clientNodes)
        {
            string clientName = "Server-client " + Guid.NewGuid().ToString().Substring(0, 5);
            var messageListener = messageListenerFactory.Create(logger, clientName);
            messageListener.OnMessageReceived += (msg) => broadcaster.BroadcastMessageToAllClientsExceptSource(clientNodes, client, msg);
            messageListener.DoneListeningForMessages += onDisconnectFromClient;

            taskRunner.Run(() => messageListener.ListenForMessages(client));
        }

        private void onDisconnectFromClient(Node clientNode)
        {
            //clientNode.Close();
        } 
    }
}