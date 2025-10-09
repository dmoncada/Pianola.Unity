using System;
using UnityEditor;
using UnityEditor.Build;
using UnityEditor.Build.Reporting;

namespace Pianola.Editor
{
    public static class Builder
    {
        private static readonly string[] Scenes = new[] { "Assets/Scenes/Main.unity" };

        private const string OutputPath = "Builds/WebGL";

        public static void BuildWebGL()
        {
            BuildPipeline.BuildPlayer(
                new BuildPlayerOptions()
                {
                    locationPathName = OutputPath,
                    target = BuildTarget.WebGL,
                    scenes = Scenes,
                }
            );
        }
    }

    public class BuildPreprocessor : IPreprocessBuildWithReport
    {
        private const string VersionAssetPath = "Assets/ScriptableObjects/VersionAsset.asset";

        public int callbackOrder => 1;

        public void OnPreprocessBuild(BuildReport report)
        {
            IncrementVersion(VersionAssetPath);
        }

        private void IncrementVersion(string versionAssetPath)
        {
            var versionAsset = AssetDatabase.LoadAssetAtPath<VersionAsset>(versionAssetPath);
            var nextVersion = GetVersion(DateTime.UtcNow);
            versionAsset.Version = nextVersion.ToString();
            EditorUtility.SetDirty(versionAsset);
            AssetDatabase.SaveAssets();
        }

        private Version GetVersion(DateTime dt)
        {
            return new Version(dt.Year, dt.Month, dt.Day, 100 * dt.Hour + dt.Minute);
        }
    }
}
