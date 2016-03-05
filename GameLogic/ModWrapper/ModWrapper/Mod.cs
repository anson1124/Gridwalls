namespace ModWrapper
{
    public interface Mod
    {
        void ConnectToServer(string host, int port);
        void Disconnect();
    }
}