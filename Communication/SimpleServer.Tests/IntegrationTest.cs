using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using SimpleServer;
using TestTools;
using Xunit;

namespace SimpleServer.Tests
{
    public class IntegrationTest : IDisposable
    {
        private readonly Logger logger;

        private const string Localhost = "127.0.0.1";

        private const int DefaultWaitTIme = 3000;

        public IntegrationTest()
        {
            logger = LogSetup.CreateLogger();
        }

        public void Dispose()
        {
            LogSetup.DisposeLogger(logger);
        }

        [Fact]
        public void Should_distribute_message_to_other_connected_client()
        {
            // Given
            int port = new PortSetup(logger).GetNextPort();
            Server server = Bootstrapper.CreateServer();
            server.ListenForConnectionsInANewThread(port);

            logger.Write<IntegrationTest>("Connecting client 1");
            var client1 = Bootstrapper.CreateClient(logger);
            AutoResetEvent client1Connected = new AutoResetEvent(false);
            server.OnClientConnected += () => client1Connected.Set();
            client1.Connect(Localhost, port);
            client1Connected.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 1 never connected");

            logger.Write<IntegrationTest>("Connecting client 2");
            var client2 = Bootstrapper.CreateClient(logger);
            AutoResetEvent client2Connected = new AutoResetEvent(false);
            server.OnClientConnected += () => client2Connected.Set();
            client2.Connect(Localhost, port);
            client2Connected.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 2 never connected");

            var client2MessageReceived = new AutoResetEvent(false);
            var msgFromNetwork = "";
            client2.OnMessageReceived += (msg) =>
            {
                client2MessageReceived.Set();
                msgFromNetwork = msg;
            };

            // When
            logger.Write<IntegrationTest>("Sending message.");
            client1.SendMessage("Hey there");
            client2MessageReceived.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Never received message from client");

            // Then
            Assert.Equal("Hey there", msgFromNetwork);

            // Finally
            client2.Disconnect();
            client1.Disconnect();
            server.Shutdown();
            logger.Write<IntegrationTest>("Integration test done.");
        }


        [Fact]
        public void Should_distribute_message_to_two_other_connected_client()
        {
            // Given
            int port = new PortSetup(logger).GetNextPort();
            Server server = Bootstrapper.CreateServer();
            server.ListenForConnectionsInANewThread(port);

            logger.Write<IntegrationTest>("Connecting client 1");
            var client1 = Bootstrapper.CreateClient(logger);
            AutoResetEvent client1Connected = new AutoResetEvent(false);
            server.OnClientConnected += () => client1Connected.Set();
            client1.Connect(Localhost, port);
            client1Connected.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 1 never connected");

            logger.Write<IntegrationTest>("Connecting client 2");
            var client2 = Bootstrapper.CreateClient(logger);
            AutoResetEvent client2Connected = new AutoResetEvent(false);
            server.OnClientConnected += () => client2Connected.Set();
            client2.Connect(Localhost, port);
            client2Connected.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 2 never connected");

            logger.Write<IntegrationTest>("Connecting client 3");
            var client3 = Bootstrapper.CreateClient(logger);
            AutoResetEvent client3Connected = new AutoResetEvent(false);
            server.OnClientConnected += () => client3Connected.Set();
            client3.Connect(Localhost, port);
            client3Connected.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 3 never connected");

            logger.Write<IntegrationTest>("Connecting client 2");
            var client4 = Bootstrapper.CreateClient(logger);
            AutoResetEvent client4Connected = new AutoResetEvent(false);
            server.OnClientConnected += () => client4Connected.Set();
            client4.Connect(Localhost, port);
            client4Connected.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 4 never connected");

            var client1MessageReceived = new AutoResetEvent(false);
            client1.OnMessageReceived += (msg) =>
            {
                client1MessageReceived.Set();
                Assert.Equal("Hey there", msg);
            };

            var client3MessageReceived = new AutoResetEvent(false);
            client3.OnMessageReceived += (msg) =>
            {
                client3MessageReceived.Set();
                Assert.Equal("Hey there", msg);
            };

            var client4MessageReceived = new AutoResetEvent(false);
            client4.OnMessageReceived += (msg) =>
            {
                client4MessageReceived.Set();
                Assert.Equal("Hey there", msg);
            };

            // When
            logger.Write<IntegrationTest>("Sending message from one of the clients.");
            client2.SendMessage("Hey there");

            // Then
            client1MessageReceived.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Never received message from client 1");
            client3MessageReceived.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Never received message from client 3");
            client4MessageReceived.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Never received message from client 4");

            // Finally
            client1.Disconnect();
            client2.Disconnect();
            client3.Disconnect();
            client4.Disconnect();
            server.Shutdown();
            logger.Write<IntegrationTest>("Integration test done.");
        }

        [Fact]
        public void client_should_be_notified_when_disconnected_from_server()
        {
            // Given
            int port = new PortSetup(logger).GetNextPort();
            Server server = Bootstrapper.CreateServer();
            server.ListenForConnectionsInANewThread(port);

            logger.Write<IntegrationTest>("Connecting client 1");
            var client1 = Bootstrapper.CreateClient(logger);
            bool disconnected = false;
            client1.OnDisconnected += () =>
            {
                disconnected = true;
            };
            AutoResetEvent client1Connected = new AutoResetEvent(false);
            server.OnClientConnected += () => client1Connected.Set();
            client1.Connect(Localhost, port);
            client1Connected.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 1 never connected");

            // When
            server.Shutdown();
            Thread.Sleep(300);

            // Then
            Assert.True(disconnected);
        }


