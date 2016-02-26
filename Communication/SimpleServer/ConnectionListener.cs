using System;
using System.Net;
using System.Net.Sockets;
using System.Threading;
using System.Threading.Tasks;
using Logging;
using SimpleTcpServer;

namespace SimpleServer
{
    public class ConnectionListener
    {
        public event Action<TcpClient> OnClientConnected;

        private TcpListener _tcpListener;
        private bool _hasStartedListening;
        private AutoResetEvent _tcpListenerStartedEvent;
        private bool _hasStartedListeningInANewThread;
        private readonly AutoResetEvent _doneListeningForConnections = new AutoResetEvent(false);
        private readonly Logger _logger;

        public ConnectionListener(Logger logger)
        {
            _logger = logger;
        }

        public void ListenForConnectionsInANewThread(int port)
        {
            throwErrorIfAlreadyListening();
            _tcpListenerStartedEvent = new AutoResetEvent(false);
            _logger.Write<ConnectionListener>(InfoLevel.Trace, "Starting listening in a new thread.");

            Task.Run(() => listenForConnections(port));

            _tcpListenerStartedEvent.WaitAndThrowErrorIfNoSignalIsSet(3000, "Internal bug, the TCP listener was never started.");
        }

        private void throwErrorIfAlreadyListening()
        {
            _logger.Write<ConnectionListener>(
                $"ListenForConnectionsInANewThread. _hasStartedListeningInANewThread = {_hasStartedListeningInANewThread}");
            if (_hasStartedListeningInANewThread)
            {
                _logger.Write<ConnectionListener>("Throwing exception.");
                throw new InvalidOperationException(
                    "Already started listening for connections. Stop listening before starting again.");
            }

            _hasStartedListeningInANewThread = true;
        }

        private void listenForConnections(int port)
        {
            _logger.Write<ConnectionListener>(InfoLevel.Trace, $"The new thread has started.");

            // Start TCP listener
            _logger.Write<ConnectionListener>(InfoLevel.Trace, "Starting tcp listener.");
            _tcpListener = new TcpListener(IPAddress.Parse("127.0.0.1"), port);
            _tcpListener.Start();

            _hasStartedListening = false;

            while (true)
            {
                _logger.Write<ConnectionListener>(InfoLevel.Trace, $"Start of listening while loop for new connections.");
                Task<TcpClient> acceptTcpClientTask = Task.Run(() => _tcpListener.AcceptTcpClient());

                _logger.Write<ConnectionListener>(InfoLevel.Trace, $"_hasStartedListening={_hasStartedListening}.");
                if (!_hasStartedListening)
                {
                    _hasStartedListening = true;
                    signalTcpListenerStarted();
                }

                // Wait for connections or stop
                ListenResult listenResult = waitForNextConnectionOrStopSignal(acceptTcpClientTask);
                if (listenResult == ListenResult.StopListening)
                {
                    break;
                }

                // Run OnClientConnected event
                _logger.Write<ConnectionListener>(InfoLevel.Info, "Client connected.");
                OnClientConnected?.Invoke(acceptTcpClientTask.Result);
            }

            _doneListeningForConnections.Set();

        }

        private void signalTcpListenerStarted()
        {
            _logger.Write<ConnectionListener>(InfoLevel.Trace, "Signalling _tcpListenerStartedEvent");
            _tcpListenerStartedEvent.Set();
        }

        private ListenResult waitForNextConnectionOrStopSignal(Task<TcpClient> acceptTcpClientTask)
        {
            try
            {
                _logger.Write<ConnectionListener>(InfoLevel.Trace, "Waiting for acceptTcpClientTask (= new connection or stop signal)...");
                acceptTcpClientTask.Wait();
            }
            catch (AggregateException e)
            {
                if (e.InnerException.GetType() == typeof(SocketException))
                {
                    _logger.Write<ConnectionListener>(InfoLevel.Trace,
                        "Stop was called when waiting for acceptTcpClientTask. Aborting listening.");
                    return ListenResult.StopListening;
                }

                throw e;
            }

            _logger.Write<ConnectionListener>(InfoLevel.Trace, "Waiting for acceptTcpClientTask done. Got a real client.");
            return ListenResult.ContinueListening;
        }

        public void StopListening()
        {
            _logger.Write<ConnectionListener>($"Aborting listening. " +
                                                      $"_hasStartedListeningInANewThread={_hasStartedListeningInANewThread} " +
                                                      $"_hasStartedListening={_hasStartedListening}");
            if (!_hasStartedListeningInANewThread)
            {
                return;
            }

            if (_hasStartedListening)
            {
                _tcpListener.Stop();
            }

            _hasStartedListeningInANewThread = false;
            _doneListeningForConnections.WaitAndThrowErrorIfNoSignalIsSet(1000, "Listening for connections never completed in time.");
        }
    }

    enum ListenResult
    {
        ContinueListening,
        StopListening
    };
}