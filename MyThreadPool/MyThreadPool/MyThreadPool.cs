using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MyThreadPool
{
    public class MyThreadPool : IDisposable
    {
        class MyTask<TResult> : IMyTask<TResult>
        {
            private TResult result;
            private Func<TResult> supplier;
            private readonly MyThreadPool threadPool;
            private AggregateException aggregateException;
            private ManualResetEvent resetEvent;
            private ConcurrentQueue<Action> continuations;

            public MyTask(Func<TResult> supplier, MyThreadPool threadPool)
            {
                this.supplier = supplier;
                resetEvent = new ManualResetEvent(false);
                this.threadPool = threadPool;
                continuations = new ConcurrentQueue<Action>();
                IsCompleted = false;
            }

            public bool IsCompleted { get; set; } 

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
                var task = new MyTask<TNewResult>(() => continuation(Result), threadPool);
                continuations.Enqueue(() => task.Run());
                return task;
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
                    while (continuations.Count != 0)
                    {
                        continuations.TryDequeue(out Action action);
                        threadPool.tasks.Add(action, threadPool.cancellationTokenSource.Token);
                        Monitor.Pulse(threadPool.locker);
                    }
                }
            }
        }

        private int activeThreads;
        private readonly object locker;
        private readonly AutoResetEvent resetEvent;
        private readonly BlockingCollection<Action> tasks;
        private readonly CancellationTokenSource cancellationTokenSource;

        public MyThreadPool(int threadCount)
        {
            if (threadCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(threadCount), "Number of threads should be a positive number");
            }

            activeThreads = threadCount;
            locker = new object();
            resetEvent = new AutoResetEvent(false);
            tasks = new BlockingCollection<Action>();
            cancellationTokenSource = new CancellationTokenSource();
            
            StartThreads();
        }

        private void StartThreads()
        {
            var threads = new Thread[activeThreads];
            for (var i = 0; i < threads.Length; ++i)
            {
                threads[i] = new Thread(() => DequeueTask())
                {
                    IsBackground = true
                };
                threads[i].Start();
            }
        }

        private void DequeueTask()
        {
            while (true)
            {
                lock (locker)
                {
                    while (tasks.Count == 0)
                    {
                        if (cancellationTokenSource.IsCancellationRequested)
                        {
                            --activeThreads;
                            if (activeThreads == 0)
                            {
                                resetEvent.Set();
                            }
                            return;
                        }
                        Monitor.Wait(locker);
                    }
                }
                var task = tasks.Take();
                task.Invoke();
            }
        }

        public IMyTask<TResult> QueueWorkItem<TResult>(Func<TResult> func)
        {
            var task = new MyTask<TResult>(func, this);
            try
            {
                tasks.Add(() => task.Run(), cancellationTokenSource.Token);
                Monitor.Pulse(locker);
            }
            catch (Exception e) 
            when ( e is OperationCanceledException 
                || e is ObjectDisposedException 
                || e is InvalidOperationException)
            {
                throw new ThreadPoolWasShuttedDownException("Impossible to perform task, thread pool was shutted down");
            }

            return task;
        }

        public void Shutdown()
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                throw new ThreadPoolWasShuttedDownException("Impossible to shut thread pool down more than once");
            }

            tasks.CompleteAdding();
            cancellationTokenSource.Cancel();
            Monitor.PulseAll(locker);
            
            resetEvent.WaitOne();
            Dispose();
        }

        public void Dispose()
        {
            resetEvent.Dispose();
            cancellationTokenSource.Dispose();
            tasks.Dispose();
        }
    }
}
