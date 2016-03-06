using Logging;
using Messaging;

namespace SimpleServer
{
    public class MessageListenerFactory : IMessageListenerFactory
    {
        public IMessageListener Create(Logger logger, string clientName)
        {
            return new MessageListener(logger, clientName);
        }
    }
}