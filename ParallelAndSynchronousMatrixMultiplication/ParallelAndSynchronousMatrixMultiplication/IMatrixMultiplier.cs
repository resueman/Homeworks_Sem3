namespace ParallelAndSynchronousMatrixMultiplication
{
    /// <summary>
    /// Describes what every matrix multiplier should be able to do
    /// </summary>
    public interface IMatrixMultiplier 
    {
        int[,] Multiply(int[,] matrix1, int[,] matrix2);
    }
}
