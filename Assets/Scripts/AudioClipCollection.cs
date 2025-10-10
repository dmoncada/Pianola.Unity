using System;
using System.Collections.Generic;
using UnityEngine;

namespace Pianola
{
    [CreateAssetMenu(fileName = nameof(AudioClipCollection), menuName = "SO/AudioClipCollection")]
    public class AudioClipCollection : ScriptableObject
    {
        [SerializeField]
        private List<AudioClipInfo> _infos = new();
        public IReadOnlyList<AudioClipInfo> Infos => _infos;

        public void SetClips(IEnumerable<AudioClip> clips)
        {
            _infos.Clear();

            foreach (var clip in clips)
            {
                var noteNumber = int.TryParse(clip.name, out int note) ? note : 0;

                _infos.Add(new() { NoteNumber = noteNumber, AudioClip = clip });
            }
        }
    }

    [Serializable]
    public struct AudioClipInfo
    {
        public int NoteNumber;
        public AudioClip AudioClip;
    }
}
