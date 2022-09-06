using System;
using Reflex;
using UnityEngine;

public class BindingsTest : MonoBehaviour
{
    public interface ITimeSource
    {
        DateTime Get();
    }

    public class UtcTimeSource : ITimeSource
    {
        public DateTime Get() => DateTime.UtcNow;
    }

    public class LocalTimeSource : ITimeSource
    {
        public DateTime Get() => DateTime.Now;
    }

    private void Start()
    {
        var container = new Container();
        container.BindFunction<ITimeSource>(() => new  UtcTimeSource());
    }
}