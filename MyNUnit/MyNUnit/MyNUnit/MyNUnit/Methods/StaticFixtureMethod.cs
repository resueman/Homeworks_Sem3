using System.Reflection;

namespace MyNUnit
{
    public class StaticFixtureMethod : MyNUnitMethod
    {
        private readonly MethodInfo method;

        public StaticFixtureMethod(MethodInfo method)
        {
            if (!method.IsStatic)
            {
                throw new IncorrectSignatureOfMyNUnitMethodException("Static fixture method should be static, not instance");
            }
            if (!method.IsPublic && !method.IsAssembly)
            {
                throw new IncorrectSignatureOfMyNUnitMethodException("Static fixture method should be public or internal");
            }
            if (method.ReturnType != typeof(void))
            {
                throw new IncorrectSignatureOfMyNUnitMethodException("Static fixture method should have void return type");
            }
            this.method = method;
        }

        public override void Execute(object instance)
            => method.Invoke(null, null);
    }
}