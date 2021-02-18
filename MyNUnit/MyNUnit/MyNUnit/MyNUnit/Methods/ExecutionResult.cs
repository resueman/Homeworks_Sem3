using System;

namespace MyNUnit
{
    public class ExecutionResult
    {
        public ExecutionResult(ExecutionStatus status, TimeSpan executionTime, 
            string message, string stackTrace)
        {
            Status = status;
            ExecutionTime = executionTime;
            Message = string.IsNullOrEmpty(message) ? Messages.Empty : message;
            StackTrace = stackTrace;
        }

        public ExecutionStatus Status { get; private set; }

        public TimeSpan ExecutionTime { get; private set; }

        public string Message { get; private set; }

        public string StackTrace { get; private set; }
    }
}
