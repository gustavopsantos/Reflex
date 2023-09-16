//using System;
//using FluentAssertions;
//using Reflex.Microsoft.Core;
//using NUnit.Framework;

//namespace Reflex.Microsoft.Tests
//{
//    internal class DisposeTests
//    {
//        private class Service : IDisposable
//        {
//            public int Disposed { get; private set; }

//            public void Dispose()
//            {
//                Disposed++;
//            }
//        }

//        [Test]
//        public void Singleton_ShouldBeDisposed_WhenOwnerIsDisposed()
//        {
//            var container = new ContainerDescriptor("")
//                .AddSingleton(typeof(Service), typeof(Service))
//                .Build();
            
//            var service = container.Single<Service>();
//            container.Dispose();
//            service.Disposed.Should().Be(1);
//        }

//        [Test]
//        public void Transient_ShouldBeDisposed_WhenOwnerIsDisposed()
//        {
//            var container = new ContainerDescriptor("")
//                .AddTransient(typeof(Service), typeof(Service))
//                .Build();
            
//            var service = container.Single<Service>();
//            container.Dispose();
//            service.Disposed.Should().Be(1);
//        }

//        [Test]
//        public void Instance_ShouldBeDisposed_WhenOwnerIsDisposed()
//        {
//            var service = new Service();
//            var container = new ContainerDescriptor("")
//                .AddInstance(service, typeof(Service))
//                .Build();
            
//            container.Dispose();
//            service.Disposed.Should().Be(1);
//        }
//    }
//}