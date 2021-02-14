using System.Reflection;

namespace MyNUnit
{
    class FixtureMethod : MyNUnitMethod
    {
        private MethodInfo method;

        public FixtureMethod(MethodInfo method)
        {
            this.method = method;
        }

        public override void Execute(object instance) 
            => method.Invoke(instance, null);
    }
}
