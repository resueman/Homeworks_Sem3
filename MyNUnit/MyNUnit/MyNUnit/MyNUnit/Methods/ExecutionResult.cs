using System;

namespace MyNUnit
{
    /// <summary>
    /// Identifies results of test execution
    /// </summary>
    public class ExecutionResult
    {
        /// <summary>
        /// Constructor of ExecutionResult instance
        /// </summary>
        /// <param name="status">success of the test</param>
        /// <param name="executionTime">shows how much time it took to complete the test</param>
        /// <param name="message">error mesage in case of fail or exception</param>
        /// <param name="stackTrace">stack trace in case of fail</param>
        public ExecutionResult(ExecutionStatus status, TimeSpan executionTime, 
            string message, string stackTrace)
        {
            Status = status;
            ExecutionTime = executionTime;
            Message = string.IsNullOrEmpty(message) ? Messages.Empty : message;
            StackTrace = stackTrace;
        }

        /// <summary>
        /// Success of the test
        /// </summary>
        public ExecutionStatus Status { get; private set; }

        /// <summary>
        /// Shows how much time it took to complete the test
        /// </summary>
        public TimeSpan ExecutionTime { get; private set; }

        /// <summary>
        /// Error mesage in case of fail or exception
        /// </summary>
        public string Message { get; private set; }

        /// <summary>
        /// Stack trace in case of fail
        /// </summary>
        public string StackTrace { get; private set; }
    }
}
