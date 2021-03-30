using System;
using System.Runtime.Serialization;

namespace Initialize
{
    /// <summary>
    /// Throws when it's impossible to create instance of type
    /// </summary>
    [Serializable]
    internal class InjectorException : Exception
    {
        /// <summary>
        /// Throws when it's impossible to create instance of type
        /// </summary>
        public InjectorException()
        {
        }

        /// <summary>
        /// Throws when it's impossible to create instance of type
        /// </summary>
        /// <param name="message">Message</param>
        public InjectorException(string message) : base(message)
        {
        }

        /// <summary>
        /// Throws when it's impossible to create instance of type
        /// </summary>
        /// <param name="message">Message</param>
        /// <param name="innerException">Exception caused the problem</param>
        public InjectorException(string message, Exception innerException) : base(message, innerException)
        {
        }

        /// <summary>
        /// Throws when it's impossible to create instance of type
        /// </summary>
        /// <param name="info">Stores data about serialization</param>
        /// <param name="context"></param>
        protected InjectorException(SerializationInfo info, StreamingContext context) : base(info, context)
        {
        }
    }
}