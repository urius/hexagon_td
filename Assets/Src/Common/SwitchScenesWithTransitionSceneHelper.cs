using System;
using Cysharp.Threading.Tasks;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;
using UnityEngine.Rendering.Universal;
using UniTask = Cysharp.Threading.Tasks.UniTask;

public class SwitchScenesWithTransitionSceneHelper
{
    private const string TransitionSceneName = SceneNames.Transition;

    private readonly IEventDispatcher globalDispatcher;

    private Camera _transitionBaseCamera;
    private Camera _transitionOverlayCamera;
    private string _switchFromSceneName;
    private string _switchToSceneName;
    private UniTaskCompletionSource<bool> _transitionFinishedTsc = new UniTaskCompletionSource<bool>();
    private bool _isSwitching = false;
    private Func<UniTask> _additionalTaskAction;

    public SwitchScenesWithTransitionSceneHelper(IEventDispatcher globalDispatcher)
    {
        this.globalDispatcher = globalDispatcher;
    }

    public async UniTask SwitchAsync(string switchFromSceneName, string switchToSceneName, Func<UniTask> additionalTaskAction = null)
    {
        if (_isSwitching)
        {
            await _transitionFinishedTsc.Task;
        }
        _additionalTaskAction = additionalTaskAction;
        _transitionFinishedTsc = new UniTaskCompletionSource<bool>();

        _switchFromSceneName = switchFromSceneName;
        _switchToSceneName = switchToSceneName;

        globalDispatcher.AddListener(MediatorEvents.UI_TS_SHOW_ANIM_ENDED, OnShowTransitionEnded);

        await LoadSceneHelper.LoadSceneAdditiveAsync(TransitionSceneName);
        _transitionBaseCamera = GameObject.FindWithTag("TransitionBaseCamera").GetComponent<Camera>();
        _transitionOverlayCamera = GameObject.FindWithTag("TransitionCamera").GetComponent<Camera>();
        AddOverlayCameraToAllCameras();

        await _transitionFinishedTsc.Task;
    }

    private async void OnShowTransitionEnded(IEvent payload)
    {
        globalDispatcher.RemoveListener(MediatorEvents.UI_TS_SHOW_ANIM_ENDED, OnShowTransitionEnded);

        await LoadSceneHelper.UnloadSceneAsync(_switchFromSceneName);

        var additionalTask = _additionalTaskAction != null ? _additionalTaskAction() : UniTask.CompletedTask;
        var loadGameTask = LoadSceneHelper.LoadSceneAdditiveAsync(_switchToSceneName, out var loadGameOperation);
        while (!loadGameTask.Status.IsCompleted())
        {
            globalDispatcher.Dispatch(MediatorEvents.UI_TS_LOAD_GAME_PROGRESS_UPDATED, loadGameOperation.progress);
            await UniTask.Delay(100);
        }
        //AddOverlayCameraToAllCameras();

        await additionalTask;

        globalDispatcher.AddListener(MediatorEvents.UI_TS_HIDE_ANIM_ENDED, OnHideTransitionEnded);
        globalDispatcher.Dispatch(MediatorEvents.UI_TS_REQUEST_HIDE_ANIM);
    }

    private void AddOverlayCameraToAllCameras()
    {
        var cameras = GameObject.FindObjectsOfType<Camera>();
        foreach (var camera in cameras)
        {
            var cameraData = camera.GetUniversalAdditionalCameraData();
            if (cameraData.renderType == CameraRenderType.Base
                && !cameraData.cameraStack.Contains(_transitionOverlayCamera))
            {
                cameraData.cameraStack.Add(_transitionOverlayCamera);
            }
        }
    }

    private async void OnHideTransitionEnded(IEvent payload)
    {
        globalDispatcher.RemoveListener(MediatorEvents.UI_TS_HIDE_ANIM_ENDED, OnHideTransitionEnded);

        _transitionBaseCamera.GetUniversalAdditionalCameraData().cameraStack.Remove(_transitionOverlayCamera);
        await LoadSceneHelper.UnloadSceneAsync(TransitionSceneName);

        _isSwitching = false;
        _transitionFinishedTsc.TrySetResult(true);
    }
}
