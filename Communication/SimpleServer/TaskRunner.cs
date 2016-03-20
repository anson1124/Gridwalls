using System;
using System.Threading.Tasks;

namespace SimpleServer
{
    public class TaskRunner
    {
        public void Run(Action action)
        {
            Task.Run(action);
        }
    }
}