using System;
using System.Collections.Generic;
using System.Net.Sockets;
using Logging;
using Messaging;
using Moq;
using SimpleClient;
using SimpleServer;
using TestTools;
using Xunit;

namespace SimpleServer.Tests
{
    public class MessageDispatcherTest
    {
        [Fact]
        public void Should_distribute_message_to_all_clients()
        {
            // Given
            Logger logger = LogSetup.CreateLogger();
            var msgDispatcher = new MessageDispatcher(logger);

            var source = new Mock<Node>();
            var client1 = new Mock<Node>();
            var client2 = new Mock<Node>();
            msgDispatcher.AddClient(source.Object);
            msgDispatcher.AddClient(client1.Object);
            msgDispatcher.AddClient(client2.Object);

            // When
            msgDispatcher.OnMessageReceived(source.Object, "Hey!");

            // Then
            client1.Verify(c => c.SendMessage("Hey!"));
            client2.Verify(c => c.SendMessage("Hey!"));
            source.Verify(c => c.SendMessage(It.IsAny<string>()), Times.Never);
        }

        [Fact]
        public void Should_distribute_message_to_all_subscribed_clients()
        {
            // Given
            Logger logger = LogSetup.CreateLogger();
            var msgDispatcher = new MessageDispatcher(logger);

            var source = new Mock<Node>();
            var client1 = new Mock<Node>();
            var client2 = new Mock<Node>();
            var client3 = new Mock<Node>();
            msgDispatcher.AddClient(source.Object);
            msgDispatcher.AddClient(client1.Object);
            msgDispatcher.AddClient(client2.Object);
            msgDispatcher.AddClient(client3.Object);

            msgDispatcher.OnMessageReceived(client1.Object, new SubscribeEvent("CowEvent").Serialize());
            msgDispatcher.OnMessageReceived(client2.Object, new SubscribeEvent("CowEvent").Serialize());

            // When
            msgDispatcher.OnMessageReceived(source.Object, "[CowEvent] Moo!");

            // Then
            client1.Verify(c => c.SendMessage("Moo!"));
            client2.Verify(c => c.SendMessage("Moo!"));
            client3.Verify(c => c.SendMessage(It.IsAny<string>()), Times.Never);
        }
        [Fact]
        public void Should_support_multiple_tags()
        {
            // Given
            Logger logger = LogSetup.CreateLogger(typeof(MessageDispatcher).Name + "_");
            var msgDispatcher = new MessageDispatcher(logger);

            var client1 = new Mock<Node>();
            var client2 = new Mock<Node>();
            var client3 = new Mock<Node>();
            var client4 = new Mock<Node>();
            client1.Setup(c => c.ToString()).Returns("client1");
            client2.Setup(c => c.ToString()).Returns("client2");
            client3.Setup(c => c.ToString()).Returns("client3");
            client4.Setup(c => c.ToString()).Returns("client4");

            msgDispatcher.AddClient(client1.Object);
            msgDispatcher.AddClient(client2.Object);
            msgDispatcher.AddClient(client3.Object);
            msgDispatcher.AddClient(client4.Object);

            msgDispatcher.OnMessageReceived(client1.Object, new SubscribeEvent("CowEvent").Serialize());
            msgDispatcher.OnMessageReceived(client2.Object, new SubscribeEvent("CowEvent").Serialize());
            msgDispatcher.OnMessageReceived(client3.Object, new SubscribeEvent("SheepEvent").Serialize());
            msgDispatcher.OnMessageReceived(client4.Object, new SubscribeEvent("SheepEvent").Serialize());

            client3.ResetCalls();
            client4.ResetCalls();

            // When
            msgDispatcher.OnMessageReceived(client1.Object, "[CowEvent] Moo!");

            // Then
            client2.Verify(c => c.SendMessage("Moo!"));
            client3.Verify(c => c.SendMessage(It.IsAny<string>()), Times.Never);
            client4.Verify(c => c.SendMessage(It.IsAny<string>()), Times.Never);

            client1.ResetCalls();
            client2.ResetCalls();

            // When
            msgDispatcher.OnMessageReceived(client3.Object, "[SheepEvent] Baa!");

            // Then
            client1.Verify(c => c.SendMessage(It.IsAny<string>()), Times.Never);
            client2.Verify(c => c.SendMessage(It.IsAny<string>()), Times.Never);
            client4.Verify(c => c.SendMessage("Baa!"));
        }

        [Fact]
        public void Should_not_send_message_to_subscribed_client_after_removing()
        {
            // Given
            Logger logger = LogSetup.CreateLogger();
            var msgDispatcher = new MessageDispatcher(logger);

            var source = new Mock<Node>();
            var client1 = new Mock<Node>();
            msgDispatcher.AddClient(source.Object);
            msgDispatcher.AddClient(client1.Object);

            msgDispatcher.OnMessageReceived(client1.Object, new SubscribeEvent("CowEvent").Serialize());
            msgDispatcher.RemoveClient(client1.Object);

            // When
            msgDispatcher.OnMessageReceived(source.Object, "[CowEvent] Moo!");

            // Then
            client1.Verify(c => c.SendMessage(It.IsAny<string>()), Times.Never);
        }


        [Fact]
        public void Should_distribute_message_to_all_clients_that_are_not_subscribed_to_anything()
        {
            // Given
            Logger logger = LogSetup.CreateLogger();
            var msgDispatcher = new MessageDispatcher(logger);

            var source = new Mock<Node>();
            var client1 = new Mock<Node>();
            msgDispatcher.AddClient(source.Object);
            msgDispatcher.AddClient(client1.Object);

            // When
            msgDispatcher.OnMessageReceived(source.Object, "Random message");

            // Then
            client1.Verify(c => c.SendMessage("Random message"));
        }
    }
}