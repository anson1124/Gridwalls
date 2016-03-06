using Logging;
using TestTools;

namespace SimpleServer.Tests
{
    public class Bootstrapper
    {
        static internal Server CreateServer()
        {
            Logger logger = LogSetup.CreateLogger();
            var messageDispatcher = new MessageDispatcher(logger, new MessageListenerFactory(), new Broadcaster(logger), new TaskRunner());
            ICommunicator communicator = new Communicator(logger, new ClientNodeFactory(), messageDispatcher);

            return new Server(logger, new ConnectionListener(logger), communicator);
        }
    }
}