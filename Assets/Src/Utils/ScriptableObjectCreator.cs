using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class ScriptableObjectCreator
{
    public static void CreateAndSaveAsset<T>(string path, string name) where T : ScriptableObject
    {
        T asset = ScriptableObject.CreateInstance<T>();

        SaveAsset(asset, path, name);
    }

    public static void SaveAsset<T>(T asset, string path, string name, bool enableRewrite = false) where T : ScriptableObject
    {
        var fullName = path + name + ".asset";

        string assetPathAndName;
        if (enableRewrite)
        {
            assetPathAndName = fullName;
            AssetDatabase.DeleteAsset(assetPathAndName);
        } else
        {
            assetPathAndName = AssetDatabase.GenerateUniqueAssetPath(fullName);
        }

        AssetDatabase.CreateAsset(asset, assetPathAndName);

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Selection.activeObject = asset;
    }
}
