using Attributes;
using System;

namespace TestProject
{
    /// <summary>
    /// Example of MyNUnit tests class
    /// </summary>
    public class ClassForTestingSeveralFixtureMetodsWork
    {
        private int counter = 1;

        [Before]
        public void BeforeTest1()
        {
            ++counter;
        }

        [Before]
        public void BeforeTest2()
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
