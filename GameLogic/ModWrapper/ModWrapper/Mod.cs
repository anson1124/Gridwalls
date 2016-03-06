using System;

namespace ModWrapper
{
    public interface Mod
    {
        event Action OnDisconnect;

        void ConnectToServer(string host, int port);

        void Disconnect();
    }
}