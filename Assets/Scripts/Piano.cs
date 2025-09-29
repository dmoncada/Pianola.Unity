using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using UnityEngine;

[RequireComponent(typeof(AudioClipProvider))]
[RequireComponent(typeof(AudioSourcePool))]
public class Piano : MonoBehaviour
{
    private const int NumPianoKeys = 88;
    private const int FirstMidiNote = 21;

    private AudioClipProvider _clipProvider = null;
    private AudioSourcePool _sourcePool = null;

    private readonly Dictionary<int, PianoKey> _noteToKey = new();

    private void Awake()
    {
        _clipProvider = GetComponent<AudioClipProvider>();

        _sourcePool = GetComponent<AudioSourcePool>();

        var keys = GetComponentsInChildren<PianoKey>();

        Debug.Assert(keys.Length == NumPianoKeys, "Different number of piano keys than expected.");

        int midiNote = FirstMidiNote;

        foreach (var key in keys)
        {
            key.Initialize(midiNote, _sourcePool, _clipProvider);
            _noteToKey[midiNote] = key;
            midiNote += 1;
        }
    }

    public void OnMidiEvent(MidiEvent midiEvent)
    {
        if (midiEvent is NoteOnEvent noteOn)
        {
            if (_noteToKey.TryGetValue(noteOn.NoteNumber, out var key))
            {
                if (noteOn.Velocity > 0)
                {
                    key.Press(noteOn.Channel);
                }
                else
                {
                    key.Release();
                }
            }
        }
        if (midiEvent is NoteOffEvent noteOff)
        {
            if (_noteToKey.TryGetValue(noteOff.NoteNumber, out var key))
            {
                key.Release();
            }
        }
    }
}
