using System.Text;

namespace Test.SimpleTcpServer.Messaging
{
    public class ByteUtil
    {
        public static byte[] GetBytes(string text)
        {
            return Encoding.UTF8.GetBytes(text);
        }

        public static string GetString(byte[] bytes)
        {
            return Encoding.UTF8.GetString(bytes);
        }
    }
}