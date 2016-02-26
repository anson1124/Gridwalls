using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using Messaging;
using Test.SimpleTcpServer.Messaging;
using TestTools;
using Xunit;

namespace Messaging.Tests
{
    public class MessageLengthReaderTest : IDisposable
    {
        private readonly Logger _logger;

        public MessageLengthReaderTest()
        {
            _logger = LogSetup.CreateLogger();
        }

        public void Dispose()
        {
            LogSetup.DisposeLogger(_logger);
        }

        [Fact]
        public void Should_determine_length_of_message()
        {
            // Given
            var reader = new MessageLengthReader(_logger);
            var stream = new MemoryStream();
            TestMemoryStreamWriter.Write(stream, MessageFactory.CreateMessage("House"));

            // When
            int msgLength = reader.GetNextMessageLength(stream);

            // Then
            Assert.Equal(5, msgLength);
            
            // Finally
            stream.Close();
        }

        [Fact]
        public void Length_of_msg_should_be_0_when_no_more_messages()
        {
            // Given
            var reader = new MessageLengthReader(_logger);
            var stream = new MemoryStream();

            // When
            int msgLength = reader.GetNextMessageLength(stream);

            // Then
            Assert.Equal(0, msgLength);

            // Finally
            stream.Close();
        }
    }
}