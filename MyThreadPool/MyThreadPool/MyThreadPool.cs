using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MyThreadPool
{
    public class MyThreadPool
    {
        private readonly ConcurrentQueue<Action> tasks;

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
                // consume
                tasks.TryDequeue(out Action action);
                action.Invoke();
            }
        }

        // produce
        public IMyTask<TResult> QueueWorkItem<TResult>(Func<TResult> func)
        {
            var task = new MyTask<TResult>(func);
            tasks.Enqueue(() => task.Run());
            return task;
        }

        public void Shutdown()
        {
            throw new NotImplementedException();
        }
    }
}
