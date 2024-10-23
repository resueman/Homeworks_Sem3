using System;

namespace MyThreadPool
{
    /// <summary>
    /// Throws if there is an attempt to enqueue task to shutted down thread pool
    /// or shut down the pool more than once
    /// </summary>
    public class ThreadPoolWasShuttedDownException : Exception
    {
        /// <summary>
        /// Throws if there is an attempt to enqueue task to shutted down thread pool
        /// or shut down the pool more than once
        /// </summary>
        public ThreadPoolWasShuttedDownException()
        {
        }

        /// <summary>
        /// Throws if there is an attempt to enqueue task to shutted down thread pool
        /// or shut down the pool more than once
        /// </summary>
        /// <param name="message">Message that describes an error</param>
        public ThreadPoolWasShuttedDownException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Throws if there is an attempt to enqueue task to shutted down thread pool
        /// or shut down the pool more than once
        /// </summary>
        /// <param name="message">Message that describes an error</param>
        /// <param name="inner">The exception that is the cause of current excepption, null if there's no inner exception</param>
        public ThreadPoolWasShuttedDownException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
