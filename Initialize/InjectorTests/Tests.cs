using Injector;
using NUnit.Framework;
using System.Collections.Generic;

namespace InjectorTests
{
    /// <summary>
    /// Contains tests for Injector class
    /// </summary>
    public class Tests
    {
        [Test]
        public void CorrectInjectionTest()
        {
            var assemblyName = typeof(Tests).Assembly.GetName().Name;
            var rootClassName = $"{typeof(RootClass2).FullName}, {assemblyName}";
            var availableTypeNames = new List<string> 
            {
                $"{typeof(B).FullName}, {assemblyName}", 
                $"{typeof(Interface1).FullName}, {assemblyName}" 
            };
            var instance = Injector.Injector.Initialize(rootClassName, availableTypeNames);
            Assert.IsInstanceOf(typeof(RootClass2), instance);
        }

        [Test]
        public void ThrowsExpectedException()
        {
            var assemblyName = typeof(Tests).Assembly.GetName().Name;
            var rootClassName = $"{typeof(RootClass1).FullName}, {assemblyName}";
            var availableTypeNames = new List<string> 
            {
                $"{typeof(A).FullName}, {assemblyName}", 
                $"{typeof(B).FullName}, {assemblyName}",
                $"{typeof(Interface1).FullName}, {assemblyName}"
            };
            Assert.Throws<InjectorException>(() => Injector.Injector.Initialize(rootClassName, availableTypeNames));
        }
    }
}