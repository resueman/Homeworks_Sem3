using System;
using System.Threading.Tasks;

namespace ParallelAndSynchronousMatrixMultiplication
{
    /// <summary>
    /// Perform parallel matrix multiplication using system Parallel.For() method
    /// </summary>
    public class ParallelForMatrixMultiplier : IMatrixMultiplier
    {
        /// <summary>
        /// Perform parallel matrix multiplication
        /// </summary>
        /// <param name="left">Left matrix factor</param>
        /// <param name="right">Right matrix factor</param>
        /// <returns></returns>
        public int[,] Multiply(int[,] left, int[,] right)
        {
            if (left.GetLength(1) != right.GetLength(0))
            {
                throw new ArgumentOutOfRangeException("Number of columns of the left matrix isn't equal " +
                    "to the number of rows of the right matrix");
            }

            var leftLinesCount = left.GetLength(0);
            var rightColumnsCount = right.GetLength(1);
            var matrixProduct = new int[leftLinesCount, rightColumnsCount];
            Parallel.For(0, leftLinesCount, i =>
            {
                for (int j = 0; j < rightColumnsCount; ++j)
                {
                    for (var k = 0; k < left.GetLength(1); ++k)
                    {
                        matrixProduct[i, j] += left[i, k] * right[k, j];
                    }
                }
            });        

            return matrixProduct;
        }
    }
}
