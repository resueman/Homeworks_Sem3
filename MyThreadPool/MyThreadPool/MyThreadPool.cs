using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MyThreadPool
{
    public class MyThreadPool
    {
        private CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        private ManualResetEvent resetEvent = new ManualResetEvent(true);
        private readonly BlockingCollection<Action> tasks = new BlockingCollection<Action>();
        private object locker = new object(); 

        public MyThreadPool(int threadCount)
        {
            if (threadCount < 1)
            {
                throw new ArgumentOutOfRangeException("Number of thread should be a positive number");
            }

            var threads = new Thread[threadCount];
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
                throw new Exception("Impossible to add task, because ThreadPool was shouted down");
            }

            var task = new MyTask<TResult>(func, this);
            tasks.Add(() => task.Run());
            Monitor.Pulse(locker);
            return task;
        }

        public void Shutdown()
        {
            cancellationTokenSource.Cancel();
        }
    }
}
