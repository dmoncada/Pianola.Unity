using UnityEngine;

namespace Pianola
{
    public class LoadAudioClipsStep : MonoBehaviour
    {
        [SerializeField]
        private AudioClipProvider _provider = null;

        private void Update()
        {
            if (_provider.IsInitialized)
            {
                gameObject.SetActive(false);
            }
        }
    }
}
