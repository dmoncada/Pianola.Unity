using System.IO;
using UnityEngine;
using UnityEngine.Networking;

public class PlayStep : MonoBehaviour
{
    [SerializeField]
    private MidiPlayer _midiPlayer = null;

    [SerializeField]
    private string _midiFileName = "entertainer.mid";

    [SerializeField]
    private bool _playOnEnable = true;

    [SerializeField]
    private async Awaitable OnEnable()
    {
        string midiFilePath = Path.Combine(Application.streamingAssetsPath, _midiFileName);

        Stream midiStream = null;
        try
        {
            if (Application.platform == RuntimePlatform.WebGLPlayer)
            {
                midiStream = await LoadMidiAsStreamAsync(midiFilePath);
            }
            else
            {
                midiStream = LoadMidiAsStream(midiFilePath);
            }

            var success = _midiPlayer.Initialize(midiStream);
            if (success && _playOnEnable)
            {
                _midiPlayer.Play();
            }
        }
        finally
        {
            midiStream?.Dispose();
        }

        gameObject.SetActive(false); // Done, disable self.
    }

    private Stream LoadMidiAsStream(string filePath)
    {
        if (File.Exists(filePath) == false)
        {
            Debug.LogError($"File: {filePath} does not exist.", this);
            return null;
        }

        return File.OpenRead(filePath);
    }

    private async Awaitable<Stream> LoadMidiAsStreamAsync(string filePath)
    {
        using var request = UnityWebRequest.Get(filePath);
        await request.SendWebRequest();

        if (request.result != UnityWebRequest.Result.Success)
        {
            Debug.LogError(request.error, this);
            return null;
        }

        var midiData = request.downloadHandler.data;
        return new MemoryStream(midiData);
    }
}
