using System;

namespace Lazy
{
    /// <summary>
    /// Creates thread-safe lazy object
    /// </summary>
    /// <typeparam name="T">The type of value that is being lazily initialized</typeparam>
    public class ConcurrentLazy<T> : ILazy<T>
    {
        private T evaluated;
        private volatile Func<T> supplier;
        private readonly object locker;

        /// <summary>
        /// Creates instance of thread-safe ConcurrentLazy class
        /// </summary>
        /// <param name="supplier">Deferred calculation</param>
        public ConcurrentLazy(Func<T> supplier)
        {
            this.supplier = supplier ?? throw new ArgumentNullException("Supplier can't be null");
            locker = new object();
        }

        /// <summary>
        /// Returns a lazily initialized value with a guarantee of 
        /// correct work in a multithreading program
        /// </summary>
        /// <returns>The first call causes the calculation and returns the result. 
        /// Next calls return the same object as the first call</returns>
        public T Get()
        {
            if (supplier == null)
            {
                return evaluated;
            }

            lock (locker)
            {
                if (supplier != null)
                {
                    evaluated = supplier();
                    supplier = null;
                }
            }

            return evaluated;
        }
    }
}
