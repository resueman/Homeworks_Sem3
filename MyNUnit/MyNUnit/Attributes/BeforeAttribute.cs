using System;

namespace Attributes
{
    /// <summary>
    /// Identifies the method that is executed before each test method in the class
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class BeforeAttribute : Attribute
    {

    }
}
