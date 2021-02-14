using System;

namespace MyNUnit
{
    [AttributeUsage(AttributeTargets.Method, Inherited = false)]
    public class TestAttribute : Attribute
    {
        public Type Expected { get; set; }

        public string Ignore { get; set; }
    }
}
