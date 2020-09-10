using System;

namespace Lazy
{
    class Lazy<T> : ILazy<T>
    {
        private Func<T> supplier;
        private T lazyObject;
        private bool isCreated;

        public Lazy(Func<T> supplier) 
        {
            this.supplier = supplier;
        }

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