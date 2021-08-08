using System;

public interface ICollectableRegistry
{
    event Action OnValueChanged;
    void Clear();
    int CollectionCount();
    bool Contains(string collectableId);
    void Register(string collectableId);
}