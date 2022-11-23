using Reflex.Sample.Application;
using Reflex.Scripts.Attributes;
using UnityEngine;

namespace Reflex.Sample.Infrastructure
{
    public class Collectable : MonoBehaviour
    {
        [SerializeField] private string _id;
        [Inject] private ICollectionStorage _collectionStorage;
        [Inject] private PickupSoundEffect _pickupSoundEffectPrefab;
        
        private void Start()
        {
            gameObject.SetActive(!_collectionStorage.IsCollected(_id));
        }

        public void Collect()
        {
            gameObject.SetActive(false);
            _collectionStorage.Add(_id);
            Instantiate(_pickupSoundEffectPrefab);
        }
    }
}