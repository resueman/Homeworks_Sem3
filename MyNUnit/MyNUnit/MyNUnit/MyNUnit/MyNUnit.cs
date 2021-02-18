using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnit
{
    static class MyNUnit
    {
        public static async Task Run(string path)
        {
            var tasks = new List<Task>();
            var directoryAssemblies = Directory.EnumerateFiles(path).Where(f => f.EndsWith(".dll") || f.EndsWith(".exe")).ToList();
            var currentAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            directoryAssemblies.RemoveAll(a => a.EndsWith(currentAssemblyName + ".dll") || a.EndsWith(currentAssemblyName + ".exe"));

            var types = directoryAssemblies.Select(Assembly.LoadFrom).SelectMany(a => a.ExportedTypes).Where(t => t.IsClass).ToList();
            var result = new Dictionary<MyNUnitTestsClass, Task>();
            foreach (var type in types)
            {
                var testsClass = new MyNUnitTestsClass(type);
                result.Add(testsClass, Task.Run(() => testsClass.RunTests()));
            }            
            await Task.WhenAll(result.Values);
            PrintResult(result.Keys.ToList());
        }

        public static void PrintResult(List<MyNUnitTestsClass> testTypes)
        {
            var i = 1;
            foreach (var testType in testTypes)
            {
                var j = 1;
                foreach (var test in testType.TestMethods)
                {
                    Console.WriteLine($"{i}.{j} {test.Method.Name}");
                    Console.WriteLine($"Результат выполнения теста: {test.ExecutionResult.Status}");
                    Console.WriteLine($"Время выполнения теста {test.ExecutionResult.ExecutionTime.TotalMilliseconds}");
                    if (test.ExecutionResult.Message != "")
                        Console.WriteLine($"Причина провала теста: {test.ExecutionResult.Message}");
                    Console.WriteLine();
                    ++j;
                }
                ++i;
            }
        }
    }
}
