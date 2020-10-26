using System;

namespace SimpleFTP
{
    public class ConnectionToServerException : Exception
    {
        public ConnectionToServerException()
        {
        }

        public ConnectionToServerException(string message)
            : base(message)
        {
        }

        public ConnectionToServerException(string message, Exception inner)
            : base(message, inner)
        {
        }
    }
}
