using System;

namespace MyNUnit
{
    /// <summary>
    /// Identifies the method that is executed only once before any test methods are executed
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class BeforeClassAttribute : Attribute
    {

    }
}
