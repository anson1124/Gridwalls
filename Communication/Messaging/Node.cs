namespace Messaging
{
    public interface Node
    {
        string Read();
        void Disconnect();
        void SendMessage(string msg);
    }
}