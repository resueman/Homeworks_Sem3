using System;
using System.Collections.Generic;
using System.Threading;

namespace ParallelAndSynchronousMatrixMultiplication
{
    /// <summary>
    /// Performs parallel matrix multiplication using striped parallelizing algorithm
    /// </summary>
    public class StripedParallelizingMatrixMultiplier : IMatrixMultiplier
    {
        private readonly int threadCount;

        /// <summary>
        /// Creates instance of multiplier
        /// </summary>
        public StripedParallelizingMatrixMultiplier()
        {
            threadCount = Environment.ProcessorCount;
        }

        /// <summary>
        /// Creates instance of multiplier with specified number of using threads
        /// </summary>
        /// <param name="threadCount">Number of threads</param>
        public StripedParallelizingMatrixMultiplier(int threadCount)
        {
            if (threadCount < 0)
            {
                throw new ArgumentOutOfRangeException("Number of threads shouls be positive");
            }
            this.threadCount = threadCount;
        }

        /// <summary>
        /// Evenly distributes line-column multiplying pairs between threads
        /// </summary>
        /// <param name="leftLinesCount">Lines of left matrix factor</param>
        /// <param name="rightColumnsCount">Columns of right matrix factor</param>
        /// <returns>Array of line-column pairs for each thread</returns>
        private List<(int Line, int Column)>[] DistributeTasks(int leftLinesCount, int rightColumnsCount)
        {
            var productCellsCount = leftLinesCount * rightColumnsCount;
            var numberOfActiveThreads = threadCount > productCellsCount ? productCellsCount : threadCount;
            var lineColumnPairs = new List<(int Line, int Column)>[numberOfActiveThreads];
            for (var i = 0; i < numberOfActiveThreads; ++i)
            {
                lineColumnPairs[i] = new List<(int Line, int Column)>();
            }

            var threadNumber = 0;
            for (var i = 0; i < leftLinesCount; ++i)
            {
                for (var j = 0; j < rightColumnsCount; ++j)
                {
                    lineColumnPairs[threadNumber].Add((i, j));
                    threadNumber = (threadNumber + 1) % threadCount;
                }
            }

            return lineColumnPairs;
        }

        /// <summary>
        /// Multiplies matrices using striped parallelizing algorithm
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

            var lineColumnPairs = DistributeTasks(left.GetLength(0), right.GetLength(1));
            var threads = new Thread[lineColumnPairs.Length];
            var matrixProduct = new int[left.GetLength(0), right.GetLength(1)];
            for (var i = 0; i < threads.Length; ++i)
            {
                var threadNumber = i;
                threads[i] = new Thread(() =>
                {
                    foreach (var (i, j) in lineColumnPairs[threadNumber])
                    {
                        for (var k = 0; k < left.GetLength(1); ++k)
                        {
                            matrixProduct[i, j] += left[i, k] * right[k, j];
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
