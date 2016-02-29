using System;
using System.IO;

namespace Logging
{
    internal class LogFactory
    {
        public string Log(string text)
        {
            return text;
        }

        public string Log<T>(string text)
        {
            return Log<T>(InfoLevel.Info, text);
        }

        public string Log<T>(InfoLevel infolevel, string text)
        {
            return $"[{infolevel.Shortversion()}] [{typeof(T).Name}] {text}";
        }
    }
}