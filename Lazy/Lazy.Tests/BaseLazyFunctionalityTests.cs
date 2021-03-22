using NUnit.Framework;
using System;

namespace Lazy.Tests
{
    /// <summary>
    /// Tests if instances of thread-unsafe and thread-safe Lazy
    /// work correctly in base scenarios of single-thread program 
    /// </summary>
    /// <typeparam name="TLazy">Type of Lazy - thread-unsafe or thread-safe</typeparam>
    [TestFixture(typeof(Lazy.Lazy<GiantMatrix>))]
    [TestFixture(typeof(ConcurrentLazy<GiantMatrix>))]
    class BaseLazyFunctionalityTests<TLazy>
    {
        private ILazy<GiantMatrix> lazyGiantMatrix;

        [SetUp]
        public void Setup()
        {
            lazyGiantMatrix = typeof(TLazy) == typeof(ConcurrentLazy<GiantMatrix>) 
                ? LazyFactory<GiantMatrix>.CreateConcurrentLazy(() => new GiantMatrix(10000, 10000))               
                : LazyFactory<GiantMatrix>.CreateLazy(() => new GiantMatrix(10000, 10000));
        }

        [Test]
        public void DoesLazyReturnsTheSameObjectAsAfterEvaluationTest()
        {
            var justEvaluatedMatrix = lazyGiantMatrix.Get();
            var alreadyEvaluatedMatrix = lazyGiantMatrix.Get();
            Assert.AreSame(justEvaluatedMatrix, alreadyEvaluatedMatrix);
        }

        [Test]
        public void DoesLazyReturnsTheSameObjectAsAfterEvaluationManyTimesTest()
        {
            for (var i = 0; i < 10000; ++i)
            {
                var objectReference1 = lazyGiantMatrix.Get();
                var objectReference2 = lazyGiantMatrix.Get();
                Assert.AreSame(objectReference1, objectReference2);
            }
        }

        [Test]
        public void PassingNullSupplierToLazyTest()
        {
            if (typeof(TLazy) == typeof(ConcurrentLazy<GiantMatrix>))
            {
                Assert.Throws<ArgumentNullException>(() => LazyFactory<GiantMatrix>.CreateConcurrentLazy(null));
                return;
            }
            Assert.Throws<ArgumentNullException>(() => LazyFactory<GiantMatrix>.CreateLazy(null));
        }

        [Test]
        public void PassingSupplierThatReturnsNullToLazyTest()
        {
            var lazyReturningNull = typeof(TLazy) == typeof(ConcurrentLazy<GiantMatrix>)
                ? LazyFactory<GiantMatrix>.CreateConcurrentLazy(() => null)
                : LazyFactory<GiantMatrix>.CreateLazy(() => null);
            
            Assert.IsNull(lazyReturningNull.Get());
        }
    }
}
