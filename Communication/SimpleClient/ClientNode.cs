using System.Net.Sockets;
using System.Text;
using Logging;
using Messaging;
using Messaging.Tests;

namespace SimpleServer
{
    public class ClientNode : Node
    {
        private readonly TcpClient tcpClient;
        private MessageStreamReader messageStreamReader;
        private readonly Logger logger;

        public ClientNode(Logger logger, TcpClient tcpClient)
        {
            this.logger = logger;
            this.tcpClient = tcpClient;
        }

        public string Read()
        {
            messageStreamReader = new MessageStreamReader(logger, tcpClient.GetStream());
            return messageStreamReader.Read();
        }

        public void SendMessage(string msg)
        {
            msg = MessageFactory.CreateMessage(msg);
            logger.Write<ClientNode>($"Sending message: {msg}");
            byte[] bytes = Encoding.UTF8.GetBytes(msg);
            tcpClient.Client.Send(bytes);
        }

        public void Disconnect()
        {
            tcpClient.Close();
        }
    }
}