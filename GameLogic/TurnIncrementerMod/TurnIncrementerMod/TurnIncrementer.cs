using System;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using SimpleClient;

namespace TurnIncrementerMod
{
    public class TurnIncrementer
    {
        private readonly Client client;
        private readonly Logger logger;
        private Int64 newTurn = 0;
        private bool sendMoreEvents = true;

        public TurnIncrementer(Logger logger, Client client)
        {
            this.logger = logger;
            this.client = client;
        }

        public void StartSendingEvents()
        {
            Task sendEventTask = Task.Run(() =>
            {
                while (sendMoreEvents)
                {
                    sendNextTurnEvent();
                    Thread.Sleep(1000);
                }
            });
        }

        private void sendNextTurnEvent()
        {
            newTurn++;
            client.SendMessage("TurnIncremented " + newTurn);
        }

        public void StopSendingEvents()
        {
            sendMoreEvents = false;
        }
    }
}