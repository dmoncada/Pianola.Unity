using System.Collections.Generic;
using System.IO;
using System.Net.Http;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Multimedia;
using UnityEngine;
using UnityEngine.Networking;

public class MidiPlayer : MonoBehaviour
{
    private readonly Dictionary<int, PianoKey> _noteToKey = new();

    [SerializeField]
    private GameObject _pianoKeysRoot = null;

    [SerializeField]
    private string _midiFileName = "entertainer.mid";

    [SerializeField]
    private float _startDelay = 1.0f;

    private Playback _playback = null;

    private void Awake()
    {
        int noteNumber = 21;

        foreach (var key in _pianoKeysRoot.GetComponentsInChildren<PianoKey>())
        {
            key.MidiNoteNumber = noteNumber++;

            _noteToKey[key.MidiNoteNumber] = key;
        }
    }

    private async Awaitable Start()
    {
        await Awaitable.WaitForSecondsAsync(_startDelay);

        var midiFilePath = Path.Combine(Application.streamingAssetsPath, _midiFileName);

        MidiFile midiFile = null;
        try
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                midiFile = await LoadMidiAsync(midiFilePath);
            }
            else
            {
                midiFile = LoadMidi(midiFilePath);
            }
        }
        catch (HttpRequestException exception)
        {
            Debug.LogException(exception);
        }

        if (midiFile != null)
        {
            PlayMidi(midiFile);
        }
    }

    private MidiFile LoadMidi(string filePath)
    {
        return MidiFile.Read(filePath);
    }

    private async Awaitable<MidiFile> LoadMidiAsync(string filePath)
    {
        using var request = UnityWebRequest.Get(filePath);
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            throw new HttpRequestException(request.error);
        }

        var midiData = request.downloadHandler.data;
        using var stream = new MemoryStream(midiData);
        return MidiFile.Read(stream);
    }

    private void PlayMidi(MidiFile midiFile)
    {
        var settings = new PlaybackSettings
        {
            ClockSettings = new MidiClockSettings { CreateTickGeneratorCallback = () => null },
        };

        _playback = midiFile.GetPlayback(settings);
        _playback.EventPlayed += OnEventPlayed;
        _playback.Start();
    }

    private void OnDestroy()
    {
        if (_playback == null)
        {
            return;
        }

        if (_playback.IsRunning)
        {
            _playback.Stop();
        }

        _playback.EventPlayed -= OnEventPlayed;
        _playback.Dispose();
        _playback = null;
    }

    private void FixedUpdate()
    {
        if (_playback != null && _playback.IsRunning)
        {
            _playback.TickClock();
        }
    }

    private void OnEventPlayed(object sender, MidiEventPlayedEventArgs args)
    {
        var midiEvent = args.Event;
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
