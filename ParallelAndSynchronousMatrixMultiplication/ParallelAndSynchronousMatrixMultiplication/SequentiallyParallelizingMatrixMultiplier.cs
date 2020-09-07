using System;
using System.Threading;

namespace ParallelAndSynchronousMatrixMultiplication
{
    /// <summary>
    /// Performs parallel matrix multiplication using sequential parallelizing algorithm
    /// </summary>
    public class SequentiallyParallelizingMatrixMultiplier : IMatrixMultiplier
    {
        private readonly int threadCount;

        /// <summary>
        /// Creates instance of multiplier
        /// </summary>
        public SequentiallyParallelizingMatrixMultiplier()
        {
            threadCount = Environment.ProcessorCount;
        }

        /// <summary>
        /// Creates instance of multiplier with specified number of using threads
        /// </summary>
        /// <param name="threadCount">Number of threads</param>
        public SequentiallyParallelizingMatrixMultiplier(int threadCount)
        {
            if (threadCount < 0)
            {
                throw new ArgumentOutOfRangeException("Number of threads shouls be positive");
            }
            this.threadCount = threadCount;
        }

        /// <summary>
        /// Multiplies matrices using sequential parallelizing algorithm
        /// </summary>
        /// <param name="left">Left matrix factor</param>
        /// <param name="right">Right matrix factor</param>
        /// <returns>Result of multiplication</returns>
        public int[,] Multiply(int[,] left, int[,] right)
        {
            if (left.GetLength(1) != right.GetLength(0))
            {
                throw new ArgumentOutOfRangeException("Number of columns of the left matrix isn't equal " +
                    "to the number of rows of the right matrix");
            }

            var leftLinesCount = left.GetLength(0);
            var rightColumnsCount = right.GetLength(1);
            var numberOfActiveThreads = threadCount > leftLinesCount 
                ? leftLinesCount 
                : threadCount;

            var chunkSize = numberOfActiveThreads > leftLinesCount
                ? 1
                : (numberOfActiveThreads > 1) 
                    ? leftLinesCount / (numberOfActiveThreads - 1)
                    : leftLinesCount;

            var threads = new Thread[numberOfActiveThreads];
            var matrixProduct = new int[leftLinesCount, rightColumnsCount];
            for (var i = 0; i < threads.Length; ++i)
            {
                var threadNumber = i;
                threads[i] = new Thread(() =>
                {
                    for (var i = chunkSize * threadNumber; 
                        i < chunkSize * (threadNumber + 1) && i < leftLinesCount; ++i)
                    {
                        for (int j = 0; j < rightColumnsCount; ++j)
                        {
                            for (var k = 0; k < left.GetLength(1); ++k)
                            {
                                matrixProduct[i, j] += left[i, k] * right[k, j];
                            }
                        }
                    }
                });
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }

            foreach (var thread in threads)
            {
                thread.Join();
            }

            return matrixProduct;
        }
    }
}
