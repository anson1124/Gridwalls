using System;
using System.Globalization;

namespace PerfomanceTestMod
{
    public class Datapoint
    {
        public int Id { get; }
        public DateTime Time { get; private set; }

        private readonly string source;

        public Datapoint(string source, int id, DateTime time)
        {
            this.source = source;
            this.Id = id;
            this.Time = time;
        }

        public string Serialize()
        {
            return source + "," + Id + "," + Time.ToString("O");
        }

        public static Datapoint Deserialize(string seralized)
        {
            string[] splitted = seralized.Split(',');

            string source = splitted[0];
            int id = Int32.Parse(splitted[1]);
            DateTime time = DateTime.Parse(splitted[2], CultureInfo.InvariantCulture, DateTimeStyles.RoundtripKind);

            return new Datapoint(source, id, time);
        }

        public TimeSpan Subtract(Datapoint other)
        {
            return Time - other.Time;
        }

        public override string ToString()
        {
            return Serialize();
        }
    }
}