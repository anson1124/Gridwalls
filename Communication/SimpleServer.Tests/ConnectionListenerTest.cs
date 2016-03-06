using System;
using Logging;
using SimpleTcpServer;
using TestTools;
using Xunit;

namespace SimpleClient.Tests
{
    public class ConnectionListenerTest : IDisposable
    {
        private readonly Logger _logger = LogSetup.CreateLogger();
        
        public void Dispose()
        {
            LogSetup.DisposeLogger(_logger);
        }

        [Fact]
        public void Can_stop_listening_for_connections_after_listening_has_started()
        {
            // Given
            int port = new PortSetup(_logger).GetNextPort();
            var connectionListener = new ConnectionListener(_logger);
            connectionListener.ListenForConnectionsInANewThread(port);

            // When
            _logger.Write<ConnectionListenerTest>("Stopping listening.");
            connectionListener.StopListening();
            // Then no error should occur
        }

        [Fact]
        public void Listening_for_connections_twice_should_throw_an_error()
        {
            // Given
            int port = new PortSetup(_logger).GetNextPort();
            var connectionListener = new ConnectionListener(_logger);
            connectionListener.ListenForConnectionsInANewThread(port);

            // Then
            Assert.Throws<InvalidOperationException>(() =>
            {
                // When
                connectionListener.ListenForConnectionsInANewThread(port);
            });

            // Finally
            connectionListener.StopListening();
        }

        [Fact]
        public void Starting_and_stopping_and_starting_to_listen_for_connections_should_not_throw_any_errors()
        {
            // Given
            int port = new PortSetup(_logger).GetNextPort();
            var connectionListener = new ConnectionListener(_logger);

            // When
            connectionListener.ListenForConnectionsInANewThread(port);
            connectionListener.StopListening();

            connectionListener.ListenForConnectionsInANewThread(port);
            connectionListener.StopListening();

            // Then no error should occur
        }

        [Fact]
        public void Stopping_listening_should_not_throw_an_error_if_we_are_not_listening()
        {
            // Given
            var connectionListener = new ConnectionListener(_logger);

            // When
            connectionListener.StopListening();

            // Then no error should occur
        }
    }
}