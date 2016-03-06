using System.Net.Sockets;
using Logging;
using SimpleServer;

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