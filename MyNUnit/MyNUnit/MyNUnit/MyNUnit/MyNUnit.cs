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
            var directoryAssemblies = Directory.GetFiles(path, @"$(?:.exe|.dll)").ToList();
            var currentAssembly = directoryAssemblies.SingleOrDefault(a => a == Assembly.GetExecutingAssembly().GetName().Name); ///
            directoryAssemblies.Remove(currentAssembly);
            foreach (var assemblyName in directoryAssemblies)
            {
                var types = Assembly.Load(assemblyName).ExportedTypes.Where(t => t.IsClass).ToList();
                foreach (var type in types)
                {
                    var testsClass = new MyNUnitTestsClass(type);
                    var task = Task.Run(() => testsClass.RunTests());
                    tasks.Add(task);
                }
            }
            await Task.WhenAll(tasks);
        }

        public static void PrintResult()
        {

        }
    }
}
