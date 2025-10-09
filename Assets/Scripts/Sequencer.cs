using System.Collections.Generic;
using UnityEngine;

namespace Pianola
{
    public class Sequencer : MonoBehaviour
    {
        private readonly List<GameObject> _sequenceItems = new();

        [SerializeField]
        private int _loopCount = 1;

        private int _currentIndex = -1;

        private void OnEnable()
        {
            Initialize();

            Advance();
        }

        private void Update()
        {
            if (_currentIndex < 0 || _currentIndex >= _sequenceItems.Count)
            {
                return;
            }

            if (_sequenceItems[_currentIndex].activeSelf == false)
            {
                Advance();
            }
        }

        private void Initialize()
        {
            _sequenceItems.Clear();
            _currentIndex = -1;

            for (int i = 0; i < transform.childCount; i++)
            {
                var child = transform.GetChild(i).gameObject;
                _sequenceItems.Add(child);
                child.SetActive(false);
            }
        }

        public void Advance()
        {
            if (0 <= _currentIndex && _currentIndex < _sequenceItems.Count)
            {
                Debug.LogFormat("Deactivating: {0} ...", _sequenceItems[_currentIndex].name, this);

                _sequenceItems[_currentIndex].SetActive(false);
            }

            if (++_currentIndex >= _sequenceItems.Count)
            {
                if (--_loopCount > 0)
                {
                    _currentIndex = 0;
                }
                else
                {
                    gameObject.SetActive(false);

                    return; // End of sequence
                }
            }

            if (_currentIndex < _sequenceItems.Count)
            {
                Debug.LogFormat("Activating: {0} ...", _sequenceItems[_currentIndex].name, this);

                _sequenceItems[_currentIndex].SetActive(true);
            }
        }
    }
}
