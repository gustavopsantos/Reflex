//using FluentAssertions;
//using Microsoft.Extensions.DependencyInjection;
//using NUnit.Framework;
//using System;

//namespace Reflex.Tests
//{
//    internal class SelfInjectionTests
//    {
//        [Test]
//        public void Container_ShouldBeAbleToResolveItself()
//        {
//            var container = new ServiceCollection()();
//            container.GetService<IServiceProvider>().Should().Be(container);
//        }
//    }
//}