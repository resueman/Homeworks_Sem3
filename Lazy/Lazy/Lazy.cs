using System;

namespace Lazy
{
    /// <summary>
    /// Creates lazy thread-unsafe object
    /// </summary>
    /// <typeparam name="T">The type of value that is being lazily initialized</typeparam>
    class Lazy<T> : ILazy<T>
    {
        private Func<T> supplier;
        private T lazyObject;
        private bool isCreated;

        /// <summary>
        /// Creates instance of Lazy class
        /// </summary>
        /// <param name="supplier">Deferred calculation</param>
        public Lazy(Func<T> supplier) 
        {
            this.supplier = supplier;
        }

        /// <summary>
        /// Returns a lazily initialized value without a guarantee of 
        /// correct work in multithreading program
        /// </summary>
        /// <returns>The first call causes the calculation and returns the result. 
        /// Repeated calls return the same object as the first call</returns>
        public T Get()
        {
            if (!isCreated)
            {
                lazyObject = supplier();
                isCreated = true;
                supplier = null;
            }
            return lazyObject;
        }
    }
}