using System;
using Logging;

namespace Messaging
{
    public class MessageListener
    {
        public event Action<string> OnMessageReceived;
        public event Action DoneListeningForMessages;
        private readonly Logger _logger;
        private readonly string _name;

        public MessageListener(Logger logger, String name)
        {
            _logger = logger;
            _name = name;
        }

        public void ListenForMessages(Node node)
        {
            while (true)
            {
                _logger.Write<MessageListener>($"{_name} - ReadLoop: Waiting for message.");
                String msgReceived = node.Read();
                _logger.Write<MessageListener>($"{_name} - ReadLoop: Message received: {msgReceived}");

                if (msgReceived == "")
                {
                    _logger.Write<MessageListener>($"{_name} - ReadLoop: Node returned empty string, stopping listening for messages.");
                    break;
                }

                _logger.Write<MessageListener>($"{_name} - ReadLoop: Invoking OnMessageReceived with msg: {msgReceived}");
                OnMessageReceived?.Invoke(msgReceived);
            }

            _logger.Write<MessageListener>($"{_name} - Done listening for messages.");
            DoneListeningForMessages?.Invoke();
        }

        // TODO: Support abort reading.
    }
}