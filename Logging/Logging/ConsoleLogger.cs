using System;

namespace Logging
{
    public class ConsoleLogger : Logger
    {
        public InfoLevel MinimumInfoLevelBeforeWrite { get; set; }
        private readonly LogLineFactory logLineFactory;

        public ConsoleLogger()
        {
            logLineFactory = new LogLineFactory();
            MinimumInfoLevelBeforeWrite = InfoLevel.Info;
        }

        public void Write<T>(string text)
        {
            writeText<T>(InfoLevel.Info, text);
        }

        private void writeText<T>(InfoLevel infoLevel, string text)
        {
            if (infoLevel < MinimumInfoLevelBeforeWrite)
            {
                return;
            }

            Console.WriteLine(logLineFactory.Log<T>(infoLevel, text));
        }

        public void Write<T>(InfoLevel infolevel, string text)
        {
            writeText<T>(infolevel, text);
        }

        public void Dispose()
        {
        }
    }
}