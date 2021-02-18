using MyNUnit;

namespace TestProject
{
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
