using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using Reflex.Attributes;
using Reflex.Caching;

namespace Reflex.EditModeTests
{
    public class TypeInfoCacheTests
    {
        #region Public

        private class HasPublicField
        {
            [Inject] public int Number;
        }

        private class HasPublicProperty
        {
            [Inject] public int Number { get; private set; }
        }

        private class HasPublicMethod
        {
            [Inject]
            public void Inject(int number)
            {
            }
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PublicFields()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasPublicField));
            typeInfo.InjectableFields.Single().Name.Should().Be("Number");
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PublicProperties()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasPublicProperty));
            typeInfo.InjectableProperties.Single().Name.Should().Be("Number");
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PublicMethods()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasPublicMethod));
            typeInfo.InjectableMethods.Single().MethodInfo.Name.Should().Be("Inject");
        }

        #endregion

        #region Protected

        private class HasProtectedField
        {
            [Inject] protected int Number;
        }

        private class HasProtectedProperty
        {
            [Inject] protected int Number { get; private set; }
        }

        private class HasProtectedMethod
        {
            [Inject]
            protected void Inject(int number)
            {
            }
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_ProtectedFields()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasProtectedField));
            typeInfo.InjectableFields.Single().Name.Should().Be("Number");
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_ProtectedProperties()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasProtectedProperty));
            typeInfo.InjectableProperties.Single().Name.Should().Be("Number");
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_ProtectedMethods()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasProtectedMethod));
            typeInfo.InjectableMethods.Single().MethodInfo.Name.Should().Be("Inject");
        }

        #endregion

        #region Private

        private class HasPrivateField
        {
            [Inject] private int _number;
        }

        private class HasPrivateProperty
        {
            [Inject] private int Number { get; set; }
        }

        private class HasPrivateMethod
        {
            [Inject]
            private void Inject(int number)
            {
            }
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PrivateFields()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasPrivateField));
            typeInfo.InjectableFields.Single().Name.Should().Be("_number");
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PrivateProperties()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasPrivateProperty));
            typeInfo.InjectableProperties.Single().Name.Should().Be("Number");
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PrivateMethods()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasPrivateMethod));
            typeInfo.InjectableMethods.Single().MethodInfo.Name.Should().Be("Inject");
        }

        #endregion

        #region Public Inherited

        private class HasParentWithPublicField : HasPublicField
        {
        }

        private class HasParentWithPublicProperty : HasPublicProperty
        {
        }

        private class HasParentWithPublicMethod : HasPublicMethod
        {
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PublicFields_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithPublicField));
            typeInfo.InjectableFields.Single().Name.Should().Be("Number");
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PublicProperties_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithPublicProperty));
            typeInfo.InjectableProperties.Single().Name.Should().Be("Number");
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PublicMethods_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithPublicMethod));
            typeInfo.InjectableMethods.Single().MethodInfo.Name.Should().Be("Inject");
        }

        #endregion

        #region Protected Inherited

        private class HasParentWithProtectedField : HasProtectedField
        {
        }

        private class HasParentWithProtectedProperty : HasProtectedProperty
        {
        }

        private class HasParentWithProtectedMethod : HasProtectedMethod
        {
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_ProtectedFields_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithProtectedField));
            typeInfo.InjectableFields.Single().Name.Should().Be("Number");
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_ProtectedProperties_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithProtectedProperty));
            typeInfo.InjectableProperties.Single().Name.Should().Be("Number");
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_ProtectedMethods_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithProtectedMethod));
            typeInfo.InjectableMethods.Single().MethodInfo.Name.Should().Be("Inject");
        }

        #endregion

        #region Private Inherited

        private class HasParentWithPrivateField : HasPrivateField
        {
        }

        private class HasParentWithPrivateProperty : HasPrivateProperty
        {
        }

        private class HasParentWithPrivateMethod : HasPrivateMethod
        {
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PrivateFields_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithPrivateField));
            typeInfo.InjectableFields.Single().Name.Should().Be("_number");
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PrivateProperties_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithPrivateProperty));
            typeInfo.InjectableProperties.Single().Name.Should().Be("Number");
        }

        [Test]
        public void Get_ShouldReturn_AllInjectable_PrivateMethods_UpInHierarchy()
        {
            var typeInfo = TypeInfoCache.Get(typeof(HasParentWithPrivateMethod));
            typeInfo.InjectableMethods.Single().MethodInfo.Name.Should().Be("Inject");
        }

        #endregion
    }
}