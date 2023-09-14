using System;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Reflectors;

namespace Reflex.Tests
{
    public class ActivatorFactoryTests
    {
        [Test]
        [TestCase(typeof(MonoActivatorFactory))]
        [TestCase(typeof(IL2CPPActivatorFactory))]
        public void CanActivate_ValueType(Type activatorFactoryType)
        {
			IActivatorFactory activatorFactory = (IActivatorFactory) Activator.CreateInstance(activatorFactoryType);
			Delegates.ObjectActivator activator = activatorFactory.GenerateDefaultActivator(typeof(int));
			int number = (int) activator.Invoke(null);
            number.Should().Be(default(int));
        }
        
        [Test]
        [TestCase(typeof(MonoActivatorFactory))]
        [TestCase(typeof(IL2CPPActivatorFactory))]
        public void CanActivate_ReferenceType(Type activatorFactoryType)
        {
			IActivatorFactory activatorFactory = (IActivatorFactory) Activator.CreateInstance(activatorFactoryType);
			Delegates.ObjectActivator activator = activatorFactory.GenerateActivator(typeof(string), typeof(string).GetConstructor(new[] {typeof(char[])}), new[] {typeof(char[])});
			string complex = (string) activator.Invoke(Array.Empty<char>());
            complex.Should().NotBeNull();
            complex.Should().Be(string.Empty);
        }
    }
}