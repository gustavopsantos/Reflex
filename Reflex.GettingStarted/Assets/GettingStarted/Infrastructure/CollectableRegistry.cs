using System;
using UnityEngine;
using Newtonsoft.Json;
using System.Collections.Generic;

public class CollectableRegistry : ICollectableRegistry
{
    public event Action OnValueChanged = delegate {  };

    public void Clear()
    {
        PlayerPrefs.SetString(nameof(CollectableRegistry), JsonConvert.SerializeObject(new HashSet<string>()));
        OnValueChanged.Invoke();
    }

    public bool Contains(string collectableId)
    {
        return FetchRegistry().Contains(collectableId);
    }

    public void Register(string collectableId)
    {
        var registry = FetchRegistry();
        registry.Add(collectableId);
        Persist(registry);
        OnValueChanged.Invoke();
    }

    public int CollectionCount()
    {
        return FetchRegistry().Count;
    }

    private static HashSet<string> FetchRegistry()
    {
        var json = PlayerPrefs.GetString(nameof(CollectableRegistry), JsonConvert.SerializeObject(new HashSet<string>()));
        return JsonConvert.DeserializeObject<HashSet<string>>(json);
    }

    private static void Persist(HashSet<string> registry)
    {
        PlayerPrefs.SetString(nameof(CollectableRegistry), JsonConvert.SerializeObject(registry));
    }
}