using UnityEngine;
using UnityEngine.InputSystem;

public class PianoKeyDriver : MonoBehaviour
{
    private readonly Key _triggerKey = Key.P;

    [SerializeField]
    private GameObject _pianoKeysRoot = null;

    private int _currentIndex = 0;
    private PianoKey _currentKey = null;
    private PianoKey[] _pianoKeys = null;

    private void Awake()
    {
        if (_pianoKeysRoot == null)
        {
            _pianoKeysRoot = gameObject;
        }

        _pianoKeys = _pianoKeysRoot.GetComponentsInChildren<PianoKey>();

        _currentKey = _pianoKeys[_currentIndex];
    }

    private void Update()
    {
        if (GetKeyDown(Key.Comma))
        {
            if (--_currentIndex < 0)
            {
                _currentIndex = _pianoKeys.Length - 1;
            }

            _currentKey.Release();
            _currentKey = _pianoKeys[_currentIndex];
        }

        if (GetKeyDown(Key.Period))
        {
            if (++_currentIndex >= _pianoKeys.Length)
            {
                _currentIndex = 0;
            }

            _currentKey.Release();
            _currentKey = _pianoKeys[_currentIndex];
        }

        if (GetKeyDown(_triggerKey))
        {
            _currentKey.Press();
        }

        if (GetKeyUp(_triggerKey))
        {
            _currentKey.Release();
        }
    }

    private bool GetKeyUp(Key key)
    {
        return Keyboard.current[key].wasReleasedThisFrame;
    }

    private bool GetKeyDown(Key key)
    {
        return Keyboard.current[key].wasPressedThisFrame;
    }
}
