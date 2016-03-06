using System.Collections.Generic;
using Messaging;

namespace SimpleClient
{
    public interface IBroadcaster
    {
        void BroadcastMessageToAllClientsExceptSource(List<Node> clients, Node source, string msg);
    }
}