using NUnit.Framework;
using FluentAssertions;
using Reflex.Scripts.Core;

namespace Reflex.Tests
{
    public class ContainerStackTests
    {
        [Test]
        public void ContainerStack_WithoutOverride_ShouldReturnNonOverridenValue()
        {
            var stack = new ContainerStack();
            stack.PushNew();
            {
                stack.BindSingleton("default");
                stack.Resolve<string>().Should().Be("default");
            }
        }

        [Test]
        public void ContainerStack_WithOverride_ShouldReturnOverridenValue()
        {
            var stack = new ContainerStack();
            stack.PushNew();
            {
                stack.BindSingleton("default");

                stack.PushNew();
                {
                    stack.BindSingleton("override");
                    stack.Resolve<string>().Should().Be("override");
                }
            }
        }

        [Test]
        public void ContainerStack_WithNestedContainer_WithValueOnlyOnRoot_ShouldBeAbleToTracebackAndReturnRootValue()
        {
            var stack = new ContainerStack();
            stack.PushNew();
            {
                stack.BindSingleton("default");

                stack.PushNew();
                {
                    stack.Resolve<string>().Should().Be("default");
                }
            }
        }

        [Test]
        public void ContainerStack_WithNestedContainerOverridingValue_ShouldReturnRootValueAfterPop()
        {
            var stack = new ContainerStack();
            stack.PushNew();
            {
                stack.BindSingleton("default");

                stack.PushNew();
                {
                    stack.BindSingleton("override");
                    stack.Resolve<string>().Should().Be("override");
                }
                stack.Pop();

                stack.Resolve<string>().Should().Be("default");
            }
        }
    }
}