using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AudioClipProvider : MonoBehaviour
{
    public static AudioClipProvider Instance = null;

    [SerializeField]
    private AudioClip[] _clips = new AudioClip[0];

    private readonly Dictionary<string, AudioClip> _noteClips = new();

    public bool IsLoaded { get; private set; } = false;

    private void Awake()
    {
        Instance = this;

        foreach (var clip in _clips)
        {
            _noteClips[clip.name] = clip;
        }
    }

    private IEnumerator Start()
    {
        foreach (var clip in _clips)
        {
            while (clip.loadState == AudioDataLoadState.Loading)
            {
                yield return new WaitForEndOfFrame();
            }
        }

        Debug.Log("Finished loading audio clips.");

        IsLoaded = true;
    }

    public AudioClip Get(string note)
    {
        return _noteClips[note];
    }
}
