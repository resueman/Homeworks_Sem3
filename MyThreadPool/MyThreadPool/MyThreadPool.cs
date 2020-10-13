using System;
using System.Collections.Concurrent;
using System.Threading;

namespace MyThreadPool
{
    /// <summary>
    /// Provides pool with fixed number of threads that can be used to execute tasks
    /// </summary>
    public class MyThreadPool : IDisposable
    {
        /// <summary>
        /// Represents an asynchronous operation
        /// </summary>
        /// <typeparam name="TResult">Type of asynchronous operation result</typeparam>
        class MyTask<TResult> : IMyTask<TResult>
        {
            private TResult result;
            private Func<TResult> supplier;
            private readonly MyThreadPool threadPool;
            private AggregateException aggregateException;
            private ManualResetEvent isCompletedResetEvent;
            private ConcurrentQueue<Action> continuations;

            /// <summary>
            /// Creates instance of asynchronus operation
            /// </summary>
            /// <param name="supplier">Function which represents a task</param>
            /// <param name="threadPool">Thread pool which will perform task</param>
            public MyTask(Func<TResult> supplier, MyThreadPool threadPool)
            {
                this.supplier = supplier;
                isCompletedResetEvent = new ManualResetEvent(false);
                this.threadPool = threadPool;
                continuations = new ConcurrentQueue<Action>();
                IsCompleted = false;
            }

            /// <summary>
            /// Determines whether the task is completed
            /// </summary>
            public bool IsCompleted { get; private set; } 

            /// <summary>
            /// Returns result of task evaluation
            /// </summary>
            public TResult Result
            {
                get
                {
                    isCompletedResetEvent.WaitOne();
                    if (aggregateException != null)
                    {
                        throw aggregateException;
                    }
                    return result;
                }
                private set
                {
                    result = value;
                }
            }

            /// <summary>
            /// Creates new task based on result of previous task
            /// </summary>
            /// <typeparam name="TNewResult">Type of new task result</typeparam>
            /// <param name="continuation">Function which will be applied to the result of a given task</param>
            /// <returns>New task for execution</returns>
            public IMyTask<TNewResult> ContinueWith<TNewResult>(Func<TResult, TNewResult> continuation)
            {
                var task = new MyTask<TNewResult>(() => continuation(Result), threadPool);
                if (IsCompleted)
                {
                    threadPool.EnqueueWrappedTask(() => task.Run());
                }
                else
                {
                    continuations.Enqueue(() => task.Run());
                }

                return task;
            }

            /// <summary>
            /// Evaluates task
            /// </summary>
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
                    isCompletedResetEvent.Set();
                    supplier = null;
                    while (continuations.Count != 0)
                    {
                        continuations.TryDequeue(out Action action);
                        threadPool.EnqueueWrappedTask(action); 
                        Monitor.Pulse(threadPool.locker);
                    }
                }
            }
        }

        private int activeThreads;
        private readonly object locker;
        private readonly BlockingCollection<Action> tasks;
        private readonly AutoResetEvent areAllThreadsTerminatedResetEvent;
        private readonly CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Creates instance of MyThreadPool class
        /// </summary>
        /// <param name="threadCount">Number of threads in pool</param>
        public MyThreadPool(int threadCount)
        {
            if (threadCount < 1)
            {
                throw new ArgumentOutOfRangeException(nameof(threadCount), "Number of threads should be a positive number");
            }

            activeThreads = threadCount;
            locker = new object();
            tasks = new BlockingCollection<Action>();
            areAllThreadsTerminatedResetEvent = new AutoResetEvent(false);
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
                                areAllThreadsTerminatedResetEvent.Set();
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

        /// <summary>
        /// Queues a method for execution. Method will be executed on first available thread
        /// </summary>
        /// <typeparam name="TResult">Method return type</typeparam>
        /// <param name="func">Method for asynchronous execution</param>
        /// <returns>Task that will be executed on first available thread</returns>
        public IMyTask<TResult> QueueWorkItem<TResult>(Func<TResult> func) 
        {
            var task = new MyTask<TResult>(func, this);
            EnqueueWrappedTask(() => task.Run());

            return task;
        }

        private void EnqueueWrappedTask(Action action)
        {
            try
            {
                lock (locker)
                {
                    tasks.Add(action, cancellationTokenSource.Token);
                    Monitor.Pulse(locker);
                }
            }
            catch (Exception e)
            when (e is OperationCanceledException
                || e is ObjectDisposedException
                || e is InvalidOperationException)
            {
                throw new ThreadPoolWasShuttedDownException("Impossible to add task, thread pool was shutted down");
            }
        }

        /// <summary>
        /// Terminates threads, but before lets all tasks that have already entered the queue be counted
        /// </summary>
        public void Shutdown()
        {
            if (cancellationTokenSource.IsCancellationRequested)
            {
                throw new ThreadPoolWasShuttedDownException("Impossible to shut thread pool down more than once");
            }

            tasks.CompleteAdding();
            cancellationTokenSource.Cancel();
            lock (locker)
            {
                Monitor.PulseAll(locker);
            }
            
            areAllThreadsTerminatedResetEvent.WaitOne();
            Dispose();
        }

        /// <summary>
        /// Releases resourses
        /// </summary>
        public void Dispose()
        {
            areAllThreadsTerminatedResetEvent.Dispose();
            cancellationTokenSource.Dispose();
            tasks.Dispose();
        }
    }
}
