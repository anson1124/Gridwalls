using System;
using System.Net.Sockets;

namespace SimpleServer
{
    public interface IConnectionListener
    {
        event Action<TcpClient> OnClientConnected;

        void ListenForConnectionsInANewThread(int port);

        void StopListening();
    }
}