using System;
using Logging;

namespace TestTools
{
    public class LogSetup
    {
        private const String Path = "../../../../../";
        private const String FilenamePrefix = "logs/testLog_";
        private const String FilenameExtension = "css";

        public static Logger CreateLogger()
        {
            Logger logger = new FileLogger(Path, FilenamePrefix, FilenameExtension);
            logger.Write<LogSetup>("Log created.");
            return logger;
        }

        public static void DisposeLogger(Logger logger)
        {
            logger.Write<LogSetup>("Disposing log.");
            logger.Dispose();
        }
    }
}