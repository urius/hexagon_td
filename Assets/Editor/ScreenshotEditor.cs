using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(Screenshot))]
public class ScreenshotEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Take Screenshot"))
        {
            var filepath = ((Screenshot)serializedObject.targetObject).TakeScreenshot();

            if (File.Exists(filepath))
            {
                var fileBytes = File.ReadAllBytes(filepath);
            }
        }
    }
}
