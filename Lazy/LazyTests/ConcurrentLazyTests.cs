using NUnit.Framework;
using Lazy;
using System.Threading;

namespace LazyTests
{
    class ConcurrentLazyTests
    {
        private ILazy<GiantMatrix> lazyGiantMatrix;
        private static ManualResetEvent resetEvent; 

        [SetUp]
        public void Setup()
        {
            lazyGiantMatrix = LazyFactory<GiantMatrix>.CreateLazy(() => new GiantMatrix(1000, 1000));
            resetEvent = new ManualResetEvent(false);
        }

        [Test]
        public void DoesConcurrentLazyReturnTheSameObjectAsAfterEvaluationWhenExecutedSynchronously()
        {
            var justEvaluatedMatrix = lazyGiantMatrix.Get();
            var alreadyEvaluatedMatrix = lazyGiantMatrix.Get();
            Assert.AreSame(justEvaluatedMatrix, alreadyEvaluatedMatrix);
        }

        [Test]
        public void DoesConcurrentLazyReturnTheSameObjectManyTimesWhenExecutedSynchronously()
        {
            for (var i = 0; i < 10000; ++i)
            {
                var matrixObject1 = lazyGiantMatrix.Get();
                var matrixObject2 = lazyGiantMatrix.Get();
                Assert.AreSame(matrixObject1, matrixObject2);
            }
        }

        [Test]
        public void DoesLazyReturnTheSameObjectInMultithreadedScenario()
        {
            var threadCount = 100;
            var matrices = new GiantMatrix[threadCount];
            var threads = new Thread[threadCount];
            for (var i = 0; i < threads.Length; ++i)
            {
                var threadNumber = i;
                threads[i] = new Thread(() => 
                {
                    resetEvent.WaitOne();
                    matrices[threadNumber] = lazyGiantMatrix.Get();
                }); 
            }

            foreach (var thread in threads)
            {
                thread.Start();
            }
            resetEvent.Set();

            foreach (var thread in threads)
            {
                thread.Join();
            }
        }
    }
}
