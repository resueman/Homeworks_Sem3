namespace ParallelAndSynchronousMatrixMultiplication
{
    public interface IMatrixMultiplier
    {
        int[,] MultiplyMatrices(int[,] matrix1, int[,] matrix2);
    }
}
