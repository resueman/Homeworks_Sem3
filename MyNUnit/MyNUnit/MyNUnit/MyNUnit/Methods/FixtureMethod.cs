using System.Reflection;

namespace MyNUnit
{
    public class FixtureMethod : MyNUnitMethod
    {
        private readonly MethodInfo method;

        public FixtureMethod(MethodInfo method)
        {
            var errorMessage = method.IsStatic
                ? "Fixture methods, maked with Before and After attributes, must be instance, not static"
                : method.IsPrivate
                    ? "Fixture methods, maked with Before and After attributes, must be public or internal"
                    : method.ReturnType != typeof(void)
                        ? "Fixture methods, maked with Before and After attributes, must have void return type"
                        : method.GetParameters().Length > 0
                            ? "Fixture methods, maked with Before and After attributes, must have no parameters"
                            : "";

            if (!string.IsNullOrEmpty(errorMessage))
            {
                throw new IncorrectSignatureOfMyNUnitMethodException(errorMessage);
            }

            this.method = method;
        }

        public override void Execute(object instance) 
            => method.Invoke(instance, null);
    }
}
