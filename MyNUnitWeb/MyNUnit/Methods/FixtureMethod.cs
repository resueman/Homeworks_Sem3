using System.Reflection;
using System.Text;
using MyNUnit;

namespace Methods
{
    /// <summary>
    /// Identifies method marked with AfterAttribute or BeforeAttribute
    /// </summary>
    public class FixtureMethod : IMyNUnitMethod
    {
        private readonly MethodInfo method;

        /// <summary>
        /// Creates instance of FixtureMethod class
        /// </summary>
        /// <param name="method">Method</param>
        public FixtureMethod(MethodInfo method)
        {
            var errors = new StringBuilder();
            if (method.IsStatic)
            {
                errors.AppendLine(Messages.FixtureMethodMustBeInstance);
            }
            if (method.IsPrivate)
            {
                errors.AppendLine(Messages.FixtureMethodMustBePublicOrInternal);
            }
            if (method.ReturnType != typeof(void))
            {
                errors.AppendLine(Messages.FixtureMethodMustBeVoid);
            }
            if (method.GetParameters().Length > 0)
            {
                errors.AppendLine(Messages.FixtureMethodMustHaveNoParameters);
            }
            if (errors.Length != 0)
            {
                throw new IncorrectSignatureOfMyNUnitMethodException(errors.ToString());
            }

            this.method = method;
        }

        /// <summary>
        /// Executes fixture method
        /// </summary>
        /// <param name="instance">Test class instance on which to execute method</param>
        public void Execute(object instance) 
            => method.Invoke(instance, null);
    }
}
