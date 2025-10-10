using System.Collections.Generic;
using UnityEngine;

namespace Pianola
{
    public class AudioClipProvider : MonoBehaviour
    {
        private readonly Dictionary<int, AudioClip> _noteToClip = new();

        [SerializeField]
        private AudioClipCollection _collection = null;

        public AudioClip this[int noteNumber]
        {
            get => _noteToClip[noteNumber];
            set => _noteToClip[noteNumber] = value;
        }

        public bool IsInitialized { get; private set; } = false;

        private void Awake()
        {
            foreach (var info in _collection.Infos)
            {
                _noteToClip[info.NoteNumber] = info.AudioClip;
            }
        }

        private async Awaitable Start()
        {
            foreach (var info in _collection.Infos)
            {
                var clip = info.AudioClip;

                if (clip.preloadAudioData == false)
                {
                    clip.LoadAudioData();
                }

                while (clip.loadState == AudioDataLoadState.Loading)
                {
                    await Awaitable.EndOfFrameAsync();
                }
            }

            Debug.Log("Finished loading audio clips.", this);

            IsInitialized = true;
        }
    }
}
