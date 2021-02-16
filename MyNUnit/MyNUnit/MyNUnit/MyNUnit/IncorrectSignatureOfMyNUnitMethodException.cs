using System;
using System.Runtime.Serialization;

namespace MyNUnit
{
    [Serializable]
    internal class IncorrectSignatureOfMyNUnitMethodException : Exception
    {
        public IncorrectSignatureOfMyNUnitMethodException()
        {
        }

        public IncorrectSignatureOfMyNUnitMethodException(string message) 
            : base(message)
        {
        }

        public IncorrectSignatureOfMyNUnitMethodException(string message, Exception innerException) 
            : base(message, innerException)
        {
        }

        protected IncorrectSignatureOfMyNUnitMethodException(SerializationInfo info, StreamingContext context) 
            : base(info, context)
        {
        }
    }
}