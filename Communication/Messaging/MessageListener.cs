using System;
using Logging;

namespace Messaging
{
    public class MessageListener : IMessageListener
    {
        public event Action<string> OnMessageReceived;
        public event Action<Node> DoneListeningForMessages;
        private readonly Logger logger;

        public MessageListener(Logger logger)
        {
            this.logger = logger;
        }

        public void ListenForMessages(Node node)
        {
            while (true)
            {
                logger.Write<MessageListener>(InfoLevel.Trace, "ReadLoop: Waiting for message.");
                String msgReceived = node.Read();
                logger.Write<MessageListener>(InfoLevel.Trace, $"ReadLoop: Message received: {msgReceived}");

                if (msgReceived == "")
                {
                    logger.Write<MessageListener>(InfoLevel.Trace, "ReadLoop: Node returned empty string, stopping listening for messages.");
                    break;
                }

                logger.Write<MessageListener>(InfoLevel.Trace, $"ReadLoop: Invoking OnMessageReceived with msg: {msgReceived}");
                OnMessageReceived?.Invoke(msgReceived);
            }

            logger.Write<MessageListener>(InfoLevel.Trace, "Done listening for messages.");
            DoneListeningForMessages?.Invoke(node);
        }
    }
}