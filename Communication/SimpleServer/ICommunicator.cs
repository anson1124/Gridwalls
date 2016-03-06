using System.Net.Sockets;

namespace SimpleClient
{
    public interface ICommunicator
    {
        void SetupCommunicationWith(TcpClient obj);
        void CloseConnectionToAllClients();
    }
}