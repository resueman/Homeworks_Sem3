using System;

namespace MyNUnit
{
    /// <summary>
    /// Throws if there signature of MyNUnit method is incorrect according to attribute applied to this method
    /// </summary>
    [Serializable]
    internal class IncorrectSignatureOfMyNUnitMethodException : Exception
    {
        /// <summary>
        /// Throws if there signature of MyNUnit method is incorrect according to attribute applied to this method
        /// </summary>
        public IncorrectSignatureOfMyNUnitMethodException()
        {
        }

        /// <summary>
        /// Throws if there signature of MyNUnit method is incorrect according to attribute applied to this method
        /// </summary>
        /// <param name="message">Message that describes an error</param>
        public IncorrectSignatureOfMyNUnitMethodException(string message) 
            : base(message)
        {
        }

        /// <summary>
        /// Throws if there signature of MyNUnit method is incorrect according to attribute applied to this method
        /// </summary>
        /// <param name="message">Message that describes an error</param>
        /// <param name="innerException">The exception that is the cause of current excepption, null if there's no inner exception</param>
        public IncorrectSignatureOfMyNUnitMethodException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }
    }
}