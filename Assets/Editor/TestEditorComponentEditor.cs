using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TestEditorComponent))]
public class TestEditorComponentEditor : Editor
{
    private void OnSceneGUI()
    {
        var currentEvent = Event.current;
        var isCtrlKeyDown = currentEvent.control;


        if (isCtrlKeyDown)
        {
            HandleUtility.AddDefaultControl(GUIUtility.GetControlID(FocusType.Passive));

            Debug.Log("isCtrlKeyDown");
            Handles.BeginGUI();
            GUILayout.Label($"({currentEvent.mousePosition.x}, {currentEvent.mousePosition.y})");
            Handles.EndGUI();
        }
    }
}
