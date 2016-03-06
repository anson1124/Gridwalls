using System.Net.Sockets;
using Logging;
using SimpleClient;

namespace SimpleServer
{
    public class ClientNodeFactory : IClientNodeFactory
    {
        public ClientNode Create(Logger logger, TcpClient tcpClient)
        {
            return new ClientNode(logger, tcpClient);
        }
    }
}