using System;
using System.Threading;
using System.Threading.Tasks;

namespace SimpleServer
{
    public static class AutoResetEventExtensions
    {
        public static async Task WaitOneAsync(this AutoResetEvent autoResetEvent)
        {
            await Task.Run(() =>
            {
                autoResetEvent.WaitOne();
            });
        }

        public static void WaitAndThrowErrorIfNoSignalIsSet(this AutoResetEvent autoResetEvent, int millisecondsTimeout, string errorMessage)
        {
            bool signalWasSet = autoResetEvent.WaitOne(millisecondsTimeout);
            if (!signalWasSet)
            {
                throw new ApplicationException(errorMessage);
            }
        }
    }
}