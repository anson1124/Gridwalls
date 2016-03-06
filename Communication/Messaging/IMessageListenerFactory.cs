using Logging;
using Messaging;

namespace Messaging
{
    public interface IMessageListenerFactory
    {
        IMessageListener Create(Logger logger);
    }
}