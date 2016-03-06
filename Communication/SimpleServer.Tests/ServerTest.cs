using System.Net.Sockets;
using Logging;
using Messaging;
using Moq;
using SimpleClient;
using TestTools;
using Xunit;

namespace SimpleServer.Tests
{
    public class ServerTest
    {
        [Fact(Skip = "too complex")]
        public void should_disconnect_from_client_when_client_sends_stop_message()
        {
            // Given
            Logger logger = LogSetup.CreateLogger();
            var connectionListener = new Mock<IConnectionListener>();

            var clientNodeFactory = new Mock<IClientNodeFactory>();
            var clientNode = new Mock<ClientNode>();
            clientNodeFactory.Setup(f => f.Create(It.IsAny<Logger>(), It.IsAny<TcpClient>())).Returns(clientNode.Object);

            Server server = new Server(logger, connectionListener.Object, clientNodeFactory.Object);
            server.ListenForConnectionsInANewThread(12345);

            var tcpClientMock = new Mock<TcpClient>();
            connectionListener.Raise((x) => x.OnClientConnected += null, tcpClientMock.Object);

            // Vent på signal.


            // When

        }
    }
}