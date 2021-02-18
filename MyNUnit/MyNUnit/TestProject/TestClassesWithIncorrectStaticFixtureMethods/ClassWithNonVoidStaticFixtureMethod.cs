using MyNUnit;

namespace TestProject
{
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
