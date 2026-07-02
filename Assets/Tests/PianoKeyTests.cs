using System.Collections;
using NUnit.Framework;
using UnityEditor.SceneManagement;
using UnityEngine;
using UnityEngine.TestTools;

namespace Pianola.Tests
{
    public class TestPianoKey
    {
        [SetUp]
        public void Setup()
        {
            EditorSceneManager.LoadSceneInPlayMode("Assets/Scenes/PianoTests.unity", new());
        }

        [UnityTest]
        public IEnumerator TestPianoKeyPressRelease()
        {
            yield return null;
            yield return null;
            yield return null;

            var piano = GameObject.Find("Piano");
            Assert.That(piano, Is.Not.Null);

            var keys = piano.GetComponentsInChildren<PianoKey>();
            Assert.That(keys.Length, Is.GreaterThan(0));

            var key = keys[0];

            // Playback state is meaningless in batch mode: there is no audio
            // device, so AudioSource.isPlaying never becomes true.
            var audioEnabled = Application.isBatchMode == false;

            key.Press();
            yield return new WaitForSeconds(1f);
            Assert.That(key.ActiveSource, Is.Not.Null);

            if (audioEnabled)
            {
                Assert.That(key.ActiveSource.isPlaying, Is.True);
            }

            var source = key.ActiveSource;

            key.Release();
            // Wait for the sound to fade out.
            yield return new WaitForSeconds(1f);
            Assert.That(key.ActiveSource, Is.Null);

            if (audioEnabled)
            {
                Assert.That(source.isPlaying, Is.False);
            }
        }
    }
}
