using System.Net.Sockets;
using Logging;
using SimpleClient;

namespace SimpleServer
{
    public interface IClientNodeFactory
    {
        ClientNode Create(Logger logger, TcpClient tcpClient);
    }
}