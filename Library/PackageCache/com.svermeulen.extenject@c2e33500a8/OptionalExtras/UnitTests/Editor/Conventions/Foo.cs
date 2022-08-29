#if !(UNITY_WSA && ENABLE_DOTNET)

using System;

namespace Zenject.Tests.Convention
{
    public class ConventionTestAttribute : Attribute
    {
        public ConventionTestAttribute(int num)
        {
            Num = num;
        }

        public int Num
        {
            get;
            private set;
        }
    }

    public interface IFoo
    {
    }

    public class Foo1 : IFoo
    {
    }

    [ConventionTest(0)]
    public class Foo2 : IFoo
    {
    }

    [ConventionTest(1)]
    public class Foo3 : IFoo
    {
    }
}

#endif
