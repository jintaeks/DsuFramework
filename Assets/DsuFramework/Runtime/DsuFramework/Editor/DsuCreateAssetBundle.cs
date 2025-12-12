#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

namespace Dsu.Framework
{
    public class DsuCreateAssetBundles
    {
        [MenuItem("Assets/DsuFramework/Dsu Build AssetBundle", false, 51)]
        static void BuildAllAssetBundles()
        {
            string assetBundleDirectory = "Assets/StreamingAssets";
            if (!Directory.Exists(Application.streamingAssetsPath)) {
                Directory.CreateDirectory(assetBundleDirectory);
            }
            BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
        }
    }
}
#endif
