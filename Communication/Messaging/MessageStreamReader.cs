using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Logging;
using Messaging;

namespace Messaging
{
    public class MessageStreamReader : IDisposable
    {
        private readonly Stream _stream;
        private readonly MessageLengthReader _messageLengthReader;
        private readonly BufferedReader _bufferedReader;
        private readonly Logger _logger;

        public MessageStreamReader(Logger logger, Stream stream) : this(logger, stream, 10000) { }

        public MessageStreamReader(Logger logger, Stream stream, int bufferSize)
        {
            _logger = logger;
            _stream = stream;
            _messageLengthReader = new MessageLengthReader(_logger);
            _bufferedReader = new BufferedReader(_logger, stream, bufferSize);
        }

        public string Read()
        {
            _logger.Write<MessageStreamReader>("Reading next message length...");
            int messageLength = _messageLengthReader.GetNextMessageLength(_stream);
            _logger.Write<MessageStreamReader>($"Reading next message length... Result: {messageLength}");
            if (messageLength == 0)
            {
                _logger.Write<MessageStreamReader>("MessageLengthReader returned 0 length, stopping listening for messages.");
                return "";
            }

            return _bufferedReader.ReadNextBytes(messageLength);
        }

        public void Dispose()
        {
            _stream.Close();
        }
    }
}