using System;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Caching;
using Reflex.Reflectors;

namespace Reflex.EditModeTests
{
    public class ActivatorFactoryTests
    {
        [Test]
        [TestCase(typeof(MonoActivatorFactory))]
        [TestCase(typeof(IL2CPPActivatorFactory))]
        public void CanActivate_ValueType(Type activatorFactoryType)
        {
            var activatorFactory = (IActivatorFactory) Activator.CreateInstance(activatorFactoryType);
            var activator = activatorFactory.GenerateDefaultActivator(typeof(int));
            var number = (int) activator.Invoke(null);
            number.Should().Be(default(int));
        }
        
        [Test]
        [TestCase(typeof(MonoActivatorFactory))]
        [TestCase(typeof(IL2CPPActivatorFactory))]
        public void CanActivate_ReferenceType(Type activatorFactoryType)
        {
            var activatorFactory = (IActivatorFactory) Activator.CreateInstance(activatorFactoryType);
            var activator = activatorFactory.GenerateActivator(typeof(string), typeof(string).GetConstructor(new[] {typeof(char[])}), new[] {new MethodParamInfo(typeof(char[]))});
            var complex = (string) activator.Invoke(Array.Empty<char>());
            complex.Should().NotBeNull();
            complex.Should().Be(string.Empty);
        }
    }
}