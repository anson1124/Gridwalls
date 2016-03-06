using System;
using System.IO;
using System.Text;
using Logging;

namespace Messaging
{
    public class MessageLengthReader
    {
        private readonly Logger logger;
        private readonly Decoder utf8Decoder = Encoding.UTF8.GetDecoder();

        public MessageLengthReader(Logger logger)
        {
            this.logger = logger;
        }

        public int GetNextMessageLength(Stream stream)
        {
            var messageLength = new StringBuilder();
            var nextChar = new char[1];

            while (true)
            {
                logger.Write<MessageLengthReader>(InfoLevel.Trace, "GetNextMessageLength, loop: Reading byte...");
                int byteAsInt = stream.ReadByte();
                logger.Write<MessageLengthReader>(InfoLevel.Trace, "GetNextMessageLength, loop: Reading byte... done! Got byte: " + byteAsInt);

                if (byteAsInt == -1)
                {
                    logger.Write<MessageLengthReader>(InfoLevel.Trace, "Byte was -1, returning 0.");
                    return 0;
                }

                if (!isNumerical(byteAsInt))
                {
                    logger.Write<MessageLengthReader>(InfoLevel.Trace, "Not numerical byte, exiting while loop.");
                    break;
                }
                utf8Decoder.GetChars(new[] { (byte)byteAsInt }, 0, 1, nextChar, 0);
                messageLength.Append(nextChar);
            }
            logger.Write<MessageLengthReader>(InfoLevel.Trace, $"Returning message length: {messageLength}");

            return Int32.Parse(messageLength.ToString());
        }
        private bool isNumerical(Int32 ascii)
        {
            return ascii >= 48 && ascii <= 57;  // 48 = 0, 57 = 0
        }

    }
}