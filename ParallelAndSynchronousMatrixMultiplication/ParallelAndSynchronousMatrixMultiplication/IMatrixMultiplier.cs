namespace ParallelAndSynchronousMatrixMultiplication
{
    /// <summary>
    /// Describes what every matrix multiplier should be able to do
    /// </summary>
    public interface IMatrixMultiplier 
    {
        /// <summary>
        /// Multiplies two matrices
        /// </summary>
        /// <param name="left">Left matrix factor</param>
        /// <param name="right">Right matrix factor</param>
        /// <returns>New matrix - result of matrix multiplication</returns>
        int[,] Multiply(int[,] left, int[,] right);
    }
}
