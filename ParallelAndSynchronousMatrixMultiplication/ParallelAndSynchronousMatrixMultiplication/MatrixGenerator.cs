using System;

namespace ParallelAndSynchronousMatrixMultiplication
{
    /// <summary>
    /// Generates matrix values
    /// </summary>
    public class MatrixGenerator
    {
        /// <summary>
        /// Generates matrix values of specified dimensions
        /// </summary>
        /// <param name="lines">Number of lines in generating matrix</param>
        /// <param name="columns">Number of columns in generating matrix</param>
        /// <returns>Matrix of integers</returns>
        public int[,] Generate(int lines, int columns)
        {
            if (lines < 1 || columns < 1)
            {
                throw new ArgumentOutOfRangeException("Numbers of lines and columns must be positive");
            }

            var matrix = new int[lines, columns];
            var random = new Random();
            for (var i = 0; i < lines; ++i)
            {
                for (var j = 0; j < columns; ++j)
                {
                    matrix[i, j] = random.Next(1, 9);
                }
            }

            return matrix;
        }
    }
}
