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
                "ClassWithCorrectAndIncorrectTestMethods", "PrivateTest", MyNUnit.ExecutionStatus.Failed, MyNUnit.Messages.TestMustBePublic
            },
            new object[]
            {
                "ClassWithCorrectAndIncorrectTestMethods", "InternalTest", MyNUnit.ExecutionStatus.Failed, MyNUnit.Messages.TestMustBePublic
            },
            new object[]
            {
                "ClassWithCorrectAndIncorrectTestMethods", "ParameterizedTest", MyNUnit.ExecutionStatus.Failed, MyNUnit.Messages.TestMustHaveNoParameters
            },
            new object[]
            {
                "ClassWithCorrectAndIncorrectTestMethods", "StaticTest", MyNUnit.ExecutionStatus.Failed, MyNUnit.Messages.TestMustBeInstance
            },
            new object[]
            {
                "ClassWithCorrectAndIncorrectTestMethods", "NonVoidTest", MyNUnit.ExecutionStatus.Failed, MyNUnit.Messages.TestMustBeVoid
            }
        };

        private static readonly object[] PassingTestMethodsTestCases =
        {
            new object[]
            {
                "ClassWithCorrectAndIncorrectTestMethods", "TestWithCorrectSignature", MyNUnit.ExecutionStatus.Success, MyNUnit.Messages.Empty
            },
            new object[]
            {
                "CorrectTestClass", "TestThrowsExpectedException", MyNUnit.ExecutionStatus.Success, "Exception of type 'System.Exception' was thrown."
            },
            new object[]
            {
                "CorrectTestClass", "SuccessTestWithoutAnyArguments", MyNUnit.ExecutionStatus.Success, MyNUnit.Messages.Empty
            }
        };

        private static readonly object[] FailedTestMethodsTestCases =
        {
            new object[]
            {
                "CorrectTestClass", "TestThrowsNotTheSameExceptionAsExpected", MyNUnit.ExecutionStatus.Failed, "Expected exception was ArgumentNullException, but was AccessViolationException"
            },
            new object[]
            {
                "CorrectTestClass", "TestNotThrowingExceptionAsExpected", MyNUnit.ExecutionStatus.Failed, "Expected Exception to be thrown"
            }
        };

        private static readonly object[] IgnoredTestMethodsTestCases =
        {
            new object[]
            {
                "CorrectTestClass", "IgnoredTestWithoutAnyArgument", MyNUnit.ExecutionStatus.Ignored, "Postponed for better times..."
            },
            new object[]
            {
                "CorrectTestClass", "IgnoredTestWithExpectedExceptionArgument", MyNUnit.ExecutionStatus.Ignored, "Postponed for better times..."
            }
        };

        private static readonly object[] IncorrectFixtureMethodSignatureTestCases =
        {
            new object[]
            {
                "ClassWithNonVoidFixtureMethod", "TestFromClassWithNonVoidFixtureMethod", MyNUnit.ExecutionStatus.Failed, MyNUnit.Messages.FixtureMethodMustBeVoid
            },
            new object[]
            {
                "ClassWithParameterizedFixtureMethod", "TestFromClassWithParameterizedFixtureMethod", MyNUnit.ExecutionStatus.Failed, MyNUnit.Messages.FixtureMethodMustHaveNoParameters
            },
            new object[]
            {
                "ClassWithPrivateFixtureMethod", "TestFromClassWithPrivateFixtureMethod", MyNUnit.ExecutionStatus.Failed, MyNUnit.Messages.FixtureMethodMustBePublicOrInternal
            },
            new object[]
            {
                "ClassWithStaticFixtureMethod", "TestFromClassWithStaticFixtureMethod", MyNUnit.ExecutionStatus.Failed, MyNUnit.Messages.FixtureMethodMustBeInstance
            }
        };

        private static readonly object[] IncorrectStaticFixtureMethodSignatureTestCases =
        {
            new object[]
            {
                "ClassWithNonVoidStaticFixtureMethod", "TestFromClassWithNonVoidStaticFixtureMethod", MyNUnit.ExecutionStatus.Failed, MyNUnit.Messages.StaticFixtureMethodMustBeVoid
            },
            new object[]
            {
                "ClassWithParameterizedStaticFixtureMethod", "TestFromClassWithParameterizedStaticFixtureMethod", MyNUnit.ExecutionStatus.Failed, MyNUnit.Messages.StaticFixtureMethodMustHaveNoParameters
            },
            new object[]
            {
                "ClassWithPrivateStaticFixtureMethod", "TestFromClassWithPrivateStaticFixtureMethod", MyNUnit.ExecutionStatus.Failed, MyNUnit.Messages.StaticFixtureMethodMustBePublicOrInternal
            },
            new object[]
            {
                "ClassWithNonStaticStaticFixtureMethods", "TestFromClassWithNonStaticStaticFixtureMethods", MyNUnit.ExecutionStatus.Failed, MyNUnit.Messages.StaticFixtureMethodMustBeStatic
            }
        };

        private static readonly object[] StaticFixtureMethodTestCases =
        {
            new object[]
            {
                "ClassForTestingStaticFixtureMethodsCorrectSignatures", "TestForBeforeClassTest", MyNUnit.ExecutionStatus.Success, "3"
            },
            new object[]
            {
                "ClassForTestingSeveralStaticFixtureMethodsWork", "TestForBeforeClassTest", MyNUnit.ExecutionStatus.Success, "3"
            }
        };

        private static readonly object[] FixtureMethodTestCases =
        {
            new object[]
            {
                "ClassForTestingFixtureMethodsCorrectSignatures", "TestForBeforeTest", MyNUnit.ExecutionStatus.Success, "3"
            },
            new object[]
            {
                "ClassForTestingSeveralFixtureMetodsWork", "TestForBeforeTest", MyNUnit.ExecutionStatus.Success, "3"
            }
        };
    }
}