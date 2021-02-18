using MyNUnit;
using System;

namespace TestProject
{
    public class ClassForTestingSeveralStaticFixtureMethodsWork
    {
        private static int counter = 1;

        [BeforeClass]
        public static void BeforeClassTest1()
        {
            ++counter;
        }

        [BeforeClass]
        public static void BeforeClassTest2()
        {
            ++counter;
        }

        [Test(Expected = typeof(Exception))]
        public void TestForBeforeClassTest()
        {
            throw new Exception(counter.ToString());
        }
    }
}
