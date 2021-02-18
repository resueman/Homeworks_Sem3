using System;

namespace MyNUnit
{
    /// <summary>
    /// Identifies the method that is executed after the completion of each test method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class AfterAttribute : Attribute
    {

    }
}
