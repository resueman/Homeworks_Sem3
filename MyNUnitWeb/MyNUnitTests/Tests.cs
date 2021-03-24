using Methods;
using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MyNUnitTests
{
    /// <summary>
    /// Contains tests for MyNUnit
    /// </summary>
    public class Tests
    {
        private static List<MyNUnit.MyNUnitTestsClass> testsClasses;

        [OneTimeSetUp]
        public static void BeforeClass()
        {
            testsClasses = MyNUnit.MyNUnit.Run(@"./../../../..\TestProject\bin\Debug\netcoreapp3.1").Result;
        }

        [TestCaseSource(typeof(TestCases), "IncorrectTestMethodSignatureTestCases", Category = "Incorrect Test Method Signature")]
        [TestCaseSource(typeof(TestCases), "PassingTestMethodsTestCases", Category = "Passing Test Methods")]
        [TestCaseSource(typeof(TestCases), "FailedTestMethodsTestCases", Category = "Failed Test Methods")]
        [TestCaseSource(typeof(TestCases), "IgnoredTestMethodsTestCases", Category = "Ignored Test Methods")]
        [TestCaseSource(typeof(TestCases), "IncorrectFixtureMethodSignatureTestCases", Category = "Incorrect Fixture Method Signatures")]
        [TestCaseSource(typeof(TestCases), "IncorrectStaticFixtureMethodSignatureTestCases", Category = "Incorrect Static Fixture Method Signatures")]
        [TestCaseSource(typeof(TestCases), "StaticFixtureMethodTestCases", Category = "Static Fixture Methods Correct Work")]
        [TestCaseSource(typeof(TestCases), "FixtureMethodTestCases", Category = "Fixture Method Correct Work")]
        public void Test(string className, string testMethodName, ExecutionStatus expectedStatus, string expectedMessage)
        {
            var testsClass = testsClasses.Single(c => c.TestClassType.Name == className);
            var testResult = testsClass.TestMethods.Single(m => m.Method.Name == testMethodName).ExecutionResult;
            Assert.AreEqual(expectedStatus, testResult.Status);
            Assert.AreEqual(expectedMessage, testResult.Message.Replace("\r\n", ""));
        }

        [TestCaseSource(typeof(TestCases), "IncorrectTestMethodSignatureTestCases", Category = "Zero Execution Time")]
        public void ZeroExecutionTimeTest(string className, string testMethodName, ExecutionStatus expectedStatus, string expectedMessage)
        {
            var testsClass = testsClasses.Single(c => c.TestClassType.Name == className);
            var testResult = testsClass.TestMethods.Single(m => m.Method.Name == testMethodName).ExecutionResult;
            Assert.AreEqual(TimeSpan.Zero, testResult.ExecutionTime);
            Assert.AreEqual(expectedStatus, testResult.Status);
            Assert.AreEqual(expectedMessage, testResult.Message.Replace("\r\n", ""));
        }

        [TestCaseSource(typeof(TestCases), "PassingTestMethodsTestCases", Category = "Not Zero Execution Time")]
        public void NotZeroExecutionTimeTest(string className, string testMethodName, ExecutionStatus expectedStatus, string expectedMessage)
        {
            var testsClass = testsClasses.Single(c => c.TestClassType.Name == className);
            var testResult = testsClass.TestMethods.Single(m => m.Method.Name == testMethodName).ExecutionResult;
            Assert.AreNotEqual(TimeSpan.Zero, testResult.ExecutionTime);
            Assert.AreEqual(expectedStatus, testResult.Status);
            Assert.AreEqual(expectedMessage, testResult.Message.Replace("\r\n", ""));
        }
    }
}