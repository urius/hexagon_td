using UnityEngine;
using UnityEditor;
using System.IO;

[CustomEditor(typeof(PlayerGlobalModelHolder))]
public class PlayerGlobalModelHolderEditor : Editor
{
    public override void OnInspectorGUI()
    {
        DrawDefaultInspector();

        if (GUILayout.Button("Load"))
        {
            var modelHolder = ((PlayerGlobalModelHolder)serializedObject.targetObject);
            modelHolder.Load();
        }

        if (GUILayout.Button("Save"))
        {
            var modelHolder = ((PlayerGlobalModelHolder)serializedObject.targetObject);
            modelHolder.Save();
        }
    }
}
