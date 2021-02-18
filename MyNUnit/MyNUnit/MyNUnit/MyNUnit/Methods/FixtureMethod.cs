using System.Reflection;

namespace MyNUnit
{
    public class FixtureMethod : MyNUnitMethod
    {
        private readonly MethodInfo method;

        public FixtureMethod(MethodInfo method)
        {
            var errorMessage = method.IsStatic
                ? Messages.FixtureMethodMustBeInstance
                : method.IsPrivate
                    ? Messages.FixtureMethodMustBePublicOrInternal
                    : method.ReturnType != typeof(void)
                        ? Messages.FixtureMethodMustBeVoid
                        : method.GetParameters().Length > 0
                            ? Messages.FixtureMethodMustHaveNoParameters
                            : Messages.Empty;

            if (errorMessage != Messages.Empty)
            {
                throw new IncorrectSignatureOfMyNUnitMethodException(errorMessage);
            }

            this.method = method;
        }

        public override void Execute(object instance) 
            => method.Invoke(instance, null);
    }
}
