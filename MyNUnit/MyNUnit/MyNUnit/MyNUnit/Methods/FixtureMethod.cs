using System.Reflection;

namespace MyNUnit
{
    /// <summary>
    /// Identifies method marked with AfterAttribute or BeforeAttribute
    /// </summary>
    public class FixtureMethod : MyNUnitMethod
    {
        private readonly MethodInfo method;

        /// <summary>
        /// Creates instance of FixtureMethod class
        /// </summary>
        /// <param name="method">Method</param>
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

        /// <summary>
        /// Executes fixture method
        /// </summary>
        /// <param name="instance">Test class instance on which to execute method</param>
        public override void Execute(object instance) 
            => method.Invoke(instance, null);
    }
}
