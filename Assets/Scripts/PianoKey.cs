using System.Collections;
using UnityEngine;

namespace Pianola
{
    [RequireComponent(typeof(Renderer))]
    public class PianoKey : MonoBehaviour
    {
        [SerializeField]
        private float _pressAngle = -5f;

        [SerializeField]
        private float _pressDuration = 0.1f;

        [SerializeField]
        private float _fadeDuration = 0.25f;

        private Renderer _renderer = null;
        private AudioSource _activeSource = null;
        public AudioSource ActiveSource => _activeSource;

        private float _currentAngle = 0f;
        private float _targetAngle = 0f;
        private Color _initialColor = default;
        private Color _targetColor = default;

        private AudioClip _clip = null;
        private AudioSourcePool _sourcePool = null;
        private bool _isInitialized = false;

        public bool IsPressed => _currentAngle < 0f;

        private void Awake()
        {
            _renderer = GetComponent<Renderer>();

            _initialColor = _renderer.material.color;
        }

        private void Update()
        {
            if (Mathf.Abs(_currentAngle - _targetAngle) > Mathf.Epsilon)
            {
                UpdateRotation();
                UpdateColor();
            }
        }

        public void Initialize(AudioClip clip, AudioSourcePool pool)
        {
            _clip = clip;
            _sourcePool = pool;
            _isInitialized = true;
        }

        public void Press(int channel = 0)
        {
            if (_isInitialized == false || _activeSource != null)
            {
                return;
            }

            _targetAngle = _pressAngle;

            _targetColor = channel == 0 ? Color.yellow : Color.cyan;

            _activeSource = _sourcePool.Get();
            _activeSource.clip = _clip;
            _activeSource.Play();
        }

        public void Release()
        {
            if (_isInitialized == false || _activeSource == null)
            {
                return;
            }

            _targetAngle = 0f;

            StartCoroutine(FadeAndRelease(_activeSource, _sourcePool, _fadeDuration));

            _activeSource = null;
        }

        private void UpdateRotation()
        {
            var angleDiff = _targetAngle - _currentAngle;
            var velocity = angleDiff / _pressDuration;
            var travelDelta = Mathf.Abs(velocity) * Time.deltaTime;
            _currentAngle = Mathf.MoveTowards(_currentAngle, _targetAngle, travelDelta);
            transform.rotation = Quaternion.Euler(_currentAngle, 0f, 0f);
        }

        private void UpdateColor()
        {
            var travelAmount = Mathf.InverseLerp(0f, _pressAngle, _currentAngle);
            _renderer.material.color = Color.Lerp(_initialColor, _targetColor, travelAmount);
        }

        private IEnumerator FadeAndRelease(AudioSource source, AudioSourcePool pool, float duration)
        {
            float elapsed = 0f;
            float startVolume = source.volume;

            while (elapsed < duration)
            {
                elapsed += Time.deltaTime;
                source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
                yield return new WaitForEndOfFrame();
            }

            pool.Release(source);
        }
    }
}
