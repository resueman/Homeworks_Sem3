using MyNUnit;
using System;

namespace TestProject
{
    public class CorrectTestClass
    {
        [BeforeClass]
        public static void BeforeClass()
        {

        }

        [AfterClass]
        public static void AfterClass()
        {

        }

        [Before]
        public void BeforeTest()
        {

        }

        [After]
        public void AfterTest()
        {

        }

        [Test]
        public void SuccessTestWithoutAnyArguments()
        {

        }

        [Test(Expected = typeof(ArgumentNullException))]
        public void FailedTestWithException()
        {
            throw new AccessViolationException();
        }

        [Test(Expected = typeof(ArgumentNullException))]
        public void SuccessTestWithException()
        {
            throw new ArgumentNullException();
        }

        [Test(Ignore = "Postponed for better times...")]
        public void IgnoredTestWithoutAnyArgument()
        {

        }

        [Test(Ignore = "Postponed for better times...", Expected = typeof(AggregateException))]
        public void IgnoredTestWithArgument()
        {
            throw new AggregateException();
        }
    }
}
