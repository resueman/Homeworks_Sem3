using NUnit.Framework;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading;

namespace MyThreadPool.Tests
{
    public class Tests
    {
        private MyThreadPool threadPool;
        private int threadCount;

        [SetUp]
        public void Setup()
        {
            threadCount = Environment.ProcessorCount + 1;
            threadPool = new MyThreadPool(threadCount);
        }

        [Test]
        public void Test1()
        {
        }

        [Test]
        public void AggregateExceptionTest()
        {
            var task0 = threadPool.QueueWorkItem(() => 0);
            var task1 = task0.ContinueWith(x => 13 / x);
            var task2 = task1.ContinueWith(x => x + 1);

            Assert.Throws<AggregateException>(() => _ = task1.Result);
            Assert.Throws<AggregateException>(() => _ = task2.Result);
        }

        [Test]
        public void ShutThreadPoolDownMoreThanOnceTest()
        {
            threadPool.Shutdown();
            for (var i = 0; i < 100; ++i)
            {
                Assert.Throws<ThreadPoolWasShuttedDownException>(() => threadPool.Shutdown());
            }
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
            threadPool = new MyThreadPool(threadCount);
            var threadIds = new HashSet<int>();
            var resetEvent = new ManualResetEvent(false);
            var tasks = new List<IMyTask<int>>();            
            for (var i = 0; i < 10000; ++i)
            {
                var task = threadPool.QueueWorkItem(() =>
                {
                    resetEvent.WaitOne();
                    return Thread.CurrentThread.ManagedThreadId;
                });
                tasks.Add(task);
            }

            for (var i = 0; i < 5; ++i)
            {
                resetEvent.Set();
                resetEvent.Reset();
                Thread.Sleep(1000);
            }
            resetEvent.Set();

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
            threadPool = new MyThreadPool(threadCount);
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
            threadPool = new MyThreadPool(threadCount);
            threadPool.Shutdown();
            Assert.AreEqual(0, threadPool.ActiveThreads);
        }

        [Test]
        public void Test()
        {

        }
    }
}