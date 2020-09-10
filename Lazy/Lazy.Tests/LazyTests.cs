using NUnit.Framework;
using System;

namespace Lazy.Tests
{
    /// <summary>
    /// Tests if instance of Lazy works correctly in synchronous program
    /// </summary>
    public class LazyTests
    {
        private ILazy<GiantMatrix> lazyGiantMatrix;

        [SetUp]
        public void Setup()
        {
            lazyGiantMatrix = LazyFactory<GiantMatrix>.CreateLazy(() => new GiantMatrix(1000, 1000));
        }

        [Test]
        public void DoesLazyReturnTheSameObjectAsAfterEvaluationTest()
        {            
            var justEvaluatedMatrix = lazyGiantMatrix.Get();
            var alreadyEvaluatedMatrix = lazyGiantMatrix.Get();
            Assert.AreSame(justEvaluatedMatrix, alreadyEvaluatedMatrix);
        }

        [Test]
        public void DoesLazyReturnTheSameObjectAsAfterEvaluationManyTimesTest()
        {
            for (var i = 0; i < 10000; ++i)
            {
                var matrixObject1 = lazyGiantMatrix.Get();
                var matrixObject2 = lazyGiantMatrix.Get();
                Assert.AreSame(matrixObject1, matrixObject2);
            }
        }

        [Test]
        public void PassingNullSupplierToLazyTest()
        {
            Assert.Throws<ArgumentNullException>(() => LazyFactory<GiantMatrix>.CreateLazy(null));
        }

        [Test]
        public void PassingSupplierThatReturnsNullToLazyTest()
        {
            var lazyReturningNull = LazyFactory<GiantMatrix>.CreateLazy(() => null);
            Assert.IsNull(lazyReturningNull.Get());
        }
    }
}