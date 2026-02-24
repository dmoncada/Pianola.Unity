using System;
using UnityEngine;
using UnityEngine.Networking;

namespace Pianola
{
    public class LoadMidiStep : MonoBehaviour
    {
        private const string Url = "http://localhost:8000/generate";

        [SerializeField]
        private MidiPlayer _midiPlayer = null;

        [SerializeField]
        private TextAsset _midiFileAsset = null;

        private bool _finishedLoading = false;

        private void Update()
        {
            if (_finishedLoading)
            {
                _finishedLoading = false;

                gameObject.SetActive(false);
            }
        }

        public void LoadMidi()
        {
            using (new UnityTimedRegion(this))
            {
                InitializeMidiPlayer(GetMidiBytesFromAsset(_midiFileAsset));
            }
        }

        public async void LoadMidiAsync()
        {
            using (new UnityTimedRegion(this))
            {
                InitializeMidiPlayer(await GetMidiBytesAsync(Url));
            }
        }

        private void InitializeMidiPlayer(byte[] midiBytes)
        {
            try
            {
                _midiPlayer.Initialize(midiBytes);

                // TODO(dmoncada): add error handling.
            }
            catch (Exception exception)
            {
                Debug.LogException(exception, this);
            }

            _finishedLoading = true;
        }

        private byte[] GetMidiBytesFromAsset(TextAsset asset)
        {
            Debug.LogFormat(
                "Loading text asset: {0}, size: {1}",
                asset.name,
                Utils.FormatBytes(asset.dataSize),
                this
            );

            return _midiFileAsset.bytes;
        }

        private async Awaitable<byte[]> GetMidiBytesAsync(string url)
        {
            Debug.LogFormat("Loading MIDI from resource: {0}", url, this);

            using UnityWebRequest request = UnityWebRequest.Get(url);

            await request.SendWebRequest();

            if (request.result == UnityWebRequest.Result.Success)
            {
                var data = request.downloadHandler.data;

                Debug.LogFormat("Received: {0}", Utils.FormatBytes(data.Length), this);

                return data;
            }

            Debug.LogError(request.error, this);

            return null;
        }
    }
}
