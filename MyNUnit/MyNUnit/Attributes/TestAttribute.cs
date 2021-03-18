using System;

namespace Attributes
{
    /// <summary>
    /// Identifies test method
    /// </summary>
    [AttributeUsage(AttributeTargets.Method, Inherited = false, AllowMultiple = false)]
    public class TestAttribute : Attribute
    {
        /// <summary>
        /// Identifies the type of expected exception
        /// </summary>
        public Type Expected { get; set; }

        /// <summary>
        /// Identifies the reason for ignoring the test
        /// </summary>
        public string Ignore { get; set; }
    }
}
