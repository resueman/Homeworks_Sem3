using System.Reflection;

namespace MyNUnit
{
    public class StaticFixtureMethod : MyNUnitMethod
    {
        private MethodInfo method;
        private object instance;

        public StaticFixtureMethod(MethodInfo method)
        {
            this.method = method;
        }

        public override void Execute(object instance)
            => method.Invoke(null, null);
    }
}