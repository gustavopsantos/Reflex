using System.Collections.Generic;
using Reflex.Microsoft.Sample.Application;
using Newtonsoft.Json;
using UnityEngine;

namespace Reflex.Microsoft.Sample.Infrastructure
{
    internal class CollectionStoragePrefs : ICollectionStorage
    {
        private readonly HashSet<string> _storage;

        public CollectionStoragePrefs()
        {
			string json = PlayerPrefs.GetString("collection-storage", "[]");
            _storage = JsonConvert.DeserializeObject<HashSet<string>>(json);
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
			string json = JsonConvert.SerializeObject(_storage);
            PlayerPrefs.SetString("collection-storage", json);
        }
    }
}