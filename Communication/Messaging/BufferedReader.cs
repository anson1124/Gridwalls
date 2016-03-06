using System;
using System.IO;
using System.Net.Sockets;
using System.Text;
using Logging;

namespace Messaging
{
    public class BufferedReader
    {
        private readonly Logger _logger;
        private readonly Stream _stream;
        private readonly byte[] _buffer;

        private StringBuilder _msgBuilder;

        public BufferedReader(Logger logger, Stream stream, int bufferSize)
        {
            _logger = logger;
            _stream = stream;
            _buffer = new byte[bufferSize];
        }

        public string ReadNextBytes(int messageLength)
        {
            _logger.Write<BufferedReader>(InfoLevel.Trace, $"Read message, message length={messageLength}");
            _msgBuilder = new StringBuilder();

            int bytesLeft = messageLength;
            int bufferOffset = 0;
            do
            {
                int bytesToRead = Math.Min(_buffer.Length, bytesLeft);
                _logger.Write<BufferedReader>(InfoLevel.Trace, $"Read from buffer, offset={bufferOffset}, bytesToRead={bytesToRead}");

                doRead(bufferOffset, bytesToRead);

                bufferOffset += bytesToRead;
                if (bufferOffset == _buffer.Length)
                {
                    bufferOffset = 0;
                }

                bytesLeft -= bytesToRead;
            } while (bytesLeft > 0);

            return _msgBuilder.ToString();
        }

        private void doRead(int bufferOffset, int bytesToRead)
        {
            _stream.Read(_buffer, bufferOffset, bytesToRead);
            appendFromBufferToMessage(bufferOffset, bytesToRead);
        }

        private void appendFromBufferToMessage(int bufferOffset, int bytesToRead)
        {
            String msgPart = Encoding.UTF8.GetString(_buffer, bufferOffset, bytesToRead);
            _logger.Write<BufferedReader>(InfoLevel.Trace, $"Appending to message: {msgPart}");
            _msgBuilder.Append(msgPart);
        }
    }
}