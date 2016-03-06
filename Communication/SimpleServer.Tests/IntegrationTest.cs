using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using SimpleServer;
using SimpleTcpServer;
using TestTools;
using Xunit;

namespace SimpleServer.Tests
{
    public class IntegrationTest : IDisposable
    {
        private readonly Logger logger;

        private const string Localhost = "127.0.0.1";

        private const int DefaultWaitTIme = 800;

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
            var client1 = new Client(logger);
            AutoResetEvent client1Connected = new AutoResetEvent(false);
            server.OnClientConnected += () => client1Connected.Set();
            client1.Connect(Localhost, port);
            client1Connected.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 1 never connected");

            logger.Write<IntegrationTest>("Connecting client 2");
            var client2 = new Client(logger);
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
            var client1 = new Client(logger);
            AutoResetEvent client1Connected = new AutoResetEvent(false);
            server.OnClientConnected += () => client1Connected.Set();
            client1.Connect(Localhost, port);
            client1Connected.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 1 never connected");

            logger.Write<IntegrationTest>("Connecting client 2");
            var client2 = new Client(logger);
            AutoResetEvent client2Connected = new AutoResetEvent(false);
            server.OnClientConnected += () => client2Connected.Set();
            client2.Connect(Localhost, port);
            client2Connected.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 2 never connected");

            logger.Write<IntegrationTest>("Connecting client 3");
            var client3 = new Client(logger);
            AutoResetEvent client3Connected = new AutoResetEvent(false);
            server.OnClientConnected += () => client3Connected.Set();
            client3.Connect(Localhost, port);
            client3Connected.WaitAndThrowErrorIfNoSignalIsSet(DefaultWaitTIme, "Client 3 never connected");

            logger.Write<IntegrationTest>("Connecting client 2");
            var client4 = new Client(logger);
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
            var client1 = new Client(logger);
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
    }
}
