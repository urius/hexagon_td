using UnityEngine;
using UnityEditor;

public class ToolsMenuFunctions
{
    [MenuItem("Tools/Configs")]
    private static void ShowResourcesFolder()
    {
        FocusFolder("Src/Configs/Levels");
    }

    private static void FocusFolder(string path)
    {
        EditorUtility.FocusProjectWindow();
        Object obj = AssetDatabase.LoadAssetAtPath<Object>(path);
        EditorGUIUtility.PingObject(obj);
    }
}
