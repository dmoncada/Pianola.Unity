using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class AudioClipProvider : MonoBehaviour
{
    [SerializeField]
    private AudioClip[] _clips = new AudioClip[0];

    private readonly Dictionary<string, AudioClip> _noteClips = new();

    private void Awake()
    {
        foreach (var clip in _clips)
        {
            _noteClips[clip.name] = clip;
        }
    }

    private async Awaitable Start()
    {
        await Task.WhenAll(_clips.Select(clip => Task.FromResult(clip.LoadAudioData())));

        Debug.Log("Finished loading audio clips.", this);
    }

    public AudioClip Get(string note)
    {
        return _noteClips[note];
    }
}
