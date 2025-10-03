using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

namespace Pianola
{
    public class AudioClipProvider : MonoBehaviour
    {
        [SerializeField]
        private AudioClipCollection _clips = null;

        private readonly Dictionary<string, AudioClip> _noteToClip = new();

        private void Awake()
        {
            foreach (var clip in _clips.Clips)
            {
                _noteToClip[clip.name] = clip;
            }
        }

        private async Awaitable Start()
        {
            await Task.WhenAll(_clips.Clips.Select(clip => Task.FromResult(clip.LoadAudioData())));

            Debug.Log("Finished loading audio clips.", this);
        }

        public AudioClip Get(string note)
        {
            return _noteToClip[note];
        }
    }
}
