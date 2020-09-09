namespace ParallelAndSynchronousMatrixMultiplication.Tests
{
    /// <summary>
    /// Provides functions on matrices that used in tests
    /// </summary>
    public static class FunctionsOnMatrices
    {
        /// <summary>
        /// Compares the corresponding elements of matrices for equality
        /// </summary>
        /// <param name="matrix1">First matrix</param>
        /// <param name="matrix2">Second matrix</param>
        /// <returns>True, if corresponding elements of matrices are equal; Otherwise, false</returns>
        public static bool AreEqual(int[,] matrix1, int[,] matrix2)
        {
            if (matrix1.GetLength(0) != matrix2.GetLength(0)
                || matrix1.GetLength(1) != matrix2.GetLength(1))
            {
                return false;
            }

            for (var i = 0; i < matrix1.GetLength(0); ++i)
            {
                for (var j = 0; j < matrix1.GetLength(1); ++j)
                {
                    if (matrix1[i, j] != matrix2[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }
    }
}
