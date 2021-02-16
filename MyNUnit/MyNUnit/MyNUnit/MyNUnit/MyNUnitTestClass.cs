using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnit
{
    class MyNUnitTestsClass
    {
        public MyNUnitTestsClass(Type type)
        {
            TestClassType = type;
        }

        public Type TestClassType { get; private set; }

        public List<TestMethod> TestMethods { get; private set; }

        public List<FixtureMethod> BeforeMethods { get; private set; }

        public List<FixtureMethod> AfterMethods { get; private set; }

        public List<StaticFixtureMethod> BeforeClassMethods { get; private set; }

        public List<StaticFixtureMethod> AfterClassMethods { get; private set; }

        public async Task RunTests()
        {
            var testClassInstance = Activator.CreateInstance(TestClassType);
            await DiscoverAllMyNUnitMethods();
            BeforeClassMethods.ForEach(m => m.Execute(null));
            Parallel.ForEach(TestMethods, m => m.Execute(testClassInstance));
            BeforeClassMethods.ForEach(m => m.Execute(null));
        }

        private async Task DiscoverAllMyNUnitMethods()
        {
            var tasks = new List<Task>()
            {                    
                Task.Run(() => BeforeMethods = TestClassType.GetMethods()
                    .Where(m => m.GetCustomAttributes<BeforeAttribute>().Any())
                    .Select(m => new FixtureMethod(m))
                    .ToList()),

                Task.Run(() => AfterMethods = TestClassType.GetMethods()
                    .Where(m => m.GetCustomAttributes<AfterAttribute>().Any())
                    .Select(m => new FixtureMethod(m))
                    .ToList()),

                Task.Run(() => BeforeClassMethods = TestClassType.GetMethods()
                    .Where(m => m.GetCustomAttributes<BeforeClassAttribute>().Any())
                    .Select(m => new StaticFixtureMethod(m))
                    .ToList()),

                Task.Run(() => AfterClassMethods = TestClassType.GetMethods()
                    .Where(m => m.GetCustomAttributes<AfterClassAttribute>().Any())
                    .Select(m => new StaticFixtureMethod(m))
                    .ToList())
            };

            Exception exception = null;
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (IncorrectSignatureOfMyNUnitMethodException e)
            {
                exception = e;
            }

            var errorMessage = exception != null ? exception.Message : "";
            await Task.Run(() => TestMethods = TestClassType.GetMethods()
                .Where(m => m.GetCustomAttributes<TestAttribute>().Any())
                .Select(m => new TestMethod(m, errorMessage))
                .ToList());   
        }
    }
}
