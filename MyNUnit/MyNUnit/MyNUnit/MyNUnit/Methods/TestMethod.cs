using System;
using System.Diagnostics;
using System.Reflection;

namespace MyNUnit
{
    public class TestMethod : MyNUnitMethod
    {
        public TestMethod(MethodInfo method, string errorMessage)
        {
            Method = method;

            var message = !string.IsNullOrEmpty(errorMessage)
                ? errorMessage
                : !method.IsPublic
                    ? "Test method must be public"
                    : method.IsStatic
                        ? "Test method must be instance"
                        : method.ReturnType != typeof(void)
                            ? "Test method must have void return type"
                                : method.GetParameters().Length > 0
                                    ? "Test must have no parameters"
                                    : "";

            if (message != "")
            {
                ExecutionResult = new TestExecutionResult(TestExecutionStatus.Failed, TimeSpan.Zero, message, null);
            }
        }

        public MethodInfo Method { get; private set; }

        public TestExecutionResult ExecutionResult { get; private set; }

        public override void Execute(object instance)
        {
            if (ExecutionResult != null && ExecutionResult.Status != TestExecutionStatus.Executing)
            {
                return;
            }

            var attribute = Method.GetCustomAttribute<TestAttribute>();
            var expectedExceptionType = attribute.Expected;
            if (!string.IsNullOrEmpty(attribute.Ignore))
            {
                ExecutionResult = new TestExecutionResult(TestExecutionStatus.Ignored, TimeSpan.Zero, attribute.Ignore, null);
                return;
            }

            var stopWatch = new Stopwatch();
            try
            {
                stopWatch.Start();
                Method.Invoke(instance, null);
                stopWatch.Stop();
                ExecutionResult = new TestExecutionResult(TestExecutionStatus.Success, stopWatch.Elapsed, "", null);
            }
            catch (Exception e)
            {
                stopWatch.Stop();
                var actualExceptionType = e.GetBaseException().GetType();
                var stackTrace = e.ToString();
                ExecutionResult = actualExceptionType == expectedExceptionType
                    ? new TestExecutionResult(TestExecutionStatus.Success, stopWatch.Elapsed, "", null)
                    : new TestExecutionResult(TestExecutionStatus.Failed, stopWatch.Elapsed,
                        $"Expected exception was {expectedExceptionType.Name}, but was {actualExceptionType}", stackTrace);
            }
        }
    }
}
