using System;
using System.Threading;

namespace ParallelAndSynchronousMatrixMultiplication
{
    public class SequentiallyParallelizingMatrixMultiplier : IMatrixMultiplier
    {
        private readonly int threadCount;

        public SequentiallyParallelizingMatrixMultiplier()
        {
            threadCount = Environment.ProcessorCount;
        }

        public SequentiallyParallelizingMatrixMultiplier(int threadCount)
        {
            if (threadCount < 0)
            {
                throw new ArgumentOutOfRangeException("Number of threads shouls be positive");
            }
            this.threadCount = threadCount;
        }

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
