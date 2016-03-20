using Logging;
using Messaging;

namespace SimpleServer
{
    public class MessageListenerFactory : IMessageListenerFactory
    {
        public IMessageListener Create(Logger logger)
        {
            return new MessageListener(logger);
        }
    }
}