using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyNUnit
{
    public class FixtureMethod : MyNUnitMethod
    {
        private MethodInfo method;

        public FixtureMethod(MethodInfo method)
        {
            if (!IsFixtureMethod(method))
            {
                NotifyUserAboutError();
            }
            this.method = method;
        }

        public static bool IsFixtureMethod(MethodInfo method) 
            => !method.IsStatic && method.ReturnType == typeof(void) && method.IsPublic;

        private void NotifyUserAboutError()
        {
            var exceptions = new List<Exception>();
            if (method.IsStatic)
            {
                exceptions.Add(new Exception("Fixture method should be instance"));
            }
            if (!method.IsPublic)
            {
                exceptions.Add(new Exception("Fixture method should be public"));
            }
            if (method.ReturnType != typeof(void))
            {
                exceptions.Add(new Exception("Fixture method should have void return type"));
            }
            if (exceptions.Count != 0)
            {
                throw new AggregateException("One or many errors occured", exceptions);
            }
        }

        public override void Execute(object instance) 
            => method.Invoke(instance, null);
    }
}
