using NUnit.Framework;
using System;

namespace ParallelAndSynchronousMatrixMultiplication.Tests
{
    /// <summary>
    /// Tests for matrix multipliers
    /// </summary>
    /// <typeparam name="T">Type of matrix multiplier</typeparam>
    [TestFixture(typeof(SynchronousMatrixMultiplier))]
    [TestFixture(typeof(ParallelForMatrixMultiplier))]
    [TestFixture(typeof(StripedParallelizingMatrixMultiplier))]
    [TestFixture(typeof(SequentiallyParallelizingMatrixMultiplier))]
    class MatrixMultipliersTests<T> where T : IMatrixMultiplier, new()
    {
        private IMatrixMultiplier multiplier;

        public MatrixMultipliersTests()
        {
            multiplier = new T();
        }

        [Test]
        [TestCaseSource(typeof(TestCasesInputData), "IsCorrectMultiplyingTestCases")]
        public void IsCorrectMultiplyingTest(int[,] left, int[,] right, int[,] expected, int numberOfTestCase)
        {
            var threads = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 100 };
            if (typeof(T) == typeof(StripedParallelizingMatrixMultiplier) 
                || typeof(T) == typeof(SequentiallyParallelizingMatrixMultiplier))
            {
                foreach (var thread in threads)
                {
                    multiplier = typeof(T) == typeof(StripedParallelizingMatrixMultiplier)
                        ? new StripedParallelizingMatrixMultiplier(thread)
                        : new SequentiallyParallelizingMatrixMultiplier(thread) as IMatrixMultiplier;

                    var result = multiplier.Multiply(left, right);

                    Assert.IsTrue(FunctionsOnMatrices.AreEqual(result, expected));
                }

                return;
            }

            var actualResult = multiplier.Multiply(left, right);

            Assert.IsTrue(FunctionsOnMatrices.AreEqual(actualResult, expected));
        }

        [Test]
        [TestCaseSource(typeof(TestCasesInputData), "ImpossibleMultiplicationTestCases")]
        public void ImpossibleMultiplicationTest(int[,] left, int[,] right, int numberOfTestCase)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => multiplier.Multiply(left, right));
        }
    }
}
