using System;
using System.IO;

namespace Logging
{
    internal class LogLineFactory
    {
        public string Log<T>(InfoLevel infolevel, string text)
        {
            return $"[{infolevel.Shortversion()}] [{typeof(T).Name}] {text}";
        }
    }
}