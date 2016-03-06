using System.Net.Sockets;
using Logging;
using SimpleServer;

namespace SimpleServer
{
    public interface IClientNodeFactory
    {
        ClientNode Create(Logger logger, TcpClient tcpClient);
    }
}