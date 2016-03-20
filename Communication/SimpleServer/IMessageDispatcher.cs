using System.Collections.Generic;
using Messaging;

namespace SimpleServer
{
    public interface IMessageDispatcher
    {
        void OnMessageReceived(Node source, string msg);
        void AddClient(Node client);
        void RemoveClient(Node client);
    }
}