using System.Collections.Generic;
using Messaging;

namespace SimpleServer
{
    public interface IBroadcaster
    {
        void BroadcastMessageToAllClientsExceptSource(List<Node> clients, Node source, string msg);
    }
}