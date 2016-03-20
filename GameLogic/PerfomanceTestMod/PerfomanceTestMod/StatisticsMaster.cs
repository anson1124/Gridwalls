using System;
using System.Collections.Generic;
using Logging;
using SimpleClient;

namespace PerfomanceTestMod
{
    public class StatisticsMaster
    {
        public event Action SubscribedToAllClients;
        public event Action DoneRunningStatistics;

        private readonly Logger logger;
        private readonly Client client;

        private readonly  List<RoundTrip> roundtrips = new List<RoundTrip>();
        private int msgCount = 0;
        private int currentDatapointId = 0;
        private List<PerformanceChild> children;

        public StatisticsMaster(Logger logger, List<PerformanceChild> children)
        {
            this.logger = logger;
            this.children = children;
            client = ClientFactory.Create(logger);
        }

        public void ConnectToServer(string host, int port)
        {
            client.OnMessageReceived += messageReceived;
            client.Connect(host, port);

            foreach (var child in children)
            {
                client.SubscribeTo(child.Tag);
                client.OnSubscribed += onSubscribed;
            }
        }

        private int subscriptions = 0;
        private void onSubscribed(string obj)
        {
            subscriptions++;
            if (subscriptions == children.Count)
            {
                logger.Write<StatisticsMaster>(InfoLevel.Trace, "Now subscribed to all clients.");
                SubscribedToAllClients?.Invoke();
            }
        }

        public void Start()
        {
            sendNewDatapoint();
        }

        private void sendNewDatapoint()
        {
            currentDatapointId++;
            var datapoint = new Datapoint("master", currentDatapointId, DateTime.Now);
            foreach (var performanceChild in children)
            {
                string msg = $"[{performanceChild.Tag}] " + datapoint.Serialize();
                logger.Write<StatisticsMaster>(InfoLevel.Trace, "Sending message to server: " + msg);
                client.SendMessage(msg);
            }
        }

        private void messageReceived(string incomingMessage)
        {
            logger.Write<StatisticsMaster>(InfoLevel.Trace, "Received message: " + incomingMessage);
            processMsg(incomingMessage);

            if (msgCount < 20000)
            {
                logger.Write<StatisticsMaster>(InfoLevel.Trace, "msgCount is below 10, so sending another datapoint.");
                sendNewDatapoint();
                msgCount++;
            }
            else
            {
                logger.Write<StatisticsMaster>(InfoLevel.Warning, "Done doing statistics.");
                doneProcessing();
            }
        }

        private void doneProcessing()
        {
            writeToLog();
            DoneRunningStatistics?.Invoke();
        }

        private void writeToLog()
        {
            foreach (RoundTrip roundtrip in roundtrips)
            {
                logger.Write<StatisticsMaster>(InfoLevel.Warning, roundtrip.ToString());
            }
        }

        public void processMsg(string incomingMessage)
        {
            logger.Write<StatisticsMaster>(InfoLevel.Trace, "Processing message...");

            string[] splitted = incomingMessage.Split(new string[] { PerformanceChild.AppendSeparator }, StringSplitOptions.None);
            Datapoint startDatapoint = Datapoint.Deserialize(splitted[0]);
            Datapoint clientDatapoint = Datapoint.Deserialize(splitted[1]);
            Datapoint endDatapoint = createEndDatapoint(startDatapoint.Id);

            var roundtrip = new RoundTrip(startDatapoint, clientDatapoint, endDatapoint);
            roundtrips.Add(roundtrip);
            logger.Write<StatisticsMaster>(InfoLevel.Trace, "Processing message... done");
        }

        private Datapoint createEndDatapoint(int datapointId)
        {
            return new Datapoint("master", datapointId, DateTime.Now);
        }

        internal void Disconnect()
        {
            writeToLog();
            client.Disconnect();
        }
    }
}