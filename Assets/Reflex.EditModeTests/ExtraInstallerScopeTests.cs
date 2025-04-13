using FluentAssertions;
using NUnit.Framework;
using Reflex.Core;
using Reflex.Enums;

namespace Reflex.EditModeTests
{
    public class ExtraInstallerScopeTests
    {
        [Test]
        public void ExtraInstallerScope_WillInstallOnAnyNewContainer_WhileItsNotDisposed()
        {
            using (new ExtraInstallerScope(b => b.RegisterValue(95, Lifetime.Singleton)))
            {
                var containerOneWithExtraInstaller = new ContainerBuilder().Build();
                containerOneWithExtraInstaller.Single<int>().Should().Be(95);
                
                var containerTwoWithExtraInstaller = new ContainerBuilder().Build();
                containerTwoWithExtraInstaller.Single<int>().Should().Be(95);
            }
            
            var containerWithoutExtraInstaller = new ContainerBuilder().Build();
            containerWithoutExtraInstaller.HasBinding<int>().Should().BeFalse();
        }
    }
}