        [Fact]
        public void Should_distribute_message_only_to_clients_that_have_subscribed_to_same_channel()
        {
            // Given
            int port = new PortSetup(logger).GetNextPort();
            Logger serverLogger = LogSetup.CreateLogger("Server_");
            serverLogger.MinimumInfoLevelBeforeWrite = InfoLevel.Trace;
            logger.MinimumInfoLevelBeforeWrite = InfoLevel.Trace;
            Server server = Bootstrapper.CreateServer(serverLogger);
            server.ListenForConnectionsInANewThread(port);

            logger.Write<IntegrationTest>("Connecting client 1");
            var client1 = Bootstrapper.CreateClient(logger);
            client1.Connect(Localhost, port);
            var client1SubscribedEvent = new AutoResetEvent(false);
            client1.OnSubscribed += (theEvent) =>
            {
                logger.Write<IntegrationTest>("Client 1 received subscribeevent: " + theEvent);
                client1SubscribedEvent.Set();
            };
            client1.SubscribeTo("SkeletonEvent");

            logger.Write<IntegrationTest>("Connecting client 2");
            var client2 = Bootstrapper.CreateClient(logger);
            client2.Connect(Localhost, port);
            var client2SubscribedEvent = new AutoResetEvent(false);
            client2.OnSubscribed += (theEvent) =>
            {
                logger.Write<IntegrationTest>("Client 2 received subscribeevent: " + theEvent);
                client2SubscribedEvent.Set();
            };
            client2.SubscribeTo("SkeletonEvent");

            logger.Write<IntegrationTest>("Connecting client 3");
            var client3 = Bootstrapper.CreateClient(logger);
            client3.Connect(Localhost, port);
            var client3SubscribedEvent = new AutoResetEvent(false);
            client3.OnSubscribed += (theEvent) =>
            {
                logger.Write<IntegrationTest>("Client 3 received subscribeevent: " + theEvent);
                client3SubscribedEvent.Set();
            };
            client3.SubscribeTo("SkeletonEvent");

            logger.Write<IntegrationTest>("Connecting client 4");
            var client4 = Bootstrapper.CreateClient(logger);
            client4.Connect(Localhost, port);
            var client4SubscribedEvent = new AutoResetEvent(false);
            client4.OnSubscribed += (theEvent) =>
            {
                logger.Write<IntegrationTest>("Client 4 received subscribeevent: " + theEvent);
                client4SubscribedEvent.Set();
            };
            client4.SubscribeTo("CowEvent");

            logger.Write<IntegrationTest>("Connecting client 5");
            var client5 = Bootstrapper.CreateClient(logger);
            client5.Connect(Localhost, port);
            var client5SubscribedEvent = new AutoResetEvent(false);
            client5.OnSubscribed += (theEvent) =>
            {
                logger.Write<IntegrationTest>("Client 5 received subscribeevent: " + theEvent);
                client5SubscribedEvent.Set();
            };
            client5.SubscribeTo("CowEvent");

            var client2MessageReceived = new AutoResetEvent(false);
            client2.OnMessageReceived += (msg) =>
            {
                logger.Write<IntegrationTest>("Client 2 received message: " + msg);
                client2MessageReceived.Set();
                Assert.Equal("Boo!", msg);
            };

            var client3MessageReceived = new AutoResetEvent(false);
            client3.OnMessageReceived += (msg) =>
            {
                logger.Write<IntegrationTest>("Client 3 received message: " + msg);
                client3MessageReceived.Set();
                Assert.Equal("Boo!", msg);
            };

            var client5MessageReceived = new AutoResetEvent(false);
            client5.OnMessageReceived += (msg) =>
            {
                logger.Write<IntegrationTest>("Client 5 received message: " + msg);
                client5MessageReceived.Set();
                Assert.Equal("Moo!", msg);
            };

            try
            {
                client1SubscribedEvent.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 1 never received SubscribedEvent");
                client2SubscribedEvent.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 2 never received SubscribedEvent");
                client3SubscribedEvent.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 3 never received SubscribedEvent");
                client4SubscribedEvent.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 4 never received SubscribedEvent");
                client5SubscribedEvent.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 5 never received SubscribedEvent");
            }
            catch (ApplicationException e)
            {
                logger.Dispose();
                serverLogger.Dispose();
                throw e;
            }
            

            // When
            logger.Write<IntegrationTest>("Sending message from client 1.");
            client1.SendMessage("[SkeletonEvent] Boo!");
            logger.Write<IntegrationTest>("Sending message from client 4.");
            client4.SendMessage("[CowEvent] Moo!");

            // Then
            try
            {
                client2MessageReceived.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Never received message from client 1");
                client3MessageReceived.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Never received message from client 1");
                client5MessageReceived.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Never received message from client 4");
            }
            catch (ApplicationException e)
            {
                logger.Dispose();
                serverLogger.Dispose();
                throw e;
            }
            
            // Finally
            server.Shutdown();
            logger.Write<IntegrationTest>("Integration test done.");
            serverLogger.Dispose();
        }

        // Should send to clients that have not subscribed to any tag

    }
}
