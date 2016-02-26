using System;
using System.IO;
using Logging;
using Messaging;
using TestTools;
using Xunit;

namespace Messaging.Tests
{
    public class MessageStreamReaderTest : IDisposable
    {
        private readonly Logger _logger;

        public MessageStreamReaderTest()
        {
            _logger = LogSetup.CreateLogger();
        }

        public void Dispose()   
        {
            LogSetup.DisposeLogger(_logger);
        }

        [Fact]
        public void Should_return_empty_string_if_no_more_messages()
        {
            // Given
            var stream = new MemoryStream();
            var reader = new MessageStreamReader(_logger, stream);

            // When
            String msg = reader.Read();

            // Then
            Assert.Equal("", msg);

            // Finally
            stream.Close();
        }
    }
}