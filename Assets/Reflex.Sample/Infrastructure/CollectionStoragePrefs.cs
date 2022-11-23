using System.Collections.Generic;
using Newtonsoft.Json;
using Reflex.Sample.Application;
using UnityEngine;

namespace Reflex.Sample.Infrastructure
{
    public class CollectionStoragePrefs : ICollectionStorage
    {
        private readonly HashSet<string> _storage;

        public CollectionStoragePrefs()
        {
            var json = PlayerPrefs.GetString("collection-storage", "[]");
            _storage = JsonConvert.DeserializeObject<HashSet<string>>(json); // TODO upgrade unity version and  then, rely on newtonsoft json from unity package manager
        }

        public void Clear()
        {
            _storage.Clear();
            Persist();
        }

        public void Add(string id)
        {
            _storage.Add(id);
            Persist();
        }
        
        public int Count()
        {
            return _storage.Count;
        }

        public bool IsCollected(string id)
        {
            return _storage.Contains(id);
        }
        
        private void Persist()
        {
            var json = JsonConvert.SerializeObject(_storage);
            PlayerPrefs.SetString("collection-storage", json);
        }
    }
}