using System;
using System.Runtime.Serialization;

namespace Test
{
    /// <summary>
    /// Throws when there is no connection to server
    /// </summary>
    [Serializable]
    internal class ConnectionToServerException : Exception
    {
        public ConnectionToServerException()
        {
        }

        public ConnectionToServerException(string message) : base(message)
        {
        }

        public ConnectionToServerException(string message, Exception innerException) : base(message, innerException)
        {
        }

        protected ConnectionToServerException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}