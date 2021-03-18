using Attributes;
using System;

namespace TestProject
{
    /// <summary>
    /// Example of MyNUnit tests class
    /// </summary>
    public class ClassForTestingFixtureMethodsCorrectSignatures
    {
        private int counter = 1;

        [Before]
        public void PublicBeforeTest()
        {
            ++counter;
        }

        [Before]
        internal void InternalBeforeTest()
        {
            ++counter;
        }

        [Test(Expected = typeof(Exception))]
        public void TestForBeforeTest()
        {
            throw new Exception(counter.ToString());
        }
    }
}
