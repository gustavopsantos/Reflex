using UnityEngine;

public class PickupSoundEffect : MonoBehaviour
{
    [SerializeField, Min(1f)] private float _lifeTime;
    [SerializeField, Min(0.1f)] private float _pitchVariation;
    [SerializeField] private AudioSource _audioSource;

    private void Start()
    {
        Destroy(gameObject, _lifeTime);
        _audioSource.pitch = 1f + Random.Range(-_pitchVariation, +_pitchVariation);
        _audioSource.Play();
    }
}