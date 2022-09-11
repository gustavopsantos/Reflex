using System;
using EasyButtons;
using Reflex;
using UnityEngine;

public class Sandbox : MonoBehaviour
{
    public class Foo : IDisposable
    {
        public string Value { get; } = "Foo";

        public Foo()
        {
            Debug.Log($"Constructing {GetType().Name}");
        }

        public void Dispose()
        {
            Debug.Log($"Disposing {GetType().Name}");
        }
    }

    public class DependsOnFoo : IDisposable
    {
        public string Value { get; } = "DependsOnFoo";

        public DependsOnFoo(Foo foo)
        {
            Debug.Log($"Constructing {GetType().Name}");
        }

        public void Dispose()
        {
            Debug.Log($"Disposing {GetType().Name}");
        }
    }

    [Button]
    private void Run()
    {
        using var outer = new Container();
        outer.BindSingleton<Foo, Foo>();
        Debug.Log(outer.Resolve<Foo>().Value);

        using var inner = outer.Scope();
        inner.BindSingleton<DependsOnFoo, DependsOnFoo>();

        Debug.Log(inner.Resolve<Foo>().Value);
        Debug.Log(inner.Resolve<DependsOnFoo>().Value);
        Debug.Log(inner.Resolve<DependsOnFoo>().Value);
    }
}