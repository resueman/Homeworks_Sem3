using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace Initialize
{
    class Injector
    {
        private static Dictionary<Type, object> implementations = new Dictionary<Type, object>();

        private static object InitializeRootArgument(Type rootClassParameterType, IEnumerable<Type> availableTypes)
        {
            var initializationPath = new Stack<Type>();
            var marked = new List<Type>();
            var tempStack = new Stack<Type>();
            tempStack.Push(rootClassParameterType);
            initializationPath.Push(rootClassParameterType);
            while (tempStack.Count != 0)
            {
                var type = tempStack.Pop();
                var dependencies = type.GetConstructors().First().GetParameters().Select(p => p.GetType()).ToList();
                foreach (var dependency in dependencies)
                {
                    if (!dependency.IsInterface && !dependency.IsAbstract)
                    {
                        if (!marked.Contains(dependency))
                        {
                            tempStack.Push(dependency);
                            initializationPath.Push(dependency);
                            marked.Add(dependency);
                            continue;
                        }
                        throw new Exception("Dependency graph is not a tree");
                    }

                    var realization = dependency.IsInterface
                        ? availableTypes.Where(t => t.GetInterfaces().Contains(dependency))
                        : availableTypes.Where(t => t.IsSubclassOf(dependency));

                    switch (realization.Count())
                    {
                        case 0:
                            throw new Exception("No implementation");
                        case 1:
                            implementations.Add(dependency, realization.First());
                            break;
                        default:
                            throw new Exception("Ambigiuos");
                    }
                    continue;
                }
            }

            // root argument initialization
            object instance = null;
            while (initializationPath.Count != 0)
            {
                var type = initializationPath.Pop();
                if (!implementations.ContainsKey(type))
                {
                    instance = Activator.CreateInstance(type, implementations.Values);
                    implementations.Add(type, instance);
                }
            }

            return instance;
        }

        public static object Initialize(string rootClassName, string[] realizationsTypeNames)
        {
            var rootType = Type.GetType(rootClassName);
            var availableTypes = realizationsTypeNames.Select(Type.GetType);
            var rootClassParameterTypes = rootType.GetConstructors()
                .First(c => c.GetParameters().Select(p => p.GetType()).All(p => availableTypes.Contains(p)))
                .GetParameters()
                .Select(p => p.ParameterType);

            var rootClassArguments = new List<object>();
            foreach (var rootClassParameterType in rootClassParameterTypes)
            {
                var rootClassArgument = InitializeRootArgument(rootClassParameterType, availableTypes);
                rootClassArguments.Add(rootClassArgument);
            }

            return Activator.CreateInstance(rootType, rootClassArguments);
        }
    }
}
