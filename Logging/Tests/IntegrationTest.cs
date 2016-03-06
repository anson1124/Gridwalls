using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;
using Xunit;

namespace Tests
{
    public class IntegrationTest
    {
        [Fact]
        public void should_write_info_with_expected_format()
        {
            // Given
            FileLogger logger = new FileLogger("test");

            // When
            logger.Write<IntegrationTest>("Hey");

            // Then
            var fileContents = ReadAndDeleteLog(logger);
            Assert.Equal("[INFO ] [IntegrationTest] Hey", fileContents);
        }

        [Fact]
        public void should_write_trace_with_expected_format()
        {
            // Given
            FileLogger logger = new FileLogger("test");

            // When
            logger.Write<IntegrationTest>(InfoLevel.Error, "Hey");

            // Then
            var fileContents = ReadAndDeleteLog(logger);
            Assert.Equal("[ERROR] [IntegrationTest] Hey", fileContents);
        }

        private static string ReadAndDeleteLog(FileLogger logger)
        {
            logger.Dispose();
            String fileContents = File.ReadAllText(logger.LogFilename).Trim();
            File.Delete(logger.LogFilename);
            return fileContents;
        }

        [Fact]
        public void should_not_write_when_infolevel_is_below_info()
        {
            // Given
            FileLogger logger = new FileLogger("test");

            // When
            logger.Write<IntegrationTest>(InfoLevel.Trace, "Should not be logged to file.");

            // Then
            var fileContents = ReadAndDeleteLog(logger);
            Assert.Equal(0, fileContents.Length);
        }

        [Fact]
        public void should_not_write_when_infolevel_is_below_given_setting()
        {
            // Given
            FileLogger logger = new FileLogger("test");
            logger.MinimumInfoLevelBeforeWrite = InfoLevel.Warning;

            // When
            logger.Write<IntegrationTest>(InfoLevel.Info, "Should not be logged to file.");

            // Then
            var fileContents = ReadAndDeleteLog(logger);
            Assert.Equal(0, fileContents.Length);
        }

    }
}
