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
            TestMethods = new List<TestMethod>();
            BeforeMethods = new List<FixtureMethod>();
            AfterMethods = new List<FixtureMethod>();
            BeforeClassMethods = new List<StaticFixtureMethod>();
            AfterClassMethods = new List<StaticFixtureMethod>();
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
            Parallel.ForEach(TestMethods, m =>
            {
                Parallel.ForEach(BeforeMethods, bm => bm.Execute(testClassInstance));
                m.Execute(testClassInstance);
                Parallel.ForEach(AfterMethods, am => am.Execute(testClassInstance));
            });
            AfterClassMethods.ForEach(m => m.Execute(null));
        }

        private async Task DiscoverAllMyNUnitMethods()
        {
            var tasks = new List<Task>()
            {                    
                Task.Run(() => BeforeMethods = TestClassType.GetRuntimeMethods()
                    .Where(m => m.GetCustomAttributes<BeforeAttribute>().Any())
                    .Select(m => new FixtureMethod(m))
                    .ToList()),

                Task.Run(() => AfterMethods = TestClassType.GetRuntimeMethods()
                    .Where(m => m.GetCustomAttributes<AfterAttribute>().Any())
                    .Select(m => new FixtureMethod(m))
                    .ToList()),

                Task.Run(() => BeforeClassMethods = TestClassType.GetRuntimeMethods()
                    .Where(m => m.GetCustomAttributes<BeforeClassAttribute>().Any())
                    .Select(m => new StaticFixtureMethod(m))
                    .ToList()),

                Task.Run(() => AfterClassMethods = TestClassType.GetRuntimeMethods()
                    .Where(m => m.GetCustomAttributes<AfterClassAttribute>().Any())
                    .Select(m => new StaticFixtureMethod(m))
                    .ToList())
            };

            Exception exceptionOnFixtureMethodsRetrieving = null;
            try
            {
                await Task.WhenAll(tasks);
            }
            catch (IncorrectSignatureOfMyNUnitMethodException e)
            {
                exceptionOnFixtureMethodsRetrieving = e;
            }

            var errorMessage = exceptionOnFixtureMethodsRetrieving != null ? exceptionOnFixtureMethodsRetrieving.Message : "";
            await Task.Run(() => TestMethods = TestClassType.GetRuntimeMethods()
                .Where(m => m.GetCustomAttributes<TestAttribute>().Any())
                .Select(m => new TestMethod(m, errorMessage))
                .ToList());
        }
    }
}
