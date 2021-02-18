using MyNUnit;
using System;

namespace TestProject
{
    public class ClassForTestingStaticFixtureMethodsCorrectSignatures
    {
        private static int counter = 1;

        [BeforeClass]
        public static void PublicBeforeClass()
        {
            ++counter;
        }

        [BeforeClass]
        internal static void InternalBeforeClass()
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
