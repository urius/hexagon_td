using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UnityEngine.SceneManagement;

public class StartLevelClickedCommand : EventCommand
{
    private const string TransitionSceneName = "TransitionScene";

    [Inject(ContextKeys.CROSS_CONTEXT_DISPATCHER)]
    public IEventDispatcher globalDispatcher { get; set; }

    private Camera _transitionCamera;

    public override async void Execute()
    {
        Retain();

        globalDispatcher.AddListener(MediatorEvents.UI_TS_SHOW_ANIM_ENDED, OnShowTransitionEnded);

        await LoadSceneHelper.LoadSceneAdditiveAsync(TransitionSceneName);
        _transitionCamera = GameObject.FindWithTag("TransitionCamera").GetComponent<Camera>();
        Camera.main.GetUniversalAdditionalCameraData().cameraStack.Add(_transitionCamera);
    }

    private async void OnShowTransitionEnded(IEvent payload)
    {
        globalDispatcher.RemoveListener(MediatorEvents.UI_TS_SHOW_ANIM_ENDED, OnShowTransitionEnded);

        var loadGameTask = LoadSceneHelper.LoadSceneAdditiveAsync("GameScene", out var loadGameOperation);
        while (!loadGameTask.IsCompleted)
        {
            globalDispatcher.Dispatch(MediatorEvents.UI_TS_LOAD_GAME_PROGRESS_UPDATED, loadGameOperation.progress);
            await Task.Delay(100);
        }
        var cameras = GameObject.FindObjectsOfType<Camera>();
        foreach (var camera in cameras)
        {
            var cameraData = camera.GetUniversalAdditionalCameraData();
            if (cameraData.renderType == CameraRenderType.Base && cameraData.cameraStack.Count == 0)
            {
                cameraData.cameraStack.Add(_transitionCamera);
            }
        }

        globalDispatcher.AddListener(MediatorEvents.UI_TS_HIDE_ANIM_ENDED, OnHideTransitionEnded);

        await LoadSceneHelper.UnloadSceneAsync("MainMenuScene");
        globalDispatcher.Dispatch(MediatorEvents.UI_TS_REQUEST_HIDE_ANIM);
    }

    private async void OnHideTransitionEnded(IEvent payload)
    {
        globalDispatcher.RemoveListener(MediatorEvents.UI_TS_HIDE_ANIM_ENDED, OnHideTransitionEnded);

        await LoadSceneHelper.UnloadSceneAsync(TransitionSceneName);

        Release();
    }
}
