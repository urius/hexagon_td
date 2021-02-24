using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(LocalizationProvider))]
public class LocalizationProviderEditor : Editor
{
    private SystemLanguage _debugLanguage = SystemLanguage.English;

    public override void OnInspectorGUI()
    {
        //base.OnInspectorGUI();
        DrawDefaultInspector();

        GUILayout.Label("\n\n---\nEditor tools:");

        EditorGUILayout.BeginHorizontal();
        {
            EditorGUILayout.PrefixLabel("Debug language:");
            var _sysLangBefore = _debugLanguage;
            _debugLanguage = (SystemLanguage)EditorGUILayout.EnumPopup(_debugLanguage);
            if (_sysLangBefore != _debugLanguage)
            {
                ((LocalizationProvider)serializedObject.targetObject).SetDebugSystemLanguage(_debugLanguage);
            }
        }
        EditorGUILayout.EndHorizontal();
    }
}
