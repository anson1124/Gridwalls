using System;
using System.IO;

namespace Logging
{
    public class FileLogger : Logger
    {
        public InfoLevel MinimumInfoLevelBeforeWrite { set; get; }
        public string LogFilename { get; }

        private readonly StreamWriter logStreamWriter;
        private readonly LogLineFactory logLineFactory;
        private readonly object logLock = new object();

        public FileLogger() : this("./")
        {
        }

        public FileLogger(string path) : this(path, $"log_{Path.GetRandomFileName()}", "css")
        {
        }

        public FileLogger(string path, string filenamePrefix, string filenameExtension)
        {
            LogFilename = path + $"{filenamePrefix}{Path.GetRandomFileName()}.{filenameExtension}";
            logStreamWriter = new StreamWriter(LogFilename);
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

            lock (logLock) {
                logStreamWriter.WriteLine(logLineFactory.Log<T>(infoLevel, text));
                logStreamWriter.Flush();
            }
        }

        public void Write<T>(InfoLevel infolevel, string text)
        {
            writeText<T>(infolevel, text);
        }

        public void Dispose()
        {
            logStreamWriter.Dispose();
        }
    }
}