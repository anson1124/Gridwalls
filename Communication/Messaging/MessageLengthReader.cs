using System;
using System.IO;
using System.Text;
using Logging;

namespace Messaging
{
    public class MessageLengthReader
    {
        private readonly Logger _logger;
        private readonly Decoder _utf8Decoder = Encoding.UTF8.GetDecoder();

        public MessageLengthReader(Logger logger)
        {
            _logger = logger;
        }

        public int GetNextMessageLength(Stream stream)
        {
            var messageLength = new StringBuilder();
            var nextChar = new char[1];

            while (true)
            {
                _logger.Write<MessageLengthReader>("GetNextMessageLength, loop: Reading byte...");
                int byteAsInt = stream.ReadByte();
                _logger.Write<MessageLengthReader>("GetNextMessageLength, loop: Reading byte... done! Got byte: " + byteAsInt);

                if (byteAsInt == -1)
                {
                    _logger.Write<MessageLengthReader>("Byte was -1, returning 0.");
                    return 0;
                }

                if (!isNumerical(byteAsInt))
                {
                    _logger.Write<MessageLengthReader>("Not numerical byte, exiting while loop.");
                    break;
                }
                _utf8Decoder.GetChars(new[] { (byte)byteAsInt }, 0, 1, nextChar, 0);
                messageLength.Append(nextChar);
            }
            _logger.Write<MessageLengthReader>($"Returning message length: {messageLength}");

            return Int32.Parse(messageLength.ToString());
        }
        private bool isNumerical(Int32 ascii)
        {
            return ascii >= 48 && ascii <= 57;  // 48 = 0, 57 = 0
        }

    }
}