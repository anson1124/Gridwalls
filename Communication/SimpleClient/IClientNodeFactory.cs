using System.Net.Sockets;
using Logging;
using SimpleServer;

namespace SimpleClient
{
    public interface IClientNodeFactory
    {
        ClientNode Create(Logger logger, TcpClient tcpClient);
    }
}