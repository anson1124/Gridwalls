using System;
using System.Net.Sockets;
using Logging;
using Messaging;
using Moq;
using SimpleServer;
using TestTools;
using Xunit;

namespace SimpleServer.Tests
{
    public class MessageDispatcherTest
    {
        [Fact]
        public void should_close_connection_to_client_when_client_sends_stop_message()
        {
            // Given
            Logger logger = LogSetup.CreateLogger();

            var messageListenerFactory = new Mock<IMessageListenerFactory>();
            var messageListener = new Mock<IMessageListener>();
            messageListenerFactory.Setup(m => m.Create(It.IsAny<Logger>(), It.IsAny<string>())).Returns(messageListener.Object);

            var broadcaster = new Mock<IBroadcaster>();
            var taskRunner = new Mock<TaskRunner>();
            var messageDispatcher = new MessageDispatcher(logger, messageListenerFactory.Object, broadcaster.Object, taskRunner.Object);

            var client = new Mock<Node>();

            messageDispatcher.Add(client.Object);
            messageDispatcher.SetUpCommunicationWith(client.Object);

            // When
            messageListener.Raise(m => m.DoneListeningForMessages += null, client.Object);

            // Then
            client.Verify(c => c.Disconnect());
        }
    }
}