using System;
using System.IO;
using Melanchall.DryWetMidi.Core;
using Melanchall.DryWetMidi.Interaction;
using Melanchall.DryWetMidi.Multimedia;
using UnityEngine;
using UnityEngine.Events;

namespace Pianola
{
    public class MidiPlayer : MonoBehaviour
    {
        [SerializeField]
        private UnityEvent<float> _onPlaybackSet = new();

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
        private void OnEnable()
        {
            if (_wasPlaying)
            {
                _wasPlaying = false;

                _playback?.Start();
            }
        }

        private void OnDisable()
        {
            _wasPlaying = IsPlaying;

            _playback?.Stop();
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
                CleanupPlayback(_playback);

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

        public bool Initialize(Stream midiStream)
        {
            OnDestroy(); // Clean up playback (if any).

            var midiFile = ReadMidiFromStream(midiStream);
            if (midiFile == null)
            {
                Debug.LogError("Failed to initialize MIDI player.", this);
                return false;
            }

            Debug.Log($"Finished loading MIDI, duration: {midiFile.GetDurationAsFloat():F1}", this);
            _playback = SetupPlayback(midiFile);
            _onPlaybackSet?.Invoke(Duration);
            return true;
        }

        public void Play()
        {
            _playback?.Start();
        }

        public void Pause()
        {
            _playback?.Stop();
        }

        public void Toggle()
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

        public void MoveBack(int deltaSeconds)
        {
            _playback?.MoveBack(deltaSeconds);
        }

        public void MoveForward(int deltaSeconds)
        {
            _playback?.MoveForward(deltaSeconds);
        }

        public void Seek(float positionSeconds)
        {
            _playback?.MoveToTime(positionSeconds);
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
            return playback;
        }

        private void CleanupPlayback(Playback playback)
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

            Debug.Log($"Playback {status}, time: {CurrentTime:F1} secs.", this);

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

        public static void MoveToTime(this Playback playback, float timeSeconds)
        {
            playback.MoveToTime(new MetricTimeSpan((long)(timeSeconds * 1_000_000)));
        }
    }
}
