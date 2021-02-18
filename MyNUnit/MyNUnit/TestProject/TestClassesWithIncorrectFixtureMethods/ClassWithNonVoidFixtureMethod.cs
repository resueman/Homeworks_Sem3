using MyNUnit;

namespace TestProject
{
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
