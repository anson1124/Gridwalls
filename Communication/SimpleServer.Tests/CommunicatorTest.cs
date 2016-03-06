using System.Net.Sockets;
using Logging;
using Messaging;
using Moq;
using SimpleServer;
using TestTools;
using Xunit;

namespace SimpleServer.Tests
{
    public class CommunicatorTest
    {
        //[Fact(Skip = "too complex")]
        public void should_close_connection_to_client_when_client_sends_stop_message()
        {
            // Given
            Logger logger = LogSetup.CreateLogger();

            var clientNodeFactory = new Mock<IClientNodeFactory>();
            var broadcaster = new Mock<Broadcaster>();
            var taskrunner = new Mock<TaskRunner>();
            var messageListenerFactory = new Mock<IMessageListenerFactory>();

            var communicator = new Communicator(
                logger, clientNodeFactory.Object, broadcaster.Object, taskrunner.Object, messageListenerFactory.Object);
            var tcpClient = new Mock<TcpClient>();

            //var clientNode = new Mock<ClientNode>();
            //clientNodeFactory.Setup(f => f.Create(It.IsAny<Logger>(), It.IsAny<TcpClient>())).Returns(clientNode.Object);
            communicator.SetupCommunicationWith(tcpClient.Object);

            // When


        }
    }
}