using System;
using System.Net.Sockets;

namespace SimpleClient
{
    public interface IConnectionListener
    {
        event Action<TcpClient> OnClientConnected;

        void ListenForConnectionsInANewThread(int port);

        void StopListening();
    }
}