using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;
using PerfomanceTestMod;
using Xunit;

namespace Tests
{
    public class Test
    {
        [Fact]
        public void Should_process_message_correctly()
        {
            // Given
            var logger = new FileLogger("../../../Logs/", "PerformanceTestMod_Tests_", "css");
            var statisticsMaster = new StatisticsMaster(logger, null);
            string msg = "master,1,2016-03-13T20:35:12.9135581+01:00;client1,1,2016-03-13T20:35:12.9175627+01:00";

            statisticsMaster.processMsg(msg);
        }
    }
}
