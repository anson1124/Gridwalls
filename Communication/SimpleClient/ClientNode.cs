using System.Net.Sockets;
using System.Text;
using Logging;
using Messaging;
using Messaging.Tests;

namespace SimpleClient
{
    public class ClientNode : Node
    {
        private readonly TcpClient _tcpClient;
        private MessageStreamReader _messageStreamReader;
        private readonly Logger _logger;

        public ClientNode(Logger logger, TcpClient tcpClient)
        {
            _logger = logger;
            _tcpClient = tcpClient;
        }

        public string Read()
        {
            _messageStreamReader = new MessageStreamReader(_logger, _tcpClient.GetStream());
            return _messageStreamReader.Read();
        }

        public void SendMessage(string msg)
        {
            msg = MessageFactory.CreateMessage(msg);
            _logger.Write<ClientNode>($"Sending message: {msg}");
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            _tcpClient.Client.Send(bytes);
        }

        public void Close()
        {
            _tcpClient.Close();
        }
    }
}