using Attributes;

namespace TestProject
{
    /// <summary>
    /// Example of MyNUnit tests class
    /// </summary>
    public class ClassWithCorrectAndIncorrectTestMethods
    {
        [Test]
        public void ParameterizedTest(int a)
        {
            
        }

        [Test]
        public static void StaticTest()
        {

        }

        [Test]
        public int NonVoidTest()
        {
            return 0;
        }

        [Test]
        public void TestWithCorrectSignature()
        {
            
        }

        [Test]
        internal void InternalTest()
        {

        }

        [Test]
        void PrivateTest()
        {

        }
    }
}
