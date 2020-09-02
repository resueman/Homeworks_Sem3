using System;

namespace ParallelAndSynchronousMatrixMultiplication
{
    public class SynchronousMatrixMultiplier : IMatrixMultiplier
    {
        public int[,] Multiply(int[,] left, int[,] right)
        {
            if (left.GetLength(1) != right.GetLength(0))
            {
                throw new ArgumentOutOfRangeException("Number of columns of the left matrix isn't equal " +
                    "to the number of rows of the right matrix"); 
            }

            var commonLength = left.GetLength(1);
            var leftLinesCount = left.GetLength(0);
            var rightColumnsCount = right.GetLength(1);
            var matrixProduct = new int[leftLinesCount, rightColumnsCount];
            for (var i = 0; i < leftLinesCount; ++i)
            {
                for (var j = 0; j < rightColumnsCount; ++j)
                {
                    for (var k = 0; k < commonLength; ++k)
                    {
                        matrixProduct[i, j] += left[i, k] * right[k, j];
                    }
                }
            }

            return matrixProduct;
        }
    }
}
