using System.Reflection;

namespace MyNUnit
{
    /// <summary>
    /// Identifies method marked with AfterClassAttribute or BeforeClassAttribute
    /// </summary>
    public class StaticFixtureMethod : MyNUnitMethod
    {
        private readonly MethodInfo method;

        /// <summary>
        /// Creates instance of StaticFixtureMethod class
        /// </summary>
        /// <param name="method">Method</param>
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

        /// <summary>
        /// Executes static fixture method
        /// </summary>
        /// <param name="instance">Test class instance on which to execute method</param>
        public override void Execute(object instance)
            => method.Invoke(null, null);
    }
}