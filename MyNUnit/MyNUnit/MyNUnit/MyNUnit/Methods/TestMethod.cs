using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyNUnit
{
    class TestMethod : MyNUnitMethod
    {
        public TestMethod(MethodInfo method)
        {
            Method = method;
        }

        public bool IsIgnored { get; private set; }

        public Exception Expected { get; private set; }

        public MethodInfo Method { get; private set; }

        public TimeSpan ExecutionTime { get; private set; }

        public List<Exception> Exceptions { get; private set; }

        public bool Success => Exceptions.Count == 0;

        public override void Execute(object instance)
        {

        }
    }
}
