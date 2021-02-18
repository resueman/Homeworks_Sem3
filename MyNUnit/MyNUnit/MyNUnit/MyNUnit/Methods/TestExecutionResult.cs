using System;

namespace MyNUnit
{
    public class TestExecutionResult
    {
        public TestExecutionResult(TestExecutionStatus status, TimeSpan executionTime, 
            string errorMessage, string stackTrace)
        {
            Status = status;
            ExecutionTime = executionTime;
            Message = errorMessage;
            StackTrace = stackTrace;
        }

        public TestExecutionStatus Status { get; private set; }

        public TimeSpan ExecutionTime { get; private set; }

        public string Message { get; private set; }

        public string StackTrace { get; private set; }
    }
}
