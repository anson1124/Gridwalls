using System;
using System.Globalization;
using System.IO;

namespace Logging
{
    internal class LogLineFactory
    {
        public string Log<T>(InfoLevel infolevel, string text)
        {
            return $"[{DateTime.Now.ToString("HH:mm:ss.fff", CultureInfo.InvariantCulture)}] [{infolevel.Shortversion()}] [{typeof(T).Name}] {text}";
        }
    }
}