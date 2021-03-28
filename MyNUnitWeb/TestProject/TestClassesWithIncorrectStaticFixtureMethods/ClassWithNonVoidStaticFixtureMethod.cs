using Attributes;

namespace TestProject
{
    /// <summary>
    /// Example of MyNUnit tests class
    /// </summary>
    public class ClassWithNonVoidStaticFixtureMethod
    {
        [BeforeClass]
        public static int BeforeClass()
        {
            return 6;
        }

        [Test]
        public void TestFromClassWithNonVoidStaticFixtureMethod()
        {

        }
    }
}
