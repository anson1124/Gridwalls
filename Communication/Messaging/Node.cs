namespace Messaging
{
    public interface Node
    {
        string Read();
        void Close();
        void SendMessage(string msg);
    }
}