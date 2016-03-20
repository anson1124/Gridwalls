using System;
using System.Collections.Generic;

namespace PerfomanceTestMod
{
    internal class RoundTrip
    {
        public Datapoint Start { get; }
        public Datapoint Client { get; }
        public Datapoint End { get; }

        public RoundTrip(Datapoint start, Datapoint client, Datapoint end)
        {
            Start = start;
            Client = client;
            End = end;
        }

        public override string ToString()
        {
            TimeSpan masterToClient = Client.Subtract(Start);
            TimeSpan clientBackToMaster = End.Subtract(Client);
            return masterToClient + ", " + clientBackToMaster + " *** " + Start + " - " + Client + " - " + End;
        }
    }
}