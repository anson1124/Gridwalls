using System.Net.Sockets;
using Logging;

namespace SimpleClient
{
    public interface IClientNodeFactory
    {
        ClientNode Create(Logger logger, TcpClient tcpClient);
    }
}