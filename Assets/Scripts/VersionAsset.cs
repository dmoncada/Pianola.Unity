using Unity.Properties;
using UnityEngine;

namespace Pianola
{
    [CreateAssetMenu(fileName = nameof(VersionAsset), menuName = "SO/VersionAsset")]
    public class VersionAsset : ScriptableObject
    {
        [SerializeField, DontCreateProperty]
        private string _version = null;

        public string Version
        {
            get => _version;
            set => _version = value;
        }

        [CreateProperty]
        public string FormattedVersion
        {
            get { return $"Build: {_version}"; }
        }
    }
}
