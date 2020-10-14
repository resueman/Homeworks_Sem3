using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MyThreadPool.Tests
{
    public class Tests
    {
        private int threadCount;
        private HashSet<int> threadIds;
        private ManualResetEvent manualResetEvent;
        private List<IMyTask<int>> tasks;

        [SetUp]
        public void Setup()
        {
            threadCount = Environment.ProcessorCount + 1;
            threadIds = new HashSet<int>();
            manualResetEvent = new ManualResetEvent(false);
            tasks = new List<IMyTask<int>>();
        }

        [TearDown]
        public void TearDown()
        {
            manualResetEvent.Dispose();
        }

        [Test]
        public void AggregateExceptionTest()
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
            using var threadPool = new MyThreadPool(threadCount);         
            for (var i = 0; i < 10000; ++i)
            {
                var task = threadPool.QueueWorkItem(() =>
                {
                    manualResetEvent.WaitOne();
                    return Thread.CurrentThread.ManagedThreadId;
                });
                tasks.Add(task);
            }

            for (var i = 0; i < 5; ++i)
            {
                manualResetEvent.Set();
                manualResetEvent.Reset();
                Thread.Sleep(1000);
            }
            manualResetEvent.Set();

            foreach (var task in tasks)
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
        public void WhetherTheTasksThatWereQueuedBeforeShutdownAreFinishedAfterItTest()
        {
            var taskCount = 1000;
            var tasks = new List<IMyTask<int>>();
            var countDownEvent = new CountdownEvent(taskCount);

            using var threadPool = new MyThreadPool(threadCount);
            for (var i = 0; i < taskCount; ++i)
            {
                tasks.Add(threadPool.QueueWorkItem(() => 
                {
                    manualResetEvent.WaitOne();
                    countDownEvent.Signal();
                    return Thread.CurrentThread.ManagedThreadId;
                }));
            }

            foreach (var task in tasks)
            {
                Assert.IsFalse(task.IsCompleted);
            }

            manualResetEvent.Set();
            threadPool.Shutdown();

            countDownEvent.Wait();
            foreach (var task in tasks)
            {
                threadIds.Add(task.Result);
                Assert.IsTrue(task.IsCompleted);
            }

            Assert.AreEqual(threadCount, threadIds.Count);

            countDownEvent.Dispose();
        }
    }
}