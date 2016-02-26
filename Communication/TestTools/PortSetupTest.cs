using System.IO;
using Logging;
using Xunit;

namespace TestTools
{
    public class PortSetupTest
    {
        private readonly Logger _logger = LogSetup.CreateLogger();

        public void Dispose()
        {
            LogSetup.DisposeLogger(_logger);
        }

        public void first_port_should_be_correct()
        {
            int firstPort = new PortSetup(_logger).GetNextPort();
            Assert.Equal(PortSetup.FirstPort, firstPort);
        }

        //[Fact(Skip = "Skipper fordi de sletter fila som holder på data som andre tester trenger.")]
        public void should_store_port_after_first_call()
        {
            int port = new PortSetup(_logger).GetNextPort();
            string[] lines = File.ReadAllLines(PortSetup.Datafile);
            Assert.Equal(PortSetup.FirstPort + "", lines[0]);
        }

        //[Fact(Skip = "Skipper fordi de sletter fila som holder på data som andre tester trenger.")]
        public void next_port_should_be_first_port_plus_one()
        {
            int firstPort = new PortSetup(_logger).GetNextPort();
            int nextPort = new PortSetup(_logger).GetNextPort();

            Assert.Equal(firstPort + 1, nextPort);
        }

    }
}