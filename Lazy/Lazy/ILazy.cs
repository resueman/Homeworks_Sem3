namespace Lazy
{
    /// <summary>
    /// Lazy computing interface
    /// </summary>
    /// <typeparam name="T">The type of object that is being lazily initialized</typeparam>
    public interface ILazy<T>
    {
        /// <summary>
        /// Gets the lazily initialized value of the current ILazy<T> instance
        /// </summary>
        /// <returns>The lazily initialized value</returns>
        T Get();
    }
}
