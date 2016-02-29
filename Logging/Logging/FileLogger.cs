using System;
using System.IO;

namespace Logging
{
    public class FileLogger : Logger
    {
        private readonly StreamWriter _logStreamWriter;
        private readonly LogFactory _logFactory;
        private readonly object _logLock = new object();

        public FileLogger() : this("./")
        {
        }

        public FileLogger(string path) : this(path, $"log_{Path.GetRandomFileName()}", "css")
        {
        }

        public FileLogger(string path, string filenamePrefix, string filenameExtension)
        {
            string logFilename = path + $"{filenamePrefix}{Path.GetRandomFileName()}.{filenameExtension}";
            _logStreamWriter = new StreamWriter(logFilename);
            _logFactory = new LogFactory();
        }

        public void Write(string text)
        {
            lock (_logLock)
            {
                _logStreamWriter.WriteLine(_logFactory.Log(text));
                _logStreamWriter.Flush();
            }
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
            _logStreamWriter.Dispose();
        }
    }
}