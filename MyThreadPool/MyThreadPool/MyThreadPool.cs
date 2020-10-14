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
            private readonly ManualResetEvent isCompletedResetEvent;
            private readonly ConcurrentQueue<Action> continuations;

            /// <summary>
            /// Creates instance of asynchronus operation
            /// </summary>
            /// <param name="supplier">Function which represents a task</param>
            /// <param name="threadPool">Thread pool which will perform task</param>
            public MyTask(Func<TResult> supplier, MyThreadPool threadPool)
            {
                this.supplier = supplier ?? throw new ArgumentNullException(nameof(supplier));
                this.threadPool = threadPool;
                isCompletedResetEvent = new ManualResetEvent(false);
                continuations = new ConcurrentQueue<Action>();
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
                if (continuation == null)
                {
                    throw new ArgumentNullException(nameof(continuation));
                }

                var task = new MyTask<TNewResult>(() => continuation(Result), threadPool);
                if (IsCompleted)
                {
                    threadPool.AddTask(() => task.Run());
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
                        if (!continuations.TryDequeue(out Action action))
                        {
                            continue;
                        }
                        threadPool.AddTask(action);
                        lock (threadPool.locker)
                        {
                            Monitor.Pulse(threadPool.locker);
                        }
                    }
                }
            }
        }

        private readonly object locker;
        private readonly BlockingCollection<Action> tasks;
        private readonly AutoResetEvent areAllThreadsTerminatedResetEvent;
        private readonly CancellationTokenSource cancellationTokenSource;

        /// <summary>
        /// Number of running threads
        /// </summary>
        public int ActiveThreads { get; private set; }

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

            ActiveThreads = threadCount;
            locker = new object();
            tasks = new BlockingCollection<Action>();
            areAllThreadsTerminatedResetEvent = new AutoResetEvent(false);
            cancellationTokenSource = new CancellationTokenSource();
            
            StartThreads();
        }

        private void StartThreads()
        {
            var threads = new Thread[ActiveThreads];
            for (var i = 0; i < threads.Length; ++i)
            {
                threads[i] = new Thread(() => DequeueTask())
                {
                    Name = "Thread Pool Thread",
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
                            --ActiveThreads;
                            if (ActiveThreads == 0)
                            {
                                areAllThreadsTerminatedResetEvent.Set();
                            }
                            return;
                        }
                        Monitor.Wait(locker);
                    }
                }
                tasks.TryTake(out Action task);
                task?.Invoke();
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
            AddTask(() => task.Run());

            return task;
        }

        private void AddTask(Action action)
        {
            try
            {
                tasks.Add(action, cancellationTokenSource.Token);
                lock (locker)
                {
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
