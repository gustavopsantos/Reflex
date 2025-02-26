using System.Threading;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Injectors;

namespace Reflex.EditModeTests
{
    public class ConstructorInjectorTests
    {
        class ServiceWithOneConstructorDependency
        {
            public ServiceWithOneConstructorDependency(int number1)
            {
            }
        }
        
        class ServiceWithEightConstructorDependency
        {
            public ServiceWithEightConstructorDependency(
                int number1,
                int number2,
                int number3,
                int number4,
                int number5,
                int number6,
                int number7,
                int number8)
            {
            }
        }
        
        class ServiceWithSixteenConstructorDependency
        {
            public ServiceWithSixteenConstructorDependency(
                int number1,
                int number2,
                int number3,
                int number4,
                int number5,
                int number6,
                int number7,
                int number8,
                int number9,
                int number10,
                int number11,
                int number12,
                int number13,
                int number14,
                int number15,
                int number16)
            {
            }
        }
        
        class ServiceWithThirtyTwoConstructorDependency
        {
            public ServiceWithThirtyTwoConstructorDependency(
                int number1,
                int number2,
                int number3,
                int number4,
                int number5,
                int number6,
                int number7,
                int number8,
                int number9,
                int number10,
                int number11,
                int number12,
                int number13,
                int number14,
                int number15,
                int number16,
                int number17,
                int number18,
                int number19,
                int number20,
                int number21,
                int number22,
                int number23,
                int number24,
                int number25,
                int number26,
                int number27,
                int number28,
                int number29,
                int number30,
                int number31,
                int number32)
            {
            }
        }
        
        [Test]
        public void PoolSizeShouldGrowAutomatically()
        {
            var container = new ContainerBuilder()
                .AddSingleton(42)
                .Build();

            void ConstructTypeInThreadAndAssertPoolSize<T>(int expectedPoolSize)
            {
                var thread = new Thread(() =>
                {
                    container.Construct<T>();
                    ConstructorInjector.ArrayPool.GetPoolSize().Should().Be(expectedPoolSize);
                });
                
                thread.Start();
                thread.Join();
            }
            
            ConstructTypeInThreadAndAssertPoolSize<ServiceWithThirtyTwoConstructorDependency>(33);
            ConstructTypeInThreadAndAssertPoolSize<ServiceWithSixteenConstructorDependency>(17);
            ConstructTypeInThreadAndAssertPoolSize<ServiceWithEightConstructorDependency>(16);
            ConstructTypeInThreadAndAssertPoolSize<ServiceWithOneConstructorDependency>(16);
            ConstructorInjector.ArrayPool.GetPoolSize().Should().Be(16); // Main thread
        }
    }
}