using NUnit.Framework;

namespace Lazy.Tests
{
    /// <summary>
    /// Tests if LazyFactory creates lazy matching the request
    /// </summary>
    class LazyFactoryTests
    {
        [Test]
        public void CreatesLazyTest()
        {
            var lazy = LazyFactory<GiantMatrix>.CreateLazy(() => new GiantMatrix(10, 10));
            Assert.IsInstanceOf<Lazy<GiantMatrix>>(lazy);
        }

        [Test]
        public void CreatesConcurrentLazyTest()
        {
            var concurrentLazy = LazyFactory<GiantMatrix>.CreateConcurrentLazy(() => new GiantMatrix(10, 10));
            Assert.IsInstanceOf<ConcurrentLazy<GiantMatrix>>(concurrentLazy);
        }
    }
}
