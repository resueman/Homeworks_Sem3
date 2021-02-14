using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading.Tasks;

namespace MyNUnit
{
    static class MyNUnit
    {
        public static async Task Run(string path)
        {
            var tasks = new List<Task>();
            var directoryAssemblies = Directory.GetFiles(path, @"$(?:.exe|.dll)");
            foreach (var assemblyName in directoryAssemblies)
            {
                foreach (var type in Assembly.Load(assemblyName).ExportedTypes)
                {
                    var testsClass = new TestsClass(type);
                    var task = Task.Run(() => testsClass.RunTests());
                    tasks.Add(task);
                }
            }
            await Task.WhenAll(tasks);
        }
    }
}
