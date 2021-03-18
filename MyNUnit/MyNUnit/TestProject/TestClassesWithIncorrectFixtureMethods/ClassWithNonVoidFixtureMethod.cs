using Attributes;

namespace TestProject
{
    /// <summary>
    /// Example of MyNUnit tests class
    /// </summary>
    public class ClassWithNonVoidFixtureMethod
    {
        [Before]
        public int BeforeTest()
        {
            return 0;
        }

        [Test]
        public void TestFromClassWithNonVoidFixtureMethod()
        {

        }
    }
}
