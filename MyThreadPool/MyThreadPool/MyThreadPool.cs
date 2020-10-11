using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MyThreadPool
{
    public class MyThreadPool
    {
        private int activeThreads;
        private object locker;
        private AutoResetEvent resetEvent;
        private CancellationTokenSource cancellationTokenSource;
        private readonly BlockingCollection<Action> tasks;

        public MyThreadPool(int threadCount)
        {
            if (threadCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(threadCount), "Number of threads should be a positive number");
            }

            activeThreads = threadCount;
            locker = new object();
            tasks = new BlockingCollection<Action>();
            resetEvent = new AutoResetEvent(false);
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
            if (cancellationTokenSource.IsCancellationRequested)
            {
                throw new ThreadPoolWasShuttedDownException("Impossible to add task");
            }

            var task = new MyTask<TResult>(func, this);
            tasks.Add(() => task.Run());
            Monitor.Pulse(locker);
            return task;
        }

        public void Shutdown()
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                throw new ThreadPoolWasShuttedDownException("Impossible to shut down more than once");
            }

            cancellationTokenSource.Cancel();
            Monitor.PulseAll(locker);            
            resetEvent.WaitOne();
        }
    }
}
