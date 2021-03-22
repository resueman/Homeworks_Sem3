using System;

namespace SimpleFTP
{
    /// <summary>
    /// Throws when connection to the server is broken
    /// </summary>
    public class ConnectionToServerException : Exception
    {
        /// <summary>
        /// Throws when connection to the server is broken
        /// </summary>
        public ConnectionToServerException()
        {
        }

        /// <summary>
        /// Throws when connection to the server is broken
        /// </summary>
        /// <param name="message">Message that describes error</param>
        public ConnectionToServerException(string message)
            : base(message)
        {
        }

        /// <summary>
        /// Throws when connection to the server is broken
        /// </summary>
        /// <param name="message">Message that describes error</param>
        /// <param name="inner">Exception that caused the current exception</param>
        public ConnectionToServerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
