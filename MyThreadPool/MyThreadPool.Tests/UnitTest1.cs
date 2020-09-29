using NUnit.Framework;

namespace MyThreadPool.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
        }

        [Test]
        public void Test1()
        {
            var myThreadPool = new MyThreadPool(10);
            var task = myThreadPool.QueueWorkItem(() => 20 + 8).ContinueWith(x => x.ToString());
        }
    }
}