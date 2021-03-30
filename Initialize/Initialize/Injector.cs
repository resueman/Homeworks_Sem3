using System;
using System.Collections.Generic;
using System.Linq;

namespace Initialize
{
    /// <summary>
    /// Emulates the work of the Dependency Injection container
    /// </summary>
    class Injector
    {
        /// <summary>
        /// Contains implementation for each type using in root class initialization
        /// </summary>
        private static readonly Dictionary<Type, object> implementations = new();

        /// <summary>
        /// Initializes root type argument if it's possible using list of available types
        /// </summary>
        /// <param name="rootClassParameterType">Root class parameter type</param>
        /// <param name="availableTypes">Types available to use in root class argument initialization</param>
        /// <returns></returns>
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
                        throw new InjectorException("Dependency graph is not a tree");
                    }

                    var realization = dependency.IsInterface
                        ? availableTypes.Where(t => t.GetInterfaces().Contains(dependency))
                        : availableTypes.Where(t => t.IsSubclassOf(dependency));

                    switch (realization.Count())
                    {
                        case 0:
                            throw new InjectorException($"No implementation for {dependency}");
                        case 1:
                            implementations.Add(dependency, realization.First());
                            break;
                        default:
                            throw new InjectorException("Ambiguous of implementations");
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
                    try
                    {
                        instance = Activator.CreateInstance(type, implementations.Values);
                        implementations.Add(type, instance);
                    }
                    catch (TypeLoadException e)
                    {
                        throw new InjectorException($"No implementation for {type}", e);
                    }
                }
            }

            return instance;
        }

        /// <summary>
        /// Creates object of specified type
        /// </summary>
        /// <param name="rootClassName">Full name of class, instance of which should be created</param>
        /// <param name="realizationsTypeNames">Full names of classes available for constructor initializations</param>
        /// <returns>Object of specified type</returns>
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
