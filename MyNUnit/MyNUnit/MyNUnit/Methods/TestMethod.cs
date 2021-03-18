using Attributes;
using MyNUnit;
using System;
using System.Diagnostics;
using System.Reflection;
using System.Text;

namespace Methods
{
    /// <summary>
    /// Identifies method marked with TestAttribute
    /// </summary>
    public class TestMethod : IMyNUnitMethod
    {
        /// <summary>
        /// Creates instance of TestMethod class
        /// </summary>
        /// <param name="method">Method</param>
        /// <param name="errorMessage">Message send by any type of fixture methods
        /// in case of error occured during their execution</param>
        public TestMethod(MethodInfo method, string errorMessage)
        {
            Method = method;
            var errors = new StringBuilder();
            if (!string.IsNullOrEmpty(errorMessage))
            {
                errors.AppendLine(errorMessage);
            }
            if (!method.IsPublic)
            {
                errors.AppendLine(Messages.TestMustBePublic);
            }
            if (method.IsStatic)
            {
                errors.AppendLine(Messages.TestMustBeInstance);
            }
            if (method.ReturnType != typeof(void))
            {
                errors.AppendLine(Messages.TestMustBeVoid);
            }
            if (method.GetParameters().Length > 0)
            {
                errors.AppendLine(Messages.TestMustHaveNoParameters);
            }
            if (errors.Length != 0)
            {
                ExecutionResult = new ExecutionResult(ExecutionStatus.Failed, TimeSpan.Zero, errors.ToString(), null);
            }
        }

        /// <summary>
        /// Represents executed method
        /// </summary>
        public MethodInfo Method { get; private set; }

        /// <summary>
        /// Represents the result of the test execution
        /// </summary>
        public ExecutionResult ExecutionResult { get; private set; }

        /// <summary>
        /// Executes test method
        /// </summary>
        /// <param name="instance">Test class instance on which to execute method</param>
        public void Execute(object instance)
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
