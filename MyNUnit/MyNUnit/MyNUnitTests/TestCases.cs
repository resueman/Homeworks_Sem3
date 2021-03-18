using Methods;

namespace MyNUnitTests
{
    /// <summary>
    /// Test cases for MyNUnit testing
    /// </summary>
    public class TestCases
    {
        private static readonly object[] IncorrectTestMethodSignatureTestCases =
        {
            new object[]
            {
                "ClassWithCorrectAndIncorrectTestMethods", "PrivateTest", ExecutionStatus.Failed, MyNUnit.Messages.TestMustBePublic
            },
            new object[]
            {
                "ClassWithCorrectAndIncorrectTestMethods", "InternalTest", ExecutionStatus.Failed, MyNUnit.Messages.TestMustBePublic
            },
            new object[]
            {
                "ClassWithCorrectAndIncorrectTestMethods", "ParameterizedTest", ExecutionStatus.Failed, MyNUnit.Messages.TestMustHaveNoParameters
            },
            new object[]
            {
                "ClassWithCorrectAndIncorrectTestMethods", "StaticTest", ExecutionStatus.Failed, MyNUnit.Messages.TestMustBeInstance
            },
            new object[]
            {
                "ClassWithCorrectAndIncorrectTestMethods", "NonVoidTest", ExecutionStatus.Failed, MyNUnit.Messages.TestMustBeVoid
            }
        };

        private static readonly object[] PassingTestMethodsTestCases =
        {
            new object[]
            {
                "ClassWithCorrectAndIncorrectTestMethods", "TestWithCorrectSignature", ExecutionStatus.Success, MyNUnit.Messages.Empty
            },
            new object[]
            {
                "CorrectTestClass", "TestThrowsExpectedException", ExecutionStatus.Success, "Exception of type 'System.Exception' was thrown."
            },
            new object[]
            {
                "CorrectTestClass", "SuccessTestWithoutAnyArguments", ExecutionStatus.Success, MyNUnit.Messages.Empty
            }
        };

        private static readonly object[] FailedTestMethodsTestCases =
        {
            new object[]
            {
                "CorrectTestClass", "TestThrowsNotTheSameExceptionAsExpected", ExecutionStatus.Failed, "Expected exception was ArgumentNullException, but was AccessViolationException"
            },
            new object[]
            {
                "CorrectTestClass", "TestNotThrowingExceptionAsExpected", ExecutionStatus.Failed, "Expected Exception to be thrown"
            }
        };

        private static readonly object[] IgnoredTestMethodsTestCases =
        {
            new object[]
            {
                "CorrectTestClass", "IgnoredTestWithoutAnyArgument", ExecutionStatus.Ignored, "Postponed for better times..."
            },
            new object[]
            {
                "CorrectTestClass", "IgnoredTestWithExpectedExceptionArgument", ExecutionStatus.Ignored, "Postponed for better times..."
            }
        };

        private static readonly object[] IncorrectFixtureMethodSignatureTestCases =
        {
            new object[]
            {
                "ClassWithNonVoidFixtureMethod", "TestFromClassWithNonVoidFixtureMethod", ExecutionStatus.Failed, MyNUnit.Messages.FixtureMethodMustBeVoid
            },
            new object[]
            {
                "ClassWithParameterizedFixtureMethod", "TestFromClassWithParameterizedFixtureMethod", ExecutionStatus.Failed, MyNUnit.Messages.FixtureMethodMustHaveNoParameters
            },
            new object[]
            {
                "ClassWithPrivateFixtureMethod", "TestFromClassWithPrivateFixtureMethod", ExecutionStatus.Failed, MyNUnit.Messages.FixtureMethodMustBePublicOrInternal
            },
            new object[]
            {
                "ClassWithStaticFixtureMethod", "TestFromClassWithStaticFixtureMethod", ExecutionStatus.Failed, MyNUnit.Messages.FixtureMethodMustBeInstance
            }
        };

        private static readonly object[] IncorrectStaticFixtureMethodSignatureTestCases =
        {
            new object[]
            {
                "ClassWithNonVoidStaticFixtureMethod", "TestFromClassWithNonVoidStaticFixtureMethod", ExecutionStatus.Failed, MyNUnit.Messages.StaticFixtureMethodMustBeVoid
            },
            new object[]
            {
                "ClassWithParameterizedStaticFixtureMethod", "TestFromClassWithParameterizedStaticFixtureMethod", ExecutionStatus.Failed, MyNUnit.Messages.StaticFixtureMethodMustHaveNoParameters
            },
            new object[]
            {
                "ClassWithPrivateStaticFixtureMethod", "TestFromClassWithPrivateStaticFixtureMethod", ExecutionStatus.Failed, MyNUnit.Messages.StaticFixtureMethodMustBePublicOrInternal
            },
            new object[]
            {
                "ClassWithNonStaticStaticFixtureMethods", "TestFromClassWithNonStaticStaticFixtureMethods", ExecutionStatus.Failed, MyNUnit.Messages.StaticFixtureMethodMustBeStatic
            }
        };

        private static readonly object[] StaticFixtureMethodTestCases =
        {
            new object[]
            {
                "ClassForTestingStaticFixtureMethodsCorrectSignatures", "TestForBeforeClassTest", ExecutionStatus.Success, "3"
            },
            new object[]
            {
                "ClassForTestingSeveralStaticFixtureMethodsWork", "TestForBeforeClassTest", ExecutionStatus.Success, "3"
            }
        };

        private static readonly object[] FixtureMethodTestCases =
        {
            new object[]
            {
                "ClassForTestingFixtureMethodsCorrectSignatures", "TestForBeforeTest", ExecutionStatus.Success, "3"
            },
            new object[]
            {
                "ClassForTestingSeveralFixtureMetodsWork", "TestForBeforeTest", ExecutionStatus.Success, "3"
            }
        };
    }
}