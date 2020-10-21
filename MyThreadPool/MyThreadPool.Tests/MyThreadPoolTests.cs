using NUnit.Framework;
using System;
using System.Collections.Generic;
using System.Threading;

namespace MyThreadPool.Tests
{
    /// <summary>
    /// Tests for pool with threads
    /// </summary>
    public class MyThreadPoolTests
    {
        private int threadCount;
        private List<IMyTask<int>> intTasks;
        private ManualResetEvent manualResetEvent;

        [SetUp]
        public void Setup()
        {
            threadCount = Environment.ProcessorCount + 1;            
            manualResetEvent = new ManualResetEvent(false);
            intTasks = new List<IMyTask<int>>();
        }

        [TearDown]
        public void TearDown()
        {
            manualResetEvent.Dispose();
        }

        [Test]
        public void ThrowsAggregateExceptionTest()
        {
            using var threadPool = new MyThreadPool(threadCount);
            var task0 = threadPool.QueueWorkItem(() => 0);
            var task1 = task0.ContinueWith(x => 13 / x);
            var task2 = task1.ContinueWith(x => x + 1);
            Assert.Throws<AggregateException>(() => _ = task1.Result);
            Assert.Throws<AggregateException>(() => _ = task2.Result);
        }

        [TestCase(-2)]
        [TestCase(-1)]
        [TestCase(0)]
        [Test]
        public void IncorrectThreadCountTest(int threadCount)
        {
            Assert.Throws<ArgumentOutOfRangeException>(() => new MyThreadPool(threadCount));
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(16)]
        [TestCase(20)]
        [Test]
        public void ActualNumberOfThreadsInPoolIsEqualToThreadCountTest(int threadCount)
        {
            var locker = new object();
            var countdownEvent = new CountdownEvent(threadCount - 1);
            using var threadPool = new MyThreadPool(threadCount);         
            for (var i = 0; i < 10000; ++i)
            {
                var task = threadPool.QueueWorkItem(() =>
                {
                    manualResetEvent.WaitOne();
                    lock (locker)
                    {
                        if (countdownEvent.CurrentCount > 0)
                        {
                            countdownEvent.Signal();
                        }
                    }
                    return Thread.CurrentThread.ManagedThreadId;
                });
                intTasks.Add(task);
            }

            for (var i = 0; i < 5; ++i)
            {
                manualResetEvent.Set();
                manualResetEvent.Reset();
                countdownEvent.Wait();
            }
            countdownEvent.Dispose();
            manualResetEvent.Set();

            var threadIds = new HashSet<int>();
            foreach (var task in intTasks)
            {
                threadIds.Add(task.Result);
            }
            Assert.AreEqual(threadCount, threadIds.Count);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(16)]
        [TestCase(20)]
        [Test]
        public void ActiveThreadsPropertyValueIsEqualToThreadCountTest(int threadCount)
        {
            using var threadPool = new MyThreadPool(threadCount);
            Assert.AreEqual(threadCount, threadPool.ActiveThreads);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(10)]
        [TestCase(16)]
        [TestCase(20)]
        [TestCase(101)]
        [Test]
        public void AreAllThreadsTerminatedAfterShutdownTest(int threadCount)
        {
            using var threadPool = new MyThreadPool(threadCount);
            threadPool.Shutdown();
            Assert.AreEqual(0, threadPool.ActiveThreads);
        }

        [Test]
        public void ShutThreadPoolDownMoreThanOnceTest()
        {
            using var threadPool = new MyThreadPool(threadCount);
            threadPool.Shutdown();
            for (var i = 0; i < 100; ++i)
            {
                Assert.Throws<ThreadPoolWasShuttedDownException>(() => threadPool.Shutdown());
            }
        }

        [Test]
        public void ImpossibleToAddATaskToShuttedDownThreadPoolTest()
        {
            using var threadPool = new MyThreadPool(threadCount);
            threadPool.Shutdown();
            for (var i = 0; i < 30; ++i)
            {
                Assert.Throws<ThreadPoolWasShuttedDownException>(() => threadPool.QueueWorkItem(() => 0));
            }
        }

        [Test]
        public void IsCompletedTest()
        {
            using var threadPool = new MyThreadPool(threadCount);
            for (var i = 0; i < 10000; ++i)
            {
                intTasks.Add(threadPool.QueueWorkItem(() =>
                {
                    manualResetEvent.WaitOne();
                    return 0;
                }));
            }

            foreach (var task in intTasks)
            {
                Assert.IsFalse(task.IsCompleted);
            }

            manualResetEvent.Set();

            foreach (var task in intTasks)
            {
                Assert.AreEqual(0, task.Result);
                Assert.IsTrue(task.IsCompleted);
            }
        }

        [Test]
        public void WhetherTheTasksThatWereQueuedBeforeShutdownAreFinishedAfterItTest()
        {
            using var threadPool = new MyThreadPool(threadCount);
            for (var i = 0; i < 1000; ++i)
            {
                intTasks.Add(threadPool.QueueWorkItem(() => 
                {
                    manualResetEvent.WaitOne();
                    return Thread.CurrentThread.ManagedThreadId;
                }));
            }

            foreach (var task in intTasks)
            {
                Assert.IsFalse(task.IsCompleted);
            }

            manualResetEvent.Set();
            threadPool.Shutdown();

            var threadIds = new HashSet<int>();
            foreach (var task in intTasks)
            {
                threadIds.Add(task.Result);
                Assert.IsTrue(task.IsCompleted);
            }

            Assert.AreEqual(threadCount, threadIds.Count);
        }

