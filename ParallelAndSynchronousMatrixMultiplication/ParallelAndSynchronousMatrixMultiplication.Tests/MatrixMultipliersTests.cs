using NUnit.Framework;
using System;

namespace ParallelAndSynchronousMatrixMultiplication.Tests
{
    [TestFixture(typeof(SynchronousMatrixMultiplier))]
    [TestFixture(typeof(ParallelMatrixMultiplier))]
    class MatrixMultipliersTests<T> where T : IMatrixMultiplier, new()
    {
        private IMatrixMultiplier multiplier;
        private readonly bool isSynchronousMultiplier;
        private readonly int[] numberOfThreads;

        public MatrixMultipliersTests()
        {
            multiplier = new T();
            numberOfThreads = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9 };
            isSynchronousMultiplier = typeof(T) == typeof(SynchronousMatrixMultiplier);
        }

        [Test]
        [TestCaseSource(nameof(IsCorrectMultiplyingTestCases))]
        public void IsCorrectMultiplyingTest(int[,] left, int[,] right, int[,] expected, int numberOfTestCase)
        {
            if (isSynchronousMultiplier)
            {
                var actualResult = multiplier.Multiply(left, right);
                Assert.IsTrue(FunctionsOnMatrices.AreEqual(actualResult, expected));                
                return;
            }

            foreach (var threadCount in numberOfThreads)
            { 
                multiplier = new ParallelMatrixMultiplier(threadCount);
                var actualResult = multiplier.Multiply(left, right);

                Assert.IsTrue(FunctionsOnMatrices.AreEqual(actualResult, expected));
            }
        }

        [Test]
        [TestCaseSource(nameof(ImpossibleMultiplicationTestCases))]
        public void ImpossibleMultiplicationTest(int[,] left, int[,] right, int numberOfTestCase)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => multiplier.Multiply(left, right));
        }

        private static readonly object[] IsCorrectMultiplyingTestCases =
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
            },
            new object[]
            {
                Matrices.matrix21, Matrices.matrix21, Matrices.matrix21, 7
            },
            new object[]
            {
                Matrices.matrix21, Matrices.matrix17, Matrices.matrix17, 8
            }
        };

        private static readonly object[] ImpossibleMultiplicationTestCases =
        {
            new object[]
            {
                Matrices.matrix1, Matrices.matrix2, 1
            },
            new object[]
            {
                Matrices.matrix2, Matrices.matrix1, 2
            },
            new object[]
            {
                Matrices.matrix16, Matrices.matrix20, 3
            },
            new object[]
            {
                Matrices.matrix16, Matrices.matrix17, 4
            },
            new object[]
            {
                Matrices.matrix17, Matrices.matrix16, 5
            },
            new object[]
            {
                Matrices.matrix14, Matrices.matrix13, 6
            },
            new object[]
            {
                Matrices.matrix10, Matrices.matrix11, 7
            },
            new object[]
            {
                Matrices.matrix17, Matrices.matrix21, 8
            }
        };
    }
}
