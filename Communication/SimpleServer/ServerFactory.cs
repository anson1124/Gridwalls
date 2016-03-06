using Logging;
using SimpleServer;

namespace SimpleServer
{
    public class ServerFactory
    {
        public static Server CreateServer(Logger logger)
        {
            var messageDispatcher = new MessageDispatcher(logger, new MessageListenerFactory(), new Broadcaster(logger), new TaskRunner());
            ICommunicator communicator = new Communicator(logger, new ClientNodeFactory(), messageDispatcher);

            return new Server(logger, new ConnectionListener(logger), communicator);
        }
    }
}