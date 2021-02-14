using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnit
{
    class TestsClass
    {
        public TestsClass(Type type)
        {
            TestClassType = type;

            TestMethods = GetAttributeMethods(type, typeof(TestAttribute))
                .Select(m => new TestMethod(m)).ToList();
            
            BeforeTestMethods = GetAttributeMethods(type, typeof(BeforeAttribute))
                .Select(m => new FixtureMethod(m)).ToList();
            
            AfterTestMethods = GetAttributeMethods(type, typeof(AfterAttribute))
                .Select(m => new FixtureMethod(m)).ToList();

            BeforeClassMethods = GetAttributeMethods(type, typeof(BeforeClassAttribute))
                .Select(m => new StaticFixtureMethod(m)).ToList();

            AfterClassMethods = GetAttributeMethods(type, typeof(AfterClassAttribute))
                .Select(m => new StaticFixtureMethod(m)).ToList();
        }

        public Type TestClassType { get; private set; }

        public List<TestMethod> TestMethods { get; private set; }

        public List<FixtureMethod> BeforeTestMethods { get; private set; }

        public List<FixtureMethod> AfterTestMethods { get; private set; }

        public List<StaticFixtureMethod> BeforeClassMethods { get; private set; }

        public List<StaticFixtureMethod> AfterClassMethods { get; private set; }

        private List<MethodInfo> GetAttributeMethods(Type type, Type attributeType) // здесь ловушка!!!
            => type.GetMethods().Where(m => m.GetCustomAttributes(attributeType).ToList().Count > 0).ToList();

        public void RunTests()
        {
            var testClassInstance = Convert.ChangeType(Activator.CreateInstance(TestClassType), TestClassType);
            BeforeClassMethods.ForEach(m => m.Execute(null));
            Parallel.ForEach(TestMethods, m => m.Execute(testClassInstance));
            BeforeClassMethods.ForEach(m => m.Execute(null));
        }
    }
}
