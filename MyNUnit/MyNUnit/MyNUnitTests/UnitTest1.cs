using NUnit.Framework;

namespace MyNUnitTests
{
    public class Tests
    {
        int a = 0;

        [SetUp]
        void Setup()
        {
            a = 5;
        }

        [OneTimeSetUp]
        public static void BeforeClass()
        {

        }

        [Test]
        public void Test1()
        {
            Assert.AreEqual(5, a);
        }

        [Test]
        public void Test2()
        {
            Assert.Pass();
        }
    }
}