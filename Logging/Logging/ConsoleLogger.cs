using System;

namespace Logging
{
    public class ConsoleLogger : Logger
    {
        private readonly LogFactory _logFactory;

        public ConsoleLogger()
        {
            _logFactory = new LogFactory();
        }

        public void Write(string text)
        {
            Console.WriteLine(_logFactory.Log(text));
        }

        public void Write<T>(string text)
        {
            Write(_logFactory.Log<T>(text));
        }

        public void Write<T>(InfoLevel infolevel, string text)
        {
            Write(_logFactory.Log<T>(infolevel, text));
        }

        public void Dispose()
        {
        }
    }
}