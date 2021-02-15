using System;
using System.Collections.Generic;
using System.Reflection;

namespace MyNUnit
{
    public class StaticFixtureMethod : MyNUnitMethod
    {
        private MethodInfo method;

        public StaticFixtureMethod(MethodInfo method)
        {
            if (!IsStaticFixtureMethod(method))
            {
                NotifyUserAboutError();
            }
            this.method = method;
        }

        public static bool IsStaticFixtureMethod(MethodInfo method) 
            => method.IsStatic && method.IsPublic && method.ReturnType == typeof(void);

        private void NotifyUserAboutError()
        {
            var exceptions = new List<Exception>();
            if (!method.IsStatic)
            {
                exceptions.Add(new Exception("Static fixture method should be static"));
            }
            if (!method.IsPublic) //////////////////////
            {
                exceptions.Add(new Exception("Static fixture method should be public"));
            }
            if (method.ReturnType != typeof(void))
            {
                exceptions.Add(new Exception("Static fixture method should have void return type"));
            }
            if (exceptions.Count != 0)
            {
                throw new AggregateException("One or many errors occured", exceptions);
            }
        }

        public override void Execute(object instance)
            => method.Invoke(null, null);
    }
}