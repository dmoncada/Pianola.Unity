using System.Collections;
using UnityEngine;

[RequireComponent(typeof(Renderer))]
public class PianoKey : MonoBehaviour
{
    [SerializeField]
    private int _midiNoteNumber = 0;

    public int MidiNoteNumber
    {
        get { return _midiNoteNumber; }
        set { _midiNoteNumber = value; }
    }

    [SerializeField]
    private float _pressAngle = -5f;

    [SerializeField]
    private float _pressDuration = 0.1f;

    [SerializeField]
    private float _fadeDuration = 0.25f;

    private float _currentAngle = 0f;
    private float _targetAngle = 0f;
    private Renderer _renderer = null;
    private AudioSource _activeSource = null;
    private Color _initialColor = default;
    private Color _targetColor = default;

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

    public void Press(int channel = 0)
    {
        if (_activeSource != null)
        {
            return;
        }

        _targetAngle = _pressAngle;

        _targetColor = channel == 0 ? Color.yellow : Color.cyan;

        var clip = AudioClipProvider.Instance.Get(_midiNoteNumber.ToString());
        _activeSource = AudioSourcePool.Instance.Get();
        _activeSource.clip = clip;
        _activeSource.Play();
    }

    public void Release()
    {
        if (_activeSource == null)
        {
            return;
        }

        _targetAngle = 0f;

        StartCoroutine(FadeAndRelease(_activeSource, _fadeDuration));

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

    private IEnumerator FadeAndRelease(AudioSource source, float duration)
    {
        float elapsed = 0f;
        float startVolume = source.volume;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            source.volume = Mathf.Lerp(startVolume, 0f, elapsed / duration);
            yield return new WaitForEndOfFrame();
        }

        AudioSourcePool.Instance.Release(source);
    }
}
