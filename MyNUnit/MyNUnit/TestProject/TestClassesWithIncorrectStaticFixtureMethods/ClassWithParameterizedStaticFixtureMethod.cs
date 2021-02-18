using MyNUnit;

namespace TestProject
{
    public class ClassWithParameterizedStaticFixtureMethod
    {
        [BeforeClass]
        public static void BeforeClass(int a)
        {

        }

        [Test]
        public void TestFromClassWithParameterizedStaticFixtureMethod()
        {

        }
    }
}
