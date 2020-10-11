using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MyThreadPool
{
    class MyTask<TResult> : IMyTask<TResult>
    {
        private Func<TResult> supplier;
        private readonly MyThreadPool threadPool;
        private ConcurrentQueue<Action> continuationTasks;
        private TResult result;
        private AggregateException aggregateException;
        private ManualResetEvent resetEvent;
        
        public MyTask(Func<TResult> supplier, MyThreadPool threadPool)
        {
            this.supplier = supplier;
            continuationTasks = new ConcurrentQueue<Action>();
            resetEvent = new ManualResetEvent(false);
            this.threadPool = threadPool;
            IsCompleted = false;
        }

        public bool IsCompleted { get; set; } = false;
        
        public TResult Result 
        {
            get
            {
                resetEvent.WaitOne();
                if (aggregateException != null)
                {
                    throw aggregateException;
                }
                return result;
            }
            set
            {
                result = value;
            }
        }

        public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
        {
            // ???
            return threadPool.QueueWorkItem(() => continuation(Result));
        }

        public void Run()
        {
            try
            {
                Result = supplier.Invoke();
                IsCompleted = true;
            }
            catch (Exception e)
            {
                aggregateException = new AggregateException(e);
            }
            finally
            {
                resetEvent.Set();
                supplier = null;
            }
        }
    }
}
