using System.Collections.Generic;
using Reflex.Sample.Application;
using Newtonsoft.Json;
using Newtonsoft.Json.Utilities;
using UnityEngine;
using UnityEngine.Scripting;

namespace Reflex.Sample.Infrastructure
{
    internal class CollectionStoragePrefs : ICollectionStorage
    {
        private readonly HashSet<string> _storage;

        [Preserve]
        private static void FixDeserializeObject() // https://discussions.unity.com/t/unable-to-deserialize-hashsets/807067
        {
            AotHelper.EnsureList<string>();
        }
        
        public CollectionStoragePrefs()
        {
            var json = PlayerPrefs.GetString("collection-storage", "[]");
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
            var json = JsonConvert.SerializeObject(_storage);
            PlayerPrefs.SetString("collection-storage", json);
        }
    }
}