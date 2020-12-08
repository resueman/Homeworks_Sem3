using System;
using System.Runtime.Serialization;

namespace Test2
{
    /// <summary>
    /// Throws when there is no file or directory corresponding to specified path
    /// </summary>
    [Serializable]
    internal class IncorrectPathException : Exception
    {
        /// <summary>
        /// Throws when there is no file or directory corresponding to specified path
        /// </summary>
        public IncorrectPathException()
        {
        }

        /// <summary>
        /// Throws when there is no file or directory corresponding to specified path
        /// </summary>
        /// <param name="message">The message that describes an exception</param>
        public IncorrectPathException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Throws when there is no file or directory corresponding to specified path
        /// </summary>
        /// <param name="message">The message that describes an exception</param>
        /// <param name="innerException">Exception that is the cause of current exception</param>
        public IncorrectPathException(string message, Exception innerException) 
            : base(message, innerException)
        { 
        }

        /// <summary>
        /// Throws when there is no file or directory corresponding to specified path
        /// </summary>
        /// <param name="info">All data needed to serialize and deserialize object</param>
        /// <param name="context">Sourse or destination of a given serialized stream</param>
        protected IncorrectPathException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}