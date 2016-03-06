using System.Collections.Generic;
using System.Linq;
using Logging;
using Messaging;

namespace SimpleClient
{
    public class Broadcaster : IBroadcaster
    {
        private readonly Logger logger;

        public Broadcaster(Logger logger)
        {
            this.logger = logger;
        }

        public void BroadcastMessageToAllClientsExceptSource(List<Node> clients, Node source, string msg)
        {
            var theClientThatTheMessageCameFrom = new List<Node> {source};
            var clientsToBroadcastTo = clients.Except(theClientThatTheMessageCameFrom);

            logger.Write<Broadcaster>($"Broadcasting message from {source} to {clientsToBroadcastTo.Count()} clients: {msg}");
            foreach (var clientNode in clientsToBroadcastTo)
            {
                clientNode.SendMessage(msg);
            }
        }
    }
}