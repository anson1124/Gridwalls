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
        private readonly Stream stream;
        private readonly MessageLengthReader messageLengthReader;
        private readonly BufferedReader bufferedReader;
        private readonly Logger logger;

        public MessageStreamReader(Logger logger, Stream stream) : this(logger, stream, 10000) { }

        public MessageStreamReader(Logger logger, Stream stream, int bufferSize)
        {
            this.logger = logger;
            this.stream = stream;
            messageLengthReader = new MessageLengthReader(this.logger);
            bufferedReader = new BufferedReader(this.logger, stream, bufferSize);
        }

        public string Read()
        {
            logger.Write<MessageStreamReader>(InfoLevel.Trace, "Reading next message length...");
            int messageLength = messageLengthReader.GetNextMessageLength(stream);
            logger.Write<MessageStreamReader>(InfoLevel.Trace, $"Reading next message length... Result: {messageLength}");
            if (messageLength == 0)
            {
                logger.Write<MessageStreamReader>(InfoLevel.Trace, "MessageLengthReader returned 0 length, stopping listening for messages.");
                return "";
            }

            return bufferedReader.ReadNextBytes(messageLength);
        }

        public void Dispose()
        {
            stream.Close();
        }
    }
}