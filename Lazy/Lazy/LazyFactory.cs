using System;

namespace Lazy
{
    /// <summary>
    /// Allows to create thread-safe and thread-unsafe Lazy object, 
    /// which allows to postpone creation or calculation of value
    /// </summary>
    /// <typeparam name="T">The type of value that will be lazily initialized</typeparam>
    public class LazyFactory<T>
    {
        /// <summary>
        /// Creates instance of thread-unsafe Lazy
        /// </summary>
        /// <param name="supplier">Deferred calculation</param>
        /// <returns>Thread-unsafe lazy object</returns>
        public static ILazy<T> CreateLazy(Func<T> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException("Supplier can't be null");
            }
            return new Lazy<T>(supplier);
        }

        /// <summary>
        /// Creates instance of thread-safe Lazy
        /// </summary>
        /// <param name="supplier">Deferred calculation</param>
        /// <returns>Thread-safe lazy object</returns>
        public static ILazy<T> CreateConcurrentLazy(Func<T> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException("Supplier can't be null");
            }
            return new ConcurrentLazy<T>(supplier);
        }
    }
}
