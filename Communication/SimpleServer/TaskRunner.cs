using System;
using System.Threading.Tasks;

namespace SimpleClient
{
    public class TaskRunner
    {
        public void Run(Action action)
        {
            Task.Run(action);
        }
    }
}