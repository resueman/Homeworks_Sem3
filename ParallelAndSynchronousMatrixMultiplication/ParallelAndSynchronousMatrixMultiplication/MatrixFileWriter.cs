using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace ParallelAndSynchronousMatrixMultiplication
{
    public class MatrixFileWriter
    {
        public async Task Write(int[,] matrix, string path)
        {
            using var streamWriter = new StreamWriter(path, false, Encoding.Default);
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
