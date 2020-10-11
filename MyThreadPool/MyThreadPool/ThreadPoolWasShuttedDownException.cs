using System;

namespace MyThreadPool
{
    public class ThreadPoolWasShuttedDownException : Exception
    {
        public ThreadPoolWasShuttedDownException()
        {
        }

        public ThreadPoolWasShuttedDownException(string message)
            : base(message)
        {
        }

        public ThreadPoolWasShuttedDownException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
