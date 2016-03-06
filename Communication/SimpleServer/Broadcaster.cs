using System.Collections.Generic;
using System.Linq;
using Logging;
using Messaging;

namespace SimpleServer
{
    public class Broadcaster
    {
        private readonly Logger logger;

        public Broadcaster(Logger logger)
        {
            this.logger = logger;
        }

        public void BroadcastMessageToAllClientsExceptSource(List<Node> clients, Node source, string msg)
        {
            var theClientThatTheMessageCameFrom = new List<Node> {source};

            foreach (var clientNode in clients.Except(theClientThatTheMessageCameFrom))
            {
                logger.Write<Broadcaster>($"Broadcasting message to client: {msg}");
                clientNode.SendMessage(msg);
            }
        }
    }
}