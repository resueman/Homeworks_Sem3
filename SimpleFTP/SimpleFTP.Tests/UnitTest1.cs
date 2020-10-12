using NUnit.Framework;

namespace SimpleFTP.Tests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            var server = new Server();
            var client = new Client();
        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }
    }
}