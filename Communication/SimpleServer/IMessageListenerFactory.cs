using Logging;
using Messaging;

namespace SimpleServer
{
    public interface IMessageListenerFactory
    {
        MessageListener Create(Logger logger, string clientName);
    }
}