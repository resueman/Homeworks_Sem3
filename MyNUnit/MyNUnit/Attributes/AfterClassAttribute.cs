using System;

namespace Attributes
{
    /// <summary>
    /// Identifies the method that is executed only once after the completion of all test and after-test methods
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class AfterClassAttribute : Attribute
    {

    }
}
