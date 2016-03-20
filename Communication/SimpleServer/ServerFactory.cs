using Logging;
using SimpleClient;
using SimpleServer;

namespace SimpleServer
{
    public class ServerFactory
    {
        public static Server CreateServer(Logger logger)
        {
            var messageDispatcher = new ClientCommunicationInitializer(logger, new MessageListenerFactory(), new MessageDispatcher(logger), new TaskRunner());
            ICommunicator communicator = new AllClientsCommunicator(logger, new ClientNodeFactory(), messageDispatcher);

            return new Server(logger, new ConnectionListener(logger), communicator);
        }
    }
}