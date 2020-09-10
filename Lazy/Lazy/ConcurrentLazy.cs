using System;

namespace Lazy
{
    class ConcurrentLazy<T> : ILazy<T>
    {
        private Func<T> supplier;
        private T evaluated;
        private readonly object locker;
        private volatile bool isCreated;

        public ConcurrentLazy(Func<T> supplier)
        {
            this.supplier = supplier;
        }

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