        [Test]
        public void IsCorrectResultTest()
        {
            var sum = 0;
            var expected = 1000000;
            using var threadPool = new MyThreadPool(threadCount);
            for (var i = 0; i < expected; ++i)
            {
                intTasks.Add(threadPool.QueueWorkItem(() =>
                {
                    Interlocked.Increment(ref sum);
                    return 0;
                }));
            }

            foreach (var task in intTasks)
            {
                Assert.AreEqual(0, task.Result);
            }

            Assert.AreEqual(expected, sum);
        }

        [TestCase(1)]
        [TestCase(2)]
        [TestCase(3)]
        [TestCase(5)]
        [TestCase(10)]
        [Test]
        public void MultipleContinueWithResultTest(int threadCount)
        {
            using var threadPool = new MyThreadPool(threadCount);

            var intTaskResults = new List<int> { 15, 3, 5 };
            var intTasks = new List<IMyTask<int>>
            {
                threadPool.QueueWorkItem(() =>
                    {
                        manualResetEvent.WaitOne();
                        return 15;
                    })
            };

            intTasks.Add(
                intTasks[0].ContinueWith(x =>
                {
                    manualResetEvent.WaitOne();
                    return 45 / x;
                }));

            intTasks.Add(
                intTasks[1].ContinueWith(x =>
                {
                    manualResetEvent.WaitOne();
                    return x + 2;
                }));

            var stringTasksResetEvent = new ManualResetEvent(false);
            var stringTaskResults = new List<string> { "5", "5 cats" };
            var stringTasks = new List<IMyTask<string>>
            {
                intTasks[2].ContinueWith(x =>
                    {
                        stringTasksResetEvent.WaitOne();
                        return x.ToString();
                    })
            };

            stringTasks.Add(stringTasks[0].ContinueWith(s =>
                {
                    stringTasksResetEvent.WaitOne();
                    return s + " cats";
                }));
            
            foreach (var task in intTasks)
            {
                Assert.IsFalse(task.IsCompleted);
            }
            foreach (var task in stringTasks)
            {
                Assert.IsFalse(task.IsCompleted);
            }

            manualResetEvent.Set();

            for (var i = 0; i < intTasks.Count; ++i)
            {
                Assert.AreEqual(intTaskResults[i], intTasks[i].Result);
                Assert.IsTrue(intTasks[i].IsCompleted);
            }

            foreach (var task in stringTasks)
            {
                Assert.IsFalse(task.IsCompleted);
            }

            stringTasksResetEvent.Set();

            for (var i = 0; i < stringTasks.Count; ++i)
            {
                Assert.AreEqual(stringTaskResults[i], stringTasks[i].Result);
                Assert.IsTrue(stringTasks[i].IsCompleted);
            }

            stringTasksResetEvent.Dispose();
        }

        [TestCase(5)]
        [TestCase(3)]
        [TestCase(2)]
        [TestCase(1)]
        [Test]
        public void ContinueWithThatWasNotQueuedBeforeShutdownWouldThrowExceptionTest(int threadCount)
        {
            using var threadPool = new MyThreadPool(threadCount);
            var task = threadPool.QueueWorkItem(() => 90);
            _ = task.Result;
            threadPool.Shutdown();
            for (var i = 0; i < 100; ++i) 
            {
                Assert.Throws<ThreadPoolWasShuttedDownException>(() => task.ContinueWith(x => x + 9));
            }
        }

        [Test]
        public void ContinueWithOnIncompletedTaskWorksCorrectlyTest()
        {
            using var threadPool = new MyThreadPool(2);
            var autoResetEvent = new AutoResetEvent(false);
            var task = threadPool.QueueWorkItem(() => 
            {
                autoResetEvent.WaitOne();
                return 90; 
            });

            var continuation = task.ContinueWith(x => x + 10);
            autoResetEvent.Set();

            Assert.AreEqual(100, continuation.Result);
            Assert.IsTrue(task.IsCompleted);
        }

        [Test]
        public void ContinueWithOnCompletedTaskWorksCorrectlyTest()
        {
            using var threadPool = new MyThreadPool(2);
            var task = threadPool.QueueWorkItem(() => 90);
            _ = task.Result;

            var continuation = task.ContinueWith(x => x + 10);

            Assert.AreEqual(100, continuation.Result);
            Assert.IsTrue(continuation.IsCompleted);
        }

        [Test]
        public void WhetherContinueWithThatWasQueuedBeforeShutdownIsFinishedAfterItTest()
        {
            using var threadPool = new MyThreadPool(threadCount);
            var task = threadPool.QueueWorkItem(() => 90);
            _ = task.Result;
            for (var i = 0; i < 5; ++i)
            {
                intTasks.Add(task.ContinueWith(x =>
                {
                    manualResetEvent.WaitOne();
                    return x + 10;
                }));
            }

            foreach (var t in intTasks)
            {
                Assert.IsFalse(t.IsCompleted);
            }

            manualResetEvent.Set();
            threadPool.Shutdown();

            foreach (var t in intTasks)
            {
                _ = t.Result;
                Assert.IsTrue(t.IsCompleted);
            }
        }

        [Test]
        public void EnqueueNullSupplierToThreadPoolTest()
        {
            using var threadPool = new MyThreadPool(3);
            Assert.Throws<ArgumentNullException>(() => threadPool.QueueWorkItem<object>(null));
        }

        [Test]
        public void ContinuationIsNullSupplierTest()
        {
            using var threadPool = new MyThreadPool(3);
            var task = threadPool.QueueWorkItem(() => 10);
            Assert.Throws<ArgumentNullException>(() => task.ContinueWith<object>(null));
        }
    }
}