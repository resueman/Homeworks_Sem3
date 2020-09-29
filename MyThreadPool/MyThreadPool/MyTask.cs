using System;
using System.Collections.Generic;
using System.Text;

namespace MyThreadPool
{
    class MyTask<TResult> : IMyTask<TResult>
    {
        private Func<TResult> supplier;

        public MyTask(Func<TResult> supplier)
        {
            this.supplier = supplier;
            IsCompleted = false;
        }

        public bool IsCompleted { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
        public TResult Result { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
        {
            throw new NotImplementedException();
        }

        public void Run()
        {
            supplier.Invoke();
        }
    }
}
