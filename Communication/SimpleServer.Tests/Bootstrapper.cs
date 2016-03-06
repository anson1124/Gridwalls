using Logging;
using TestTools;

namespace SimpleServer.Tests
{
    public class Bootstrapper
    {
        static internal Server CreateServer()
        {
            Logger logger = LogSetup.CreateLogger();
            ICommunicator communicator = new Communicator(logger, new ClientNodeFactory(), new Broadcaster(logger), new TaskRunner());
            return new Server(logger, new ConnectionListener(logger), communicator);
        }
    }
}