using System;
using Logging;

namespace TestTools
{
    public class LogSetup
    {
        private const string Path = "../../../Logs/";
        private const string DefaultFilenamePrefix = "testLog_";
        private const string FilenameExtension = "css";

        public static Logger CreateLogger()
        {
            return CreateLogger(DefaultFilenamePrefix);
        }

        public static Logger CreateLogger(string filenameprefix)
        {
            Logger logger = new FileLogger(Path, filenameprefix, FilenameExtension);
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