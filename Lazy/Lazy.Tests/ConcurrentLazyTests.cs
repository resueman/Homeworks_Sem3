using NUnit.Framework;
using System;
using System.Threading;

namespace Lazy.Tests
{
    /// <summary>
    /// Tests if instance of thread-safe ConcurrentLazy works correctly in multithreading program 
    /// </summary>
    class ConcurrentLazyTests
    {
        private ILazy<GiantMatrix> lazyGiantMatrix;
        private ManualResetEvent resetEvent;
        private CountdownEvent countdownEvent;

        [SetUp]
        public void Setup()
        {
            lazyGiantMatrix = LazyFactory<GiantMatrix>
                .CreateConcurrentLazy(() => new GiantMatrix(1000, 1000));

            resetEvent = new ManualResetEvent(false);
        }

        [TearDown]
        public void TearDown()
        {
            countdownEvent.Dispose();
            resetEvent.Close();
        }

        private void AreAllTheSameObject(GiantMatrix[] matrices)
        {
            for (int i = 1; i < matrices.Length; ++i)
            {
                Assert.AreSame(matrices[i - 1], matrices[i]);
            }
        }

        [Test]
        public void DoesConcurrentLazyReturnTheSameObjectInMultithreadedScenarioTest()
        {
            var threadCount = 20;
            var threads = new Thread[threadCount];
            var matrices = new GiantMatrix[threadCount];
            countdownEvent = new CountdownEvent(threadCount);
            for (var i = 0; i < threads.Length; ++i)
            {
                var threadNumber = i;
                threads[i] = new Thread(() => 
                {
                    resetEvent.WaitOne();
                    matrices[threadNumber] = lazyGiantMatrix.Get();
                    countdownEvent.Signal();
                }); 
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }
            resetEvent.Set();
            countdownEvent.Wait();

            AreAllTheSameObject(matrices);
        }
    }
}
