using System;
using System.Collections.Generic;
using System.Threading;

namespace ParallelAndSynchronousMatrixMultiplication
{
    public class StripedParallelizingMatrixMultiplier : IMatrixMultiplier
    {
        private readonly int threadCount;

        public StripedParallelizingMatrixMultiplier()
        {
            if (threadCount < 0)
            {
                throw new ArgumentOutOfRangeException("Number of threads shouls be positive");
            }
            threadCount = Environment.ProcessorCount;
        }

        public StripedParallelizingMatrixMultiplier(int threadCount)
        {
            this.threadCount = threadCount;
        }

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
