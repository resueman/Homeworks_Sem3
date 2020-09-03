namespace ParallelAndSynchronousMatrixMultiplication.Tests
{
    public static class FunctionsOnMatrices
    {
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
