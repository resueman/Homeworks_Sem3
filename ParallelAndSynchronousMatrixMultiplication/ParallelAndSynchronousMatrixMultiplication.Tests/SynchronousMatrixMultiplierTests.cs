using NUnit.Framework;

namespace ParallelAndSynchronousMatrixMultiplication.Tests
{
    public class SynchronousMatrixMultiplierTests
    {
        private SynchronousMatrixMultiplier syncMultiplier;

        [SetUp]
        public void Setup()
        {
            syncMultiplier = new SynchronousMatrixMultiplier();
        }

        [TestCaseSource(nameof(IsCorrectMultiplyingTestCases))]
        public void IsCorrectMultiplyingTest(int[,] left, int[,] right, int[,] expected, int numberOfTestCase)
        {
            var actualResult = syncMultiplier.Multiply(left, right);

            Assert.IsTrue(FunctionsOnMatrices.AreEqual(actualResult, expected));
        }

        private static object[] IsCorrectMultiplyingTestCases =
        {
            new object[]
            {
                Matrices.matrix18, Matrices.matrix19, Matrices.matrix20, 1
            },
            new object[]
            {
                Matrices.matrix6, Matrices.matrix7, Matrices.matrix8, 2 
            },
            new object[]
            {
                Matrices.matrix8, Matrices.matrix9, Matrices.matrix10, 3
            },
            new object[]
            {
                Matrices.matrix11, Matrices.matrix12, Matrices.matrix13, 4
            },
            new object[]
            {
                Matrices.matrix13, Matrices.matrix14, Matrices.matrix15, 5
            },
            new object[]
            {
                Matrices.matrix15, Matrices.matrix16, Matrices.matrix17, 6
            }
        };
    }
}
