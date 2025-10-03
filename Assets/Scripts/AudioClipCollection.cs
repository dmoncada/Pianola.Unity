using UnityEngine;

namespace Pianola
{
    [CreateAssetMenu(fileName = nameof(AudioClipCollection), menuName = "SO/AudioClipCollection")]
    public class AudioClipCollection : ScriptableObject
    {
        [SerializeField]
        private AudioClip[] _clips = new AudioClip[0];
        public AudioClip[] Clips => _clips;
    }
}
