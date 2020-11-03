using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class MenuSceneCanvasViewMediator : EventMediator
{
    [Inject] public MenuSceneCanvasView MenuSceneCanvasView { get; set; }
    [Inject] public LevelConfigProvider LevelConfigProvider { get; set; }
    [Inject] public UIPrefabsConfig UIPrefabsConfig { get; set; }

    private GameObject _currentActiveScreenPrefab;

    public override void OnRegister()
    {
        base.OnRegister();

        dispatcher.AddListener(MediatorEvents.UI_SL_HOME_CLICKED, OnHomeClicked);
        dispatcher.AddListener(MediatorEvents.UI_SS_PLAY_CLICKED, OnMainMenuPlayClicked);
    }

    private void Start()
    {
        if (LevelConfigProvider.LevelConfig == null)
        {
            ShowScreen(UIPrefabsConfig.MainMenuScreenPrefab);
        }
        else
        {
            ShowScreen(UIPrefabsConfig.SelectLevelScreenPrefab);
        }
    }

    private void OnDestroy()
    {
        dispatcher.RemoveListener(MediatorEvents.UI_SL_HOME_CLICKED, OnHomeClicked);
        dispatcher.RemoveListener(MediatorEvents.UI_SS_PLAY_CLICKED, OnMainMenuPlayClicked);
    }

    private void OnMainMenuPlayClicked(IEvent payload)
    {
        ShowScreen(UIPrefabsConfig.SelectLevelScreenPrefab);
    }

    private void OnHomeClicked(IEvent payload)
    {
        ShowScreen(UIPrefabsConfig.MainMenuScreenPrefab);
    }

    private void ShowScreen(GameObject screenPrefab)
    {
        if (_currentActiveScreenPrefab != screenPrefab)
        {
            _currentActiveScreenPrefab = screenPrefab;
            Instantiate(screenPrefab, MenuSceneCanvasView.RootTransform);
        }
    }
}
