using System.Net.Sockets;

namespace SimpleServer
{
    public interface ICommunicator
    {
        void SetupCommunicationWith(TcpClient obj);
        void CloseConnectionToAllClients();
    }
}