using Logging;
using Messaging;

namespace SimpleClient
{
    public class MessageListenerFactory : IMessageListenerFactory
    {
        public IMessageListener Create(Logger logger)
        {
            return new MessageListener(logger);
        }
    }
}