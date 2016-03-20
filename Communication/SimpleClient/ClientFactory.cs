using Logging;
using SimpleServer;

namespace SimpleClient
{
    public class ClientFactory
    {
        public static Client Create(Logger logger)
        {
            return new Client(logger, new ClientNodeFactory());
        }
    }
}