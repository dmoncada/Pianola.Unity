using UnityEngine;
using UnityEngine.Pool;

namespace Pianola
{
    public class AudioSourcePool : MonoBehaviour
    {
        [SerializeField]
        private AudioSource _audioSourcePrefab = null;

        [SerializeField]
        private bool _checkCollection = true;

        [SerializeField]
        private int _initialCapacity = 30;

        [SerializeField]
        private int _maxSize = 100;

        private IObjectPool<AudioSource> _pool = null;

        private void Awake()
        {
            _pool = new ObjectPool<AudioSource>(
                Create,
                OnGet,
                OnRelease,
                (_) => { },
                _checkCollection,
                _initialCapacity,
                _maxSize
            );
        }

        private AudioSource Create()
        {
            return Instantiate(_audioSourcePrefab, transform);
        }

        public AudioSource Get()
        {
            return _pool.Get();
        }

        public void Release(AudioSource source)
        {
            _pool.Release(source);
        }

        private void OnGet(AudioSource source)
        {
            source.gameObject.SetActive(true);
        }

        private void OnRelease(AudioSource source)
        {
            source.Stop();
            source.clip = null;
            source.volume = 1f;
            source.gameObject.SetActive(false);
        }
    }
}
