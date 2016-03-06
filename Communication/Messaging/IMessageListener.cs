using System;
using Messaging;

namespace Messaging
{
    public interface IMessageListener
    {
        event Action<string> OnMessageReceived;

        event Action<Node> DoneListeningForMessages;

        void ListenForMessages(Node node);
    }
}