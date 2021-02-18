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
                    ? Messages.TestMustBePublic
                    : method.IsStatic
                        ? Messages.TestMustBeInstance
                        : method.ReturnType != typeof(void)
                            ? Messages.TestMustBeVoid
                                : method.GetParameters().Length > 0
                                    ? Messages.TestMustHaveNoParameters
                                    : Messages.Empty;

            if (message != Messages.Empty)
            {
                ExecutionResult = new ExecutionResult(ExecutionStatus.Failed, TimeSpan.Zero, message, null);
            }
        }

        public MethodInfo Method { get; private set; }

        public ExecutionResult ExecutionResult { get; private set; }

        public override void Execute(object instance)
        {
            if (ExecutionResult != null && ExecutionResult.Status != ExecutionStatus.Executing)
            {
                return;
            }

            var attribute = Method.GetCustomAttribute<TestAttribute>();
            var expectedExceptionType = attribute.Expected;
            if (!string.IsNullOrEmpty(attribute.Ignore))
            {
                ExecutionResult = new ExecutionResult(ExecutionStatus.Ignored, TimeSpan.Zero, attribute.Ignore, null);
                return;
            }

            var stopWatch = new Stopwatch();
            try
            {
                stopWatch.Start();
                Method.Invoke(instance, null);
                stopWatch.Stop();
                ExecutionResult = expectedExceptionType == default
                    ? new ExecutionResult(ExecutionStatus.Success, stopWatch.Elapsed, Messages.Empty, null)
                    : new ExecutionResult(ExecutionStatus.Failed, stopWatch.Elapsed, $"Expected {expectedExceptionType.Name} to be thrown", null);
                
            }
            catch (Exception e)
            {
                stopWatch.Stop();
                var actualExceptionType = e.GetBaseException().GetType();
                var stackTrace = e.ToString();
                ExecutionResult = actualExceptionType == expectedExceptionType
                    ? new ExecutionResult(ExecutionStatus.Success, stopWatch.Elapsed, e.GetBaseException().Message, null)
                    : new ExecutionResult(ExecutionStatus.Failed, stopWatch.Elapsed,
                        $"Expected exception was {expectedExceptionType.Name}, but was {actualExceptionType.Name}", stackTrace);
            }
        }
    }
}
