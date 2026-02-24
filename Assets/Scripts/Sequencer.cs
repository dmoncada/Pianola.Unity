using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pianola
{
    public class Sequencer : MonoBehaviour
    {
        private readonly List<GameObject> _sequenceItems = new();

        private float _startTime = 0f;

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
            if (_currentIndex.InRange(0, _sequenceItems.Count) == false)
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
            _startTime = Time.realtimeSinceStartup;
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
            if (_currentIndex.InRange(0, _sequenceItems.Count))
            {
                var elapsed = Time.realtimeSinceStartup - _startTime;

                Debug.LogFormat(
                    "Deactivating: {0}, time spent in step: {1}",
                    _sequenceItems[_currentIndex].name,
                    Utils.FormatTime(TimeSpan.FromSeconds(elapsed)),
                    this
                );

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

                    return; // End of sequence.
                }
            }

            if (_currentIndex < _sequenceItems.Count)
            {
                Debug.LogFormat("Activating: {0} ...", _sequenceItems[_currentIndex].name, this);

                _sequenceItems[_currentIndex].SetActive(true);

                _startTime = Time.realtimeSinceStartup;
            }
        }
    }
}
