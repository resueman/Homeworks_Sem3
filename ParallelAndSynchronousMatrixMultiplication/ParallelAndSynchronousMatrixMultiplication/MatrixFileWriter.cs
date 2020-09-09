using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ParallelAndSynchronousMatrixMultiplication
{
    /// <summary>
    /// Writes matrix to file
    /// </summary>
    public class MatrixFileWriter
    {
        /// <summary>
        /// Asynchronously writes matrix to file
        /// </summary>
        /// <param name="matrix">Matrix of integers</param>
        /// <param name="path">Path to file where matrix will be written</param>
        /// <returns>The task that represents the asynchronous write operation</returns>
        public async Task WriteAsync(int[,] matrix, string path)
        {
            using var streamWriter = new StreamWriter(path, false, Encoding.Default);
            if (matrix == null)
            {
                return;
            }
            for (var i = 0; i < matrix.GetLength(0); ++i)
            {
                for (var j = 0; j < matrix.GetLength(1); ++j)
                {
                    await streamWriter.WriteAsync(matrix[i, j].ToString() + " ");
                }
                await streamWriter.WriteAsync('\n');
            }
        }
    }
}
