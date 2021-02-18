using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnit
{
    /// <summary>
    /// Represents custom MyNUnit, that allows to create tests
    /// </summary>
    public static class MyNUnit
    {
        /// <summary>
        /// Runs tests
        /// </summary>
        /// <param name="path">path to the directory with assembly containing tests</param>
        /// <returns>List of tests classes that contains info about tests and their execution results</returns>
        public static async Task<List<MyNUnitTestsClass>> Run(string path)
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

            return result.Keys.ToList();
        }

        /// <summary>
        /// Prints result of test execution on console 
        /// </summary>
        /// <param name="testTypes">Classes that contains tests</param>
        public static void PrintResult(List<MyNUnitTestsClass> testTypes)
        {
            var i = 1;
            foreach (var testType in testTypes)
            {
                var j = 1;
                Console.WriteLine($"Тестовый класс {i}. {testType.TestClassType.Name}");
                Console.WriteLine();
                foreach (var test in testType.TestMethods)
                {
                    Console.WriteLine($"{j}. {test.Method.Name}");
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
