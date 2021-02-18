using System.Reflection;

namespace MyNUnit
{
    public class StaticFixtureMethod : MyNUnitMethod
    {
        private readonly MethodInfo method;

        public StaticFixtureMethod(MethodInfo method)
        {
            var errorMessage = !method.IsStatic
                ? Messages.StaticFixtureMethodMustBeStatic
                : method.IsPrivate
                    ? Messages.StaticFixtureMethodMustBePublicOrInternal
                    : method.ReturnType != typeof(void)
                        ? Messages.StaticFixtureMethodMustBeVoid
                        : method.GetParameters().Length > 0
                            ? Messages.StaticFixtureMethodMustHaveNoParameters
                            : Messages.Empty;

            if (errorMessage != Messages.Empty)
            {
                throw new IncorrectSignatureOfMyNUnitMethodException(errorMessage);
            }

            this.method = method;
        }

        public override void Execute(object instance)
            => method.Invoke(null, null);
    }
}