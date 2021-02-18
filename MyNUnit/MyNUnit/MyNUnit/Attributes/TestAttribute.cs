using System;

namespace MyNUnit
{
    /// <summary>
    /// Identifies test method
    /// It is possible to ignore the test method
    /// it is possible to pass the type of the expected exception
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class TestAttribute : Attribute
    {
        public Type Expected { get; set; }

        public string Ignore { get; set; }
    }
}
