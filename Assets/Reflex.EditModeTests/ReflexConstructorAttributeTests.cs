using FluentAssertions;
using NUnit.Framework;
using Reflex.Attributes;
using Reflex.Core;

namespace Reflex.EditModeTests
{
    public class ReflexConstructorAttributeTests
    {
        private class ClassWithReflexConstructorIdentifiedByMaxAmountOfArguments
        {
            public bool ConstructedWithEmptyConstructor { get; }
            public bool ConstructedWithNonEmptyConstructor { get; }
            
            public ClassWithReflexConstructorIdentifiedByMaxAmountOfArguments()
            {
                ConstructedWithEmptyConstructor = true;
            }
            
            public ClassWithReflexConstructorIdentifiedByMaxAmountOfArguments(object a, int b)
            {
                ConstructedWithNonEmptyConstructor = true;
            }
        }

        private class ClassWithReflexConstructorIdentifiedByReflexConstructorAttribute
        {
            public bool ConstructedWithEmptyConstructor { get; }
            public bool ConstructedWithNonEmptyConstructor { get; }
            
            [ReflexConstructor]
            public ClassWithReflexConstructorIdentifiedByReflexConstructorAttribute()
            {
                ConstructedWithEmptyConstructor = true;
            }
            
            public ClassWithReflexConstructorIdentifiedByReflexConstructorAttribute(object a, int b)
            {
                ConstructedWithNonEmptyConstructor = true;
            }
        }

        [Test]
        public void ClassWithMultipleConstructors_WithoutAnyReflexConstructorAttribute_ShouldBeConstructedUsing_ConstructorWithMostArguments()
        {
            var container = new ContainerBuilder()
                .Add(Singleton.FromValue(new object()))
                .Add(Singleton.FromValue(42))
                .Build();

            var result = container.Construct<ClassWithReflexConstructorIdentifiedByMaxAmountOfArguments>();
            result.ConstructedWithEmptyConstructor.Should().BeFalse();
            result.ConstructedWithNonEmptyConstructor.Should().BeTrue();
        }
        
        [Test]
        public void ClassWithMultipleConstructors_WithOneDefiningReflexConstructorAttribute_ShouldBeConstructedUsing_ConstructorWithReflexConstructorAttribute()
        {
            var container = new ContainerBuilder()
                .Add(Singleton.FromValue(new object()))
                .Add(Singleton.FromValue(42))
                .Build();

            var result = container.Construct<ClassWithReflexConstructorIdentifiedByReflexConstructorAttribute>();
            result.ConstructedWithEmptyConstructor.Should().BeTrue();
            result.ConstructedWithNonEmptyConstructor.Should().BeFalse();
        }
    }
}