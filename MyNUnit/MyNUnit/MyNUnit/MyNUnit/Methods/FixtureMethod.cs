using System.Reflection;

namespace MyNUnit
{
    public class FixtureMethod : MyNUnitMethod
    {
        private readonly MethodInfo method;

        public FixtureMethod(MethodInfo method)
        {
            if (method.IsStatic)
            {
                throw new IncorrectSignatureOfMyNUnitMethodException("Fixture methods, maked with Before and After attributes, should be instance, not static");
            }
            if (!method.IsPublic && !method.IsAssembly)
            {
                throw new IncorrectSignatureOfMyNUnitMethodException("Fixture methods, maked with Before and After attributes, should be public or internal");
            }
            if (method.ReturnType != typeof(void))
            {
                throw new IncorrectSignatureOfMyNUnitMethodException("Fixture methods, maked with Before and After attributes, should have void return type");
            }
            this.method = method;
        }

        public override void Execute(object instance) 
            => method.Invoke(instance, null);
    }
}
