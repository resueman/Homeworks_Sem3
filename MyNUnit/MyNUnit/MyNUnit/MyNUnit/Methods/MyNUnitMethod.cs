namespace MyNUnit
{
    /// <summary>
    /// Identifies MyNUnit methods marked with MyNUnit attributes
    /// </summary>
    public abstract class MyNUnitMethod
    {
        /// <summary>
        /// Executes any of MyNUnit methods marked with custom attributes method
        /// </summary>
        /// <param name="instance">Test class instance on which to execute method</param>
        public abstract void Execute(object instance);
    }
}