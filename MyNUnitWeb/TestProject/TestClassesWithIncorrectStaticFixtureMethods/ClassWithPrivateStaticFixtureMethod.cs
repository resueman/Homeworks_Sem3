using Attributes;

namespace TestProject
{
    /// <summary>
    /// Example of MyNUnit tests class
    /// </summary>
    public class ClassWithPrivateStaticFixtureMethod
    {
        [BeforeClass]
        static void BeforeClass()
        {

        }

        [Test]
        public void TestFromClassWithPrivateStaticFixtureMethod()
        {

        }
    }
}
