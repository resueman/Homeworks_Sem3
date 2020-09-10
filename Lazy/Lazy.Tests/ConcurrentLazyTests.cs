using NUnit.Framework;
using System;
using System.Threading;

namespace Lazy.Tests
{
    /// <summary>
    /// Tests if instance of ConcurrentLazy works correctly in multithreading program 
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

        [Test]
        public void DoesConcurrentLazyReturnTheSameObjectAsAfterEvaluationWhenExecutedSynchronouslyTest()
        {
            var justEvaluatedMatrix = lazyGiantMatrix.Get();
            var alreadyEvaluatedMatrix = lazyGiantMatrix.Get();
            Assert.AreSame(justEvaluatedMatrix, alreadyEvaluatedMatrix);
        }

        [Test]
        public void DoesConcurrentLazyReturnTheSameObjectManyTimesWhenExecutedSynchronouslyTest()
        {
            for (var i = 0; i < 10000; ++i)
            {
                var matrixObject1 = lazyGiantMatrix.Get();
                var matrixObject2 = lazyGiantMatrix.Get();
                Assert.AreSame(matrixObject1, matrixObject2);
            }
        }

        private void AreAllElementsTheSameObject(GiantMatrix[] matrices)
        {
            for (int i = 1; i < matrices.Length; ++i)
            {
                Assert.AreSame(matrices[i - 1], matrices[i]);
            }
        }

        [Test]
        public void DoesConcurrentLazyReturnTheSameObjectInMultithreadedScenarioTest()
        {
            var threadCount = 100;
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

            AreAllElementsTheSameObject(matrices);
        }

        [Test]
        public void PassingNullSupplierToConcurrentLazyTest()
        {
            Assert.Throws<ArgumentNullException>(() => LazyFactory<GiantMatrix>.CreateConcurrentLazy(null));
        }

        [Test]
        public void PassingSupplierThatReturnsNullToConcurrentLazyTest()
        {
            var lazyReturningNull = LazyFactory<GiantMatrix>.CreateConcurrentLazy(() => null);
            Assert.IsNull(lazyReturningNull.Get());
        }
    }
}
