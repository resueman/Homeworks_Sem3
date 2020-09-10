using System;

namespace Lazy
{
    public class LazyFactory<T>
    {
        public static ILazy<T> CreateLazy(Func<T> supplier)
        {
            if (supplier == null)
            {
                throw new ArgumentNullException("Supplier can't be null");
            }
            return new Lazy<T>(supplier);
        }

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
