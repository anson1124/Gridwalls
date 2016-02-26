using System;

namespace Messaging.Tests
{
    public class MessageFactory
    {
        public static string CreateMessage(String msg)
        {
            return $"{msg.Length},{msg}";
        }
    }
}