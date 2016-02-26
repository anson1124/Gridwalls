using System.IO;
using System.Net.Sockets;
using Messaging.Tests;

namespace Test.SimpleTcpServer.Messaging
{
    public class TestMemoryStreamWriter
    {
        public static void Write(Stream stream, string text)
        {
            long streamPosition = stream.Position;

            byte[] bytesToRead = ByteUtil.GetBytes(text);
            stream.Write(bytesToRead, 0, bytesToRead.Length);
            stream.Seek(streamPosition, SeekOrigin.Begin);
        }

        public static void WriteMsgWithLength(Stream stream, string msg)
        {
            Write(stream, MessageFactory.CreateMessage(msg));
        }
    }
}