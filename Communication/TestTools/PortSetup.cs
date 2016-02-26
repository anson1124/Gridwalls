using System;
using System.IO;
using System.Net;
using System.Net.Sockets;
using Logging;

namespace TestTools
{
    public class PortSetup
    {
        internal const int FirstPort = 16800;
        internal const string Datafile = "../../../test_settings/currentport.txt";
        private readonly object _portLock = new object();
        private Logger _logger;

        public PortSetup(Logger _logger)
        {
            this._logger = _logger;
        }


        public int GetNextPort()
        {
            int nextPort;

            lock (_portLock)
            {
                if (File.Exists(Datafile))
                {
                    int lastUsedPort = int.Parse(File.ReadAllText(Datafile));
                    nextPort = lastUsedPort + 1;
                }
                else
                {
                    nextPort = FirstPort;
                }

                File.WriteAllText(Datafile, nextPort + "");
            }

            _logger.Write<PortSetup>("Returned free port: " + nextPort);
            return nextPort;
        }

        public int GetNextPort2()
        {
            int port;
            lock (_portLock)
            {
                TcpListener l = new TcpListener(IPAddress.Loopback, 0);
                l.Start();
                port = ((IPEndPoint)l.LocalEndpoint).Port;
                l.Stop();
            }
            
            return port;
        }
    }
}