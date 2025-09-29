using System;
using System.IO;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.Networking;

public class MidiPlayer : MonoBehaviour
{
    [SerializeField]
    private string _midiFileName = "entertainer.mid";

    [SerializeField]
    private bool _playOnStart = true;

    [SerializeField]
    private UnityEvent<float> _onPlayBackSet = new();

    [SerializeField]
    private UnityEvent<bool> _onPlayPause = new();

    [SerializeField]
    private UnityEvent<MidiEvent> _onMidiEvent = new();

    private Playback _playback = null;
    private bool _wasPlaying = false;

    public float CurrentTime => _playback?.GetCurrentTimeAsFloat() ?? -1f;
    public float Duration => _playback?.GetDurationAsFloat() ?? -1f;
    public bool IsPlaying => _playback?.IsRunning ?? false;

    #region Unity Lifecycle
    private async Awaitable Start()
    {
        await Awaitable.WaitForSecondsAsync(1f);
        var midiFilePath = Path.Combine(Application.streamingAssetsPath, _midiFileName);
        var midiFile = await LoadMidiAsync(midiFilePath);
        if (midiFile != null)
        {
            _playback = SetupPlayback(midiFile);
        }

        if (_playback != null && _playOnStart)
        {
            _playback.Start();
        }
    }

    private void OnEnable()
    {
        if (_wasPlaying)
        {
            _wasPlaying = false;

            _playback.Start();
        }
    }

    private void OnDisable()
    {
        _wasPlaying = IsPlaying;

        _playback.Stop();
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            OnEnable();
        }
        else
        {
            OnDisable();
        }
    }

    private void OnDestroy()
    {
        if (_playback != null)
        {
            TeardownPlayback(_playback);

            _playback = null;
        }
    }

    private void FixedUpdate()
    {
        if (IsPlaying)
        {
            _playback.TickClock();
        }
    }
    #endregion Unity Lifecycle

    public void Toggle()
    {
        if (isActiveAndEnabled)
        {
            if (IsPlaying)
            {
                _playback.Stop();
            }
            else
            {
                _playback.Start();
            }
        }
    }

    public void MoveBack(int deltaSeconds)
    {
        _playback.MoveBack(deltaSeconds);
    }

    public void MoveForward(int deltaSeconds)
    {
        _playback.MoveForward(deltaSeconds);
    }

    public void MoveToTime(float playbackTimeSeconds)
    {
        _playback.MoveToTime(new MetricTimeSpan((long)(playbackTimeSeconds * 1_000_000)));
    }

    private async Awaitable<MidiFile> LoadMidiAsync(string filePath)
    {
        MidiFile midiFile;

        if (Application.platform == RuntimePlatform.WebGLPlayer)
        {
            midiFile = await ReadMidiAsync(filePath);
        }
        else
        {
            midiFile = ReadMidi(filePath);
        }

        if (midiFile != null)
        {
            Debug.Log($"Finished loading MIDI, duration: {midiFile.GetDurationAsFloat():F1} secs.");
        }

        return midiFile;
    }

    private MidiFile ReadMidi(string filePath)
    {
        if (File.Exists(filePath) == false)
        {
            Debug.LogError($"File: {filePath} does not exist.", this);
            return null;
        }

        using var stream = File.OpenRead(filePath);
        return ReadMidiFromStream(stream);
    }

    private async Awaitable<MidiFile> ReadMidiAsync(string filePath)
    {
        using var request = UnityWebRequest.Get(filePath);
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error, this);
            return null;
        }

        var midiData = request.downloadHandler.data;
        using var stream = new MemoryStream(midiData);
        return ReadMidiFromStream(stream);
    }

    private MidiFile ReadMidiFromStream(Stream stream)
    {
        try
        {
            return MidiFile.Read(stream);
        }
        catch (Exception exception) // TODO(dmoncada): handle specific exceptions.
        {
            Debug.LogException(exception, this);
            return null;
        }
    }

    private Playback SetupPlayback(MidiFile midiFile)
    {
        var settings = new PlaybackSettings
        {
            ClockSettings = new MidiClockSettings { CreateTickGeneratorCallback = () => null },
        };

        var playback = midiFile.GetPlayback(settings);
        playback.Started += OnStartStop;
        playback.Stopped += OnStartStop;
        playback.Finished += OnFinished;
        playback.EventPlayed += OnEventPlayed;
        _onPlayBackSet?.Invoke(Duration);
        return playback;
    }

    private void TeardownPlayback(Playback playback)
    {
        playback.Stop();
        playback.Started -= OnStartStop;
        playback.Stopped -= OnStartStop;
        playback.Finished -= OnFinished;
        playback.EventPlayed -= OnEventPlayed;
        playback.Dispose();
    }

    #region Event Handlers
    private void OnStartStop(object sender, EventArgs args)
    {
        var status = IsPlaying ? "started" : "stopped";

        Debug.Log($"Playback {status}, time: {CurrentTime:F1} secs.");

        _onPlayPause?.Invoke(IsPlaying);
    }

    private void OnFinished(object sender, EventArgs args)
    {
        _onPlayPause?.Invoke(false);
    }

    private void OnEventPlayed(object _, MidiEventPlayedEventArgs args)
    {
        _onMidiEvent?.Invoke(args.Event);
    }
    #endregion Event Handlers
}

public static class Extensions
{
    public static float GetDurationAsFloat(this MidiFile midiFile)
    {
        return (float)midiFile.GetDuration<MetricTimeSpan>().TotalSeconds;
    }

    public static float GetDurationAsFloat(this Playback playback)
    {
        return (float)playback.GetDuration<MetricTimeSpan>().TotalSeconds;
    }

    public static float GetCurrentTimeAsFloat(this Playback playback)
    {
        return (float)playback.GetCurrentTime<MetricTimeSpan>().TotalSeconds;
    }

    public static void MoveBack(this Playback playback, int deltaSeconds)
    {
        playback.MoveBack(new MetricTimeSpan(0, 0, deltaSeconds));
    }

    public static void MoveForward(this Playback playback, int deltaSeconds)
    {
        playback.MoveForward(new MetricTimeSpan(0, 0, deltaSeconds));
    }
}
