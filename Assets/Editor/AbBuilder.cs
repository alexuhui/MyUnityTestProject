using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class AbBuilder
{


    [MenuItem("Tools/BuildAB")]
    private static void BuildAB()
    {
        AssetBundleBuild[] abs = new AssetBundleBuild[] {
            new AssetBundleBuild{ assetNames = new string[]{"Assets/Atlas/Atlas1.spriteatlas" }, assetBundleName = "Atlas1" },
            new AssetBundleBuild{ assetNames = new string[]{"Assets/Atlas/Atlas2.spriteatlas" }, assetBundleName = "Atlas2" },
        };

        BuildAssetBundleOptions abOptions = BuildAssetBundleOptions.DeterministicAssetBundle 
            | BuildAssetBundleOptions.ChunkBasedCompression 
            | BuildAssetBundleOptions.ForceRebuildAssetBundle;

        string path = "Assets/StreamingAssets";

        BuildPipeline.BuildAssetBundles(path, abs, abOptions, EditorUserBuildSettings.activeBuildTarget);

        AssetDatabase.Refresh();
    }
}
