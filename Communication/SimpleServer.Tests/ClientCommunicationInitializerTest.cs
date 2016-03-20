using Logging;
using Messaging;
using Moq;
using TestTools;
using Xunit;

namespace SimpleServer.Tests
{
    public class ClientCommunicationInitializerTest
    {
        [Fact]
        public void should_close_connection_to_client_when_client_sends_stop_message()
        {
            // Given
            Logger logger = LogSetup.CreateLogger();

            var messageListenerFactory = new Mock<IMessageListenerFactory>();
            var messageListener = new Mock<IMessageListener>();
            messageListenerFactory.Setup(m => m.Create(It.IsAny<Logger>())).Returns(messageListener.Object);

            var broadcaster = new Mock<IMessageDispatcher>();
            var taskRunner = new Mock<TaskRunner>();
            var communicationInitializer = new ClientCommunicationInitializer(logger, messageListenerFactory.Object, broadcaster.Object, taskRunner.Object);

            var client = new Mock<Node>();

            communicationInitializer.SetUpCommunicationWith(client.Object);

            // When
            messageListener.Raise(m => m.DoneListeningForMessages += null, client.Object);

            // Then
            client.Verify(c => c.Disconnect());
        }
    }
}