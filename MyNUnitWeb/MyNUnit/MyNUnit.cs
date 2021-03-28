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
            var types = GetDefinedTypes(path);
            var result = new Dictionary<MyNUnitTestsClass, Task>();
            foreach (var type in types)
            {
                var testsClass = new MyNUnitTestsClass(type);
                result.Add(testsClass, Task.Run(() => testsClass.RunTests()));
            }
            await Task.WhenAll(result.Values);

            return result.Keys.ToList();
        }

        private static List<Type> GetDefinedTypes(string path)
        {
            var isDirectory = File.GetAttributes(path).HasFlag(FileAttributes.Directory);
            if (!isDirectory)
            {              
                return Assembly.Load(File.ReadAllBytes(path)).ExportedTypes.Where(t => t.IsClass).ToList();
            }

            var tasks = new List<Task>();
            var directoryAssemblies = Directory.EnumerateFiles(path).Where(f => f.EndsWith(".dll") || f.EndsWith(".exe")).ToList();
            var currentAssemblyName = Assembly.GetExecutingAssembly().GetName().Name;
            directoryAssemblies.RemoveAll(a => a.EndsWith(currentAssemblyName + ".dll") || a.EndsWith(currentAssemblyName + ".exe"));
            directoryAssemblies.RemoveAll(a => a == $"{path}\\Attributes.dll");
            
            var rawAssemblies = directoryAssemblies.Select(path => File.ReadAllBytes(path));
            return rawAssemblies
                .Select(Assembly.Load)
                .AsParallel()
                .SelectMany(a => a.ExportedTypes)
                .Where(t => t.IsClass)
                .ToList();
        }
    }
}
