using Attributes;
using System;

namespace TestProject
{
    /// <summary>
    /// Example of MyNUnit tests class
    /// </summary>
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
        public void TestThrowsNotTheSameExceptionAsExpected()
        {
            throw new AccessViolationException();
        }

        [Test(Expected = typeof(Exception))]
        public void TestThrowsExpectedException()
        {
            throw new Exception();
        }

        [Test(Ignore = "Postponed for better times...")]
        public void IgnoredTestWithoutAnyArgument()
        {

        }

        [Test(Ignore = "Postponed for better times...", Expected = typeof(AggregateException))]
        public void IgnoredTestWithExpectedExceptionArgument()
        {
            throw new AccessViolationException();
        }

        [Test(Expected = typeof(Exception))]
        public void TestNotThrowingExceptionAsExpected()
        {

        }
    }
}
