using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Logging;
using ModWrapper;
using SimpleClient;

namespace PerfomanceTestMod
{
    public class ModRunner : Mod
    {
        public event Action OnDisconnect;

        private readonly Logger logger;

        private StatisticsMaster statisticsMaster;

        private List<PerformanceChild> children;

        public ModRunner()
        {
            logger = new FileLogger("../../../Logs/", "PerformanceTestMod_", "css");
            logger.MinimumInfoLevelBeforeWrite = InfoLevel.Warning;
            initStatistics();
        }

        private void initStatistics()
        {
            var statisticsClient1 = new PerformanceChild(logger, 1);
            var statisticsClient2 = new PerformanceChild(logger, 2);
            var statisticsClient3 = new PerformanceChild(logger, 3);
            children = new List<PerformanceChild>
            {
                statisticsClient1,
                statisticsClient2,
                statisticsClient3
            };

            foreach (var performanceChild in children)
            {
                performanceChild.SubscribedToTag += clientSubscribedToTag;
            }

            statisticsMaster = new StatisticsMaster(logger, children);
            statisticsMaster.DoneRunningStatistics += () =>
            {
                disconnectAll();
                OnDisconnect?.Invoke();
            };

            statisticsMaster.SubscribedToAllClients += () =>
            {
                statisticsMasterReady = true;
                checkStart();
            };
        }

        private int clientsSubscribedToTag = 0;
        private void clientSubscribedToTag()
        {
            clientsSubscribedToTag++;
            logger.Write<ModRunner>("Another child is ready.");
            if (clientsSubscribedToTag == children.Count)
            {
                logger.Write<ModRunner>("All children ready.");
                childrenReady = true;
                checkStart();
            }
        }

        private bool statisticsMasterReady = false;
        private bool childrenReady = false;
        private object startLock = new object();
        private int startCounter = 0;
        private void checkStart()
        {
            logger.Write<ModRunner>($"Checking if we should start the test... statisticsMasterReady={statisticsMasterReady} and childrenReady={childrenReady}.");
            lock (startLock)
            {
                if (statisticsMasterReady && childrenReady)
                {
                    if (startCounter == 0)
                    {
                        Console.WriteLine("Everybody ready, starting performance test.");
                        logger.Write<StatisticsMaster>(InfoLevel.Trace, "Everybody ready, starting performance test.");
                        statisticsMaster.Start();
                        startCounter++;
                    }
                    else
                    {
                        logger.Write<ModRunner>($"Stopped duplicate performance run.");
                    }
                }
            }
            
        }

        private void disconnectAll()
        {
            statisticsMaster.Disconnect();
            foreach (PerformanceChild performanceChild in children)
            {
                performanceChild.Disconnect();
            }
        }

        public void ConnectToServer(string host, int port)
        {
            logger.Write<ModRunner>("Connecting to server...");
            statisticsMaster.ConnectToServer(host, port);
            foreach (PerformanceChild child in children)
            {
                child.ConnectToServer(host, port);
            }
        }

        public void Disconnect()
        {
            disconnectAll();
        }
    }
}
