using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnit
{
    class TestRunner
    {
        public Task Run(string path)
        {
            var tasks = new List<Task>();
            var directoryAssemblies = Directory.GetFiles(path, @"$(?:.exe|.dll)");
            foreach (var assemblyName in directoryAssemblies)
            {
                foreach (var type in Assembly.Load(assemblyName).ExportedTypes)
                {
                    var localType = type;
                    var task = Task.Run(() => RunTypeTests(localType.Name));
                    tasks.Add(task);
                }
            }

            return Task.WhenAll(tasks);
        }

        private async Task RunTypeTests(string typeName)
        {
            var type = Type.GetType(typeName);
            var testMethods = GetMethodsWithSpecifiedAttribute(type, typeof(TestAttribute));
            var beforeClassMethods = GetMethodsWithSpecifiedAttribute(type, typeof(BeforeClassAttribute));
            var beforeMethods = GetMethodsWithSpecifiedAttribute(type, typeof(BeforeAttribute));
            var afterMethods = GetMethodsWithSpecifiedAttribute(type, typeof(AfterAttribute));
            var afterClassMethods = GetMethodsWithSpecifiedAttribute(type, typeof(AfterClassAttribute));
            var testObject = Convert.ChangeType(Activator.CreateInstance(type), type);
            await Task.WhenAll(ExecuteStaticMethods(beforeClassMethods));
            //foreach (var method in )
            //{
            //    // beforeMethod.Invoke(testObject, new object[] { });
            //    // testMethod.Invoke(testObject, new object[] { });
            //    // afterMethod.Invoke(testObject, new object[] { });
            //}
            await Task.WhenAll(ExecuteStaticMethods(afterClassMethods));   
        }

        private List<Task> ExecuteStaticMethods(List<MethodInfo> methods)
        {
            var methodsExecutionTasks = new List<Task>();
            foreach (var mi in methods) // static validation?
            {
                methodsExecutionTasks.Add(Task.Run(() => mi.Invoke(null, new object[0])));
            }
            return methodsExecutionTasks;
        }

        private List<MethodInfo> GetMethodsWithSpecifiedAttribute(Type type, Type attributeType)
        {
            return type.GetMethods()
                .Where(m => m.GetCustomAttributes(attributeType).ToList().Count > 0).ToList(); // ловушка!!!
        }
    }
}
