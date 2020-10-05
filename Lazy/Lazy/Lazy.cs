using System;

namespace Lazy
{
    /// <summary>
    /// Creates thread-unsafe lazy object
    /// </summary>
    /// <typeparam name="T">The type of value that is being lazily initialized</typeparam>
    public class Lazy<T> : ILazy<T>
    {
        private Func<T> supplier;
        private T lazyObject;

        /// <summary>
        /// Creates instance of thread-unsafe Lazy class
        /// </summary>
        /// <param name="supplier">Deferred calculation</param>
        public Lazy(Func<T> supplier) 
        {
            this.supplier = supplier ?? throw new ArgumentNullException("Supplier can't be null");
        }

        /// <summary>
        /// Returns a lazily initialized value without a guarantee of 
        /// correct work in a multithreading program
        /// </summary>
        /// <returns>The first call causes the calculation and returns the result. 
        /// Next calls return the same object as the first call</returns>
        public T Get()
        {
            if (supplier != null)
            {
                lazyObject = supplier();
                supplier = null;
            }
            return lazyObject;
        }
    }
}