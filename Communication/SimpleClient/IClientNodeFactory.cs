using System.Net.Sockets;
using Logging;

namespace SimpleServer
{
    public interface IClientNodeFactory
    {
        ClientNode Create(Logger logger, TcpClient tcpClient);
    }
}