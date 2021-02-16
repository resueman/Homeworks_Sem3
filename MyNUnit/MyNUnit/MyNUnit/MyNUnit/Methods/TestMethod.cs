using System;
using System.Diagnostics;
using System.Reflection;

namespace MyNUnit
{
    public class TestMethod : MyNUnitMethod
    {
        private readonly MethodInfo method;

        public TestMethod(MethodInfo method, string errorMessage)
        {
            this.method = method;

            var message = !string.IsNullOrEmpty(errorMessage)
                ? errorMessage
                : !method.IsPublic
                    ? "Test method should be public"
                    : method.IsStatic
                        ? "Test method should be instance"
                        : method.ReturnType != typeof(void)
                            ? "Test method should have void return type"
                            : "";

            if (message != "")
            {
                ExecutionResult = new TestExecutionResult(TestExecutionStatus.Failed, TimeSpan.Zero, message, null);
            }
        }

        public TestExecutionResult ExecutionResult { get; private set; }

        public override void Execute(object instance)
        {
            var attribute = method.GetCustomAttribute<TestAttribute>();
            var expectedException = attribute.Expected;
            if (!string.IsNullOrEmpty(attribute.Ignore))
            {
                ExecutionResult = new TestExecutionResult(TestExecutionStatus.Ignored, TimeSpan.Zero, attribute.Ignore, null);
                return;
            }

            var stopWatch = new Stopwatch();
            try
            {
                stopWatch.Start();
                method.Invoke(instance, null);
            }
            catch (Exception e)
            {
                stopWatch.Stop();
                ExecutionResult = e.GetType() != expectedException
                    ? new TestExecutionResult(TestExecutionStatus.Failed, stopWatch.Elapsed,
                        $"Expected exception was {expectedException.Name}, but was {e.GetType()}", e.ToString())
                    : new TestExecutionResult(TestExecutionStatus.Success, stopWatch.Elapsed, "", null);
            }
        }
    }
}
