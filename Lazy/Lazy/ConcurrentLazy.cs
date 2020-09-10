using System;

namespace Lazy
{
    /// <summary>
    /// Creates lazy thread-safe object
    /// </summary>
    /// <typeparam name="T">The type of value that is being lazily initialized</typeparam>
    class ConcurrentLazy<T> : ILazy<T>
    {
        private T evaluated;
        private Func<T> supplier;
        private readonly object locker;
        private volatile bool isCreated;

        /// <summary>
        /// Creates instance of ConcurrentLazy class
        /// </summary>
        /// <param name="supplier">Deferred calculation</param>
        public ConcurrentLazy(Func<T> supplier)
        {
            this.supplier = supplier;
            locker = new object();
            isCreated = false;
        }

        /// <summary>
        /// Returns a lazily initialized value with a guarantee of 
        /// correct work in multithreading program
        /// </summary>
        /// <returns>The first call causes the calculation and returns the result. 
        /// Repeated calls return the same object as the first call</returns>
        public T Get()
        {
            if (isCreated)
            {
                return evaluated;
            }

            lock (locker)
            {
                if (!isCreated)
                {
                    evaluated = supplier();
                    supplier = null;
                    isCreated = true;
                }
            }

            return evaluated;
        }
    }
}
