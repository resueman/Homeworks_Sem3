using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Reflection;

namespace MyNUnit
{
    public class TestMethod : MyNUnitMethod
    {
        private MethodInfo method;

        public TestMethod(MethodInfo method)
        {
            if (!IsTestMethod(method))
            {
                NotifyUserAboutError();
            }
            this.method = method;
        }

        public bool IsIgnored { get; private set; }

        public string ReasonForIgnoringTest { get; private set; }

        public Type ExpectedException { get; private set; }

        public Type ActualException { get; private set; }

        public string ErrorMessage { get; private set; }

        public TimeSpan ExecutionTime { get; private set; }

        public bool Success { get; private set; }

        public override void Execute(object instance)
        {
            var attribute = method.GetCustomAttribute<TestAttribute>();
            ExpectedException = attribute.Expected;
            ReasonForIgnoringTest = attribute.Ignore;
            IsIgnored = string.IsNullOrEmpty(attribute.Ignore);

            if (IsIgnored == true)
            {
                ExecutionTime = default;
                return;
            }

            var stopWatch = new Stopwatch();
            try
            {
                stopWatch.Start();
                method.Invoke(instance, null);
                Success = true;
            }
            catch (Exception e)
            {
                ActualException = e.GetType();
                Success = ActualException == ExpectedException;
                ErrorMessage = e.Message;
            }
            finally
            {
                stopWatch.Stop();
                ExecutionTime = stopWatch.Elapsed;
            }
        }

        public static bool IsTestMethod(MethodInfo method) 
            => !method.IsStatic && method.IsPublic && method.ReturnType == typeof(void);

        private void NotifyUserAboutError()
        {
            var exceptions = new List<Exception>();
            if (method.IsStatic)
            {
                exceptions.Add(new Exception("Test method should be instance"));
            }
            if (!method.IsPublic)
            {
                exceptions.Add(new Exception("Test method should be public"));
            }
            if (method.ReturnType != typeof(void))
            {
                exceptions.Add(new Exception("Test method should have void return type"));
            }
            if (exceptions.Count != 0)
            {
                throw new AggregateException("One or many errors occured", exceptions);
            }
        }
    }
}
