using NUnit.Framework;

namespace ParallelAndSynchronousMatrixMultiplication.Tests
{
    class ParallelMatrixMultiplierTests
    {
        private ParallelMatrixMultiplier multiplier;

        [SetUp]
        public void Setup()
        {
            multiplier = new ParallelMatrixMultiplier();
        }

        [Test]
        [TestCaseSource(nameof(IsCorrectMultiplyingTestCases))]
        public void IsCorrectMultiplyingTest(int[,] left, int[,] right, int[,] expected,
            int[] numberOfThreads)
        {
            foreach (var threadCount in numberOfThreads)
            {
                multiplier = new ParallelMatrixMultiplier(threadCount);
                var actualResult = multiplier.Multiply(left, right);

                Assert.IsTrue(FunctionsOnMatrices.AreEqual(actualResult, expected));
            }
        }

        private static object[] IsCorrectMultiplyingTestCases =
        {
            new object[]
            {
                Matrices.matrix18, Matrices.matrix19, Matrices.matrix20, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 } 
            },
            new object[]
            {
                Matrices.matrix6, Matrices.matrix7, Matrices.matrix8, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            },
            new object[]
            {
                Matrices.matrix8, Matrices.matrix9, Matrices.matrix10, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            },
            new object[]
            {
                Matrices.matrix11, Matrices.matrix12, Matrices.matrix13, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            },
            new object[]
            {
                Matrices.matrix13, Matrices.matrix14, Matrices.matrix15, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            },
            new object[]
            {
                Matrices.matrix15, Matrices.matrix16, Matrices.matrix17, new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 }
            }
        };
    }
}
