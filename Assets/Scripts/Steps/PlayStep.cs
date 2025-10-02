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
                midiStream = new MemoryStream(_midiFileAsset.bytes);
                var success = _midiPlayer.Initialize(midiStream);
                if (success && _playOnEnable)
                {
                    _midiPlayer.Play();
                }
            }
            finally
            {
                midiStream?.Dispose();
            }

            gameObject.SetActive(false); // Done, disable self.
        }
    }
}
