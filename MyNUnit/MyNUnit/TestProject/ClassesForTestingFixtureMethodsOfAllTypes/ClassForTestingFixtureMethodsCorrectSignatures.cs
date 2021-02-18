using MyNUnit;
using System;

namespace TestProject
{
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
