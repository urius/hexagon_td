using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using strange.extensions.command.impl;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class StartLevelClickedCommand : Command
{
    public override async void Execute()
    {
        Debug.Log("Start level clicked");
        var transitionSceneName = "TransitionScene";

        Retain();

        await LoadSceneHelper.LoadSceneAdditiveAsync(transitionSceneName);
        var transitionCamera = GameObject.FindWithTag("TransitionCamera").GetComponent<Camera>();
        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(transitionCamera);

        var gameScene = await LoadSceneHelper.LoadSceneAdditiveAsync("GameScene");
        var cameras = GameObject.FindObjectsOfType<Camera>();
        foreach (var camera in cameras)
        {
            var cameraData = camera.GetUniversalAdditionalCameraData();
            if (cameraData.renderType == CameraRenderType.Base && cameraData.cameraStack.Count == 0)
            {
                cameraData.cameraStack.Add(transitionCamera);
            }
        }

        await LoadSceneHelper.UnloadSceneAsync("MainMenuScene");
        await LoadSceneHelper.UnloadSceneAsync(transitionSceneName);
        //SceneManager.SetActiveScene(gameScene);

        Release();
    }
}
