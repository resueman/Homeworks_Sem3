using System.Reflection;
using System.Text;
using MyNUnit;

namespace Methods
{
    /// <summary>
    /// Identifies method marked with AfterClassAttribute or BeforeClassAttribute
    /// </summary>
    public class StaticFixtureMethod : IMyNUnitMethod
    {
        private readonly MethodInfo method;

        /// <summary>
        /// Creates instance of StaticFixtureMethod class
        /// </summary>
        /// <param name="method">Method</param>
        public StaticFixtureMethod(MethodInfo method)
        {
            var errors = new StringBuilder();
            if (!method.IsStatic)
            {
                errors.AppendLine(Messages.StaticFixtureMethodMustBeStatic);
            }
            if (method.IsPrivate)
            {
                errors.AppendLine(Messages.StaticFixtureMethodMustBePublicOrInternal);
            }
            if (method.ReturnType != typeof(void))
            {
                errors.AppendLine(Messages.StaticFixtureMethodMustBeVoid);
            }
            if (method.GetParameters().Length > 0)
            {
                errors.AppendLine(Messages.StaticFixtureMethodMustHaveNoParameters);
            }
            if (errors.Length != 0)
            {
                throw new IncorrectSignatureOfMyNUnitMethodException(errors.ToString());
            }

            this.method = method;
        }

        /// <summary>
        /// Executes static fixture method
        /// </summary>
        /// <param name="instance">Test class instance on which to execute method</param>
        public void Execute(object instance)
            => method.Invoke(null, null);
    }
}