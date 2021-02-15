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
            FindAllMyNUnitMethods();
        }

        public Type TestClassType { get; private set; }

        public List<TestMethod> TestMethods { get; private set; }

        public List<FixtureMethod> BeforeMethods { get; private set; }

        public List<FixtureMethod> AfterMethods { get; private set; }

        public List<StaticFixtureMethod> BeforeClassMethods { get; private set; }

        public List<StaticFixtureMethod> AfterClassMethods { get; private set; }

        public void RunTests()
        {
            var testClassInstance = Activator.CreateInstance(TestClassType);
            BeforeClassMethods.ForEach(m => m.Execute(null));
            Parallel.ForEach(TestMethods, m => m.Execute(testClassInstance));
            BeforeClassMethods.ForEach(m => m.Execute(null));
        }

        private async Task FindAllMyNUnitMethods()
        {
            var tasks = new List<Task>()
            {
                Task.Run(() => TestMethods = TestClassType.GetMethods()
                    .Where(m => TestMethod.IsTestMethod(m))
                    .Select(m => new TestMethod(m))
                    .ToList()),

                Task.Run(() => BeforeMethods = TestClassType.GetMethods()
                    .Where(m => FixtureMethod.IsFixtureMethod(m))
                    .Select(m => new FixtureMethod(m))
                    .ToList()),

                Task.Run(() => AfterMethods = TestClassType.GetMethods()
                    .Where(m => FixtureMethod.IsFixtureMethod(m))
                    .Select(m => new FixtureMethod(m))
                    .ToList()),

                Task.Run(() => BeforeClassMethods = TestClassType.GetMethods()
                    .Where(m => StaticFixtureMethod.IsStaticFixtureMethod(m))
                    .Select(m => new StaticFixtureMethod(m))
                    .ToList()),

                Task.Run(() => AfterClassMethods = TestClassType.GetMethods()
                    .Where(m => StaticFixtureMethod.IsStaticFixtureMethod(m))
                    .Select(m => new StaticFixtureMethod(m))
                    .ToList())
            };

            await Task.WhenAll(tasks);
        }
    }
}
