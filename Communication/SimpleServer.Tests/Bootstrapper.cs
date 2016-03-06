using Logging;
using TestTools;

namespace SimpleServer.Tests
{
    public class Bootstrapper
    {
        private static readonly IClientNodeFactory clientNodeFactory;

        static Bootstrapper()
        {
            clientNodeFactory = new ClientNodeFactory();
        }

        static internal Server CreateServer()
        {
            Logger logger = LogSetup.CreateLogger();
            return ServerFactory.CreateServer(logger);
        }

        static internal Client CreateClient(Logger logger)
        {
            return new Client(logger, clientNodeFactory);
        }
    }
}