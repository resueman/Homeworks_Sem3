using System;

namespace ParallelAndSynchronousMatrixMultiplication
{
    public class MatrixGenerator
    {
        public int[,] Generate(int lines, int columns)
        {
            var matrix = new int[lines, columns];
            var random = new Random();
            for (var i = 0; i < lines; ++i)
            {
                for (var j = 0; j < columns; ++j)
                {
                    matrix[i, j] = random.Next();
                }
            }

            return matrix;
        }
    }
}
