using System;
using System.Net.Sockets;
using Logging;

namespace SimpleClient
{
    public class ClientNodeFactory : IClientNodeFactory
    {
        public ClientNode Create(Logger logger, TcpClient tcpClient)
        {
            return new ClientNode(logger, tcpClient, createClientName());
        }

        private string createClientName()
        {
            return Guid.NewGuid().ToString().Substring(0, 5);
        }

    }
}