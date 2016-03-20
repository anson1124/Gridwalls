using System;
using System.Collections.Generic;
using Logging;
using SimpleClient;

namespace PerfomanceTestMod
{
    public class PerformanceChild
    {
        public event Action SubscribedToTag;
        public static readonly string AppendSeparator = ";";
        public readonly string Tag;

        private int id;
        private readonly Client client;
        private readonly List<Datapoint> datapoints = new List<Datapoint>();

        public PerformanceChild(Logger logger, int id)
        {
            client = ClientFactory.Create(logger);
            this.id = id;
            Tag = "client" + id;
        }

        public void ConnectToServer(string host, int port)
        {
            client.OnMessageReceived += messageReceived;
            client.Connect(host, port);
            client.SubscribeTo(Tag);
            client.OnSubscribed += (tag) => SubscribedToTag?.Invoke();
        }

        private void sendNewDatapoint(string incomingMessage)
        {
            int incommingDatapointId = Datapoint.Deserialize(incomingMessage).Id;
            var datapoint = new Datapoint("client" + id, incommingDatapointId, DateTime.Now);
            datapoints.Add(datapoint);
            client.SendMessage($"[{Tag}] " + incomingMessage + AppendSeparator + datapoint.Serialize());
        }

        private void messageReceived(string incomingMessage)
        {
            sendNewDatapoint(incomingMessage);
        }

        internal void Disconnect()
        {
            client.Disconnect();
        }

    }
}