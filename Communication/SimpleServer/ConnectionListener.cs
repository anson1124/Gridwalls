using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Logging;

namespace SimpleServer
{
    public class ConnectionListener : IConnectionListener
    {
        public event Action<TcpClient> OnClientConnected;

        private TcpListener tcpListener;
        private bool hasStartedListening;
        private AutoResetEvent tcpListenerStartedEvent;
        private bool hasStartedListeningInANewThread;
        private readonly AutoResetEvent doneListeningForConnections = new AutoResetEvent(false);
        private readonly Logger logger;

        public ConnectionListener(Logger logger)
        {
            this.logger = logger;
        }

        public void ListenForConnectionsInANewThread(int port)
        {
            throwErrorIfAlreadyListening();
            tcpListenerStartedEvent = new AutoResetEvent(false);
            logger.Write<ConnectionListener>(InfoLevel.Trace, "Starting listening in a new thread.");

            Task.Run(() => listenForConnections(port));

            tcpListenerStartedEvent.WaitAndThrowErrorIfNoSignalIsSet(3000, "Internal bug, the TCP listener was never started.");
        }

        private void throwErrorIfAlreadyListening()
        {
            logger.Write<ConnectionListener>(InfoLevel.Trace, $"ListenForConnectionsInANewThread. _hasStartedListeningInANewThread = {hasStartedListeningInANewThread}");
            if (hasStartedListeningInANewThread)
            {
                logger.Write<ConnectionListener>(InfoLevel.Error, "Throwing exception.");
                throw new InvalidOperationException(
                    "Already started listening for connections. Stop listening before starting again.");
            }

            hasStartedListeningInANewThread = true;
        }

        private void listenForConnections(int port)
        {
            logger.Write<ConnectionListener>(InfoLevel.Trace, $"The new thread has started.");

            // Start TCP listener
            logger.Write<ConnectionListener>(InfoLevel.Trace, "Starting tcp listener.");
            tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            tcpListener.Start();

            hasStartedListening = false;

            while (true)
            {
                logger.Write<ConnectionListener>(InfoLevel.Trace, $"Start of listening while loop for new connections.");
                Task<TcpClient> acceptTcpClientTask = Task.Run(() => tcpListener.AcceptTcpClient());

                logger.Write<ConnectionListener>(InfoLevel.Trace, $"_hasStartedListening={hasStartedListening}.");
                if (!hasStartedListening)
                {
                    hasStartedListening = true;
                    signalTcpListenerStarted();
                }

                // Wait for connections or stop
                ListenResult listenResult = waitForNextConnectionOrStopSignal(acceptTcpClientTask);
                if (listenResult == ListenResult.StopListening)
                {
                    break;
                }

                // Run OnClientConnected event
                logger.Write<ConnectionListener>(InfoLevel.Info, "Client connected.");
                OnClientConnected?.Invoke(acceptTcpClientTask.Result);
            }

            doneListeningForConnections.Set();

        }

        private void signalTcpListenerStarted()
        {
            logger.Write<ConnectionListener>(InfoLevel.Trace, "Signalling _tcpListenerStartedEvent");
            tcpListenerStartedEvent.Set();
        }

        private ListenResult waitForNextConnectionOrStopSignal(Task<TcpClient> acceptTcpClientTask)
        {
            try
            {
                logger.Write<ConnectionListener>(InfoLevel.Trace, "Waiting for acceptTcpClientTask (= new connection or stop signal)...");
                acceptTcpClientTask.Wait();
            }
            catch (AggregateException e)
            {
                if (e.InnerException.GetType() == typeof(SocketException))
                {
                    logger.Write<ConnectionListener>(InfoLevel.Trace,
                        "Stop was called when waiting for acceptTcpClientTask. Aborting listening.");
                    return ListenResult.StopListening;
                }

                throw e;
            }

            logger.Write<ConnectionListener>(InfoLevel.Trace, "Waiting for acceptTcpClientTask done. Got a real client.");
            return ListenResult.ContinueListening;
        }

        public void StopListening()
        {
            logger.Write<ConnectionListener>(InfoLevel.Trace, $"Aborting listening. " +
                                                      $"_hasStartedListeningInANewThread={hasStartedListeningInANewThread} " +
                                                      $"_hasStartedListening={hasStartedListening}");
            if (!hasStartedListeningInANewThread)
            {
                return;
            }

            if (hasStartedListening)
            {
                tcpListener.Stop();
            }

            hasStartedListeningInANewThread = false;
            doneListeningForConnections.WaitAndThrowErrorIfNoSignalIsSet(1000, "Listening for connections never completed in time.");
        }
    }

    enum ListenResult
    {
        ContinueListening,
        StopListening
    };
}