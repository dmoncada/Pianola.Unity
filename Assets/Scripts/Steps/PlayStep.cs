using System;
using System.IO;
using UnityEngine;

namespace Pianola
{
    public class PlayStep : MonoBehaviour
    {
        [SerializeField]
        private MidiPlayer _midiPlayer = null;

        [SerializeField]
        private TextAsset _midiFileAsset = null;

        [SerializeField]
        private bool _playOnEnable = true;

        private void OnEnable()
        {
            Stream midiStream = null;
            try
            {
                Debug.LogFormat(
                    "Loading text asset: {0}, size: {1} bytes",
                    _midiFileAsset.name,
                    _midiFileAsset.dataSize,
                    this
                );

                midiStream = new MemoryStream(_midiFileAsset.bytes);
                var success = _midiPlayer.Initialize(midiStream);
                if (success && _playOnEnable)
                {
                    _midiPlayer.Play();
                }
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }
            finally
            {
                midiStream?.Dispose();
            }

            gameObject.SetActive(false); // Done, disable self.
        }
    }
}
