using System.Reflection;

namespace MyNUnit
{
    public class StaticFixtureMethod : MyNUnitMethod
    {
        private readonly MethodInfo method;

        public StaticFixtureMethod(MethodInfo method)
        {
            var errorMessage = !method.IsStatic
                ? "Static fixture method must be static, not instance"
                : method.IsPrivate
                    ? "Static fixture method must be public or internal"
                    : method.ReturnType != typeof(void)
                        ? "Static fixture method must have void return type"
                        : method.GetParameters().Length > 0
                            ? "Static fixture method must have no parameters"
                            : "";

            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new IncorrectSignatureOfMyNUnitMethodException(errorMessage);
            }

            this.method = method;
        }

        public override void Execute(object instance)
            => method.Invoke(null, null);
    }
}