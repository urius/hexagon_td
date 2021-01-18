using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEditor.SceneManagement;
using UnityEngine;

public class ScenesMenuFunctions : MonoBehaviour
{
    [MenuItem("Scenes/Bootstrapper")]
    private static void ShowBootstrapScene()
    {
        OpenScene("BootstrapScene");
    }

    [MenuItem("Scenes/MainMenuScene")]
    private static void ShowMainMenuScene()
    {
        OpenScene("MainMenuScene");
    }

    [MenuItem("Scenes/GameScene")]
    private static void ShowGameScene()
    {
        OpenScene("GameScene");
    }

    [MenuItem("Scenes/TransitionScene")]
    private static void ShowTransitionScene()
    {
        OpenScene("TransitionScene");
    }
    
    private static void OpenScene(string sceneName)
    {
        EditorSceneManager.OpenScene($"Assets/Scenes/{sceneName}.unity");
    }
}
