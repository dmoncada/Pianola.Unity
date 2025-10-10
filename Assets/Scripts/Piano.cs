using System.Collections.Generic;
using Melanchall.DryWetMidi.Core;
using UnityEngine;

namespace Pianola
{
    [RequireComponent(typeof(AudioSourcePool))]
    [RequireComponent(typeof(AudioClipProvider))]
    public class Piano : MonoBehaviour
    {
        private const int NumPianoKeys = 88;
        private const int FirstMidiNote = 21;

        private readonly Dictionary<int, PianoKey> _noteToKey = new();

        private AudioSourcePool _sourcePool = null;
        private AudioClipProvider _clipProvider = null;

        private void Awake()
        {
            _sourcePool = GetComponent<AudioSourcePool>();

            _clipProvider = GetComponent<AudioClipProvider>();
        }

        private void Start()
        {
            var keys = GetComponentsInChildren<PianoKey>();

            Debug.Assert(keys.Length == NumPianoKeys, $"Expected {NumPianoKeys} keys.", this);

            int midiNote = FirstMidiNote;

            foreach (var key in keys)
            {
                var clip = _clipProvider[midiNote];
                key.Initialize(clip, _sourcePool);
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
}
