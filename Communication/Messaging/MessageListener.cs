using System;
using Logging;

namespace Messaging
{
    public class MessageListener
    {
        public event Action<string> OnMessageReceived;
        public event Action DoneListeningForMessages;
        private readonly Logger logger;
        private readonly string name;

        public MessageListener(Logger logger, String name)
        {
            this.logger = logger;
            this.name = name;
        }

        public void ListenForMessages(Node node)
        {
            while (true)
            {
                logger.Write<MessageListener>(InfoLevel.Trace, $"{name} - ReadLoop: Waiting for message.");
                String msgReceived = node.Read();
                logger.Write<MessageListener>(InfoLevel.Info, $"{name} - ReadLoop: Message received: {msgReceived}");

                if (msgReceived == "")
                {
                    logger.Write<MessageListener>(InfoLevel.Trace, $"{name} - ReadLoop: Node returned empty string, stopping listening for messages.");
                    break;
                }

                logger.Write<MessageListener>(InfoLevel.Trace, $"{name} - ReadLoop: Invoking OnMessageReceived with msg: {msgReceived}");
                OnMessageReceived?.Invoke(msgReceived);
            }

            logger.Write<MessageListener>(InfoLevel.Trace, $"{name} - Done listening for messages.");
            DoneListeningForMessages?.Invoke();
        }
    }
}