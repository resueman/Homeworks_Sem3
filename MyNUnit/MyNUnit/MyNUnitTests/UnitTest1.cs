using NUnit.Framework;

namespace MyNUnitTests
{
    public class Tests
    {
        [SetUp]
        public void Setup()
        {
            
        }

        [OneTimeSetUp]
        public static void BeforeClass()
        {

        }

        [Test]
        public void Test1()
        {
            Assert.Pass();
        }

        [Test]
        public void Test2()
        {
            Assert.Pass();
        }
    }
}