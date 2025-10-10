using UnityEngine;

namespace Pianola
{
    [CreateAssetMenu(fileName = nameof(PlaybackStateAsset), menuName = "SO/PlaybackStateAsset")]
    public class PlaybackStateAsset : ScriptableObject
    {
        public float Opacity = 1f;
    }
}
