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
    private GameObject _currentActiveScreenGo;

    public override void OnRegister()
    {
        base.OnRegister();

        dispatcher.AddListener(MediatorEvents.UI_HOME_CLICKED, OnHomeClicked);
        dispatcher.AddListener(MediatorEvents.UI_SS_PLAY_CLICKED, OnMainMenuPlayClicked);
        dispatcher.AddListener(MediatorEvents.UI_SS_HOW_TO_PLAY_CLICKED, OnMainMenuHowToPlayClicked);
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
        dispatcher.RemoveListener(MediatorEvents.UI_HOME_CLICKED, OnHomeClicked);
        dispatcher.RemoveListener(MediatorEvents.UI_SS_PLAY_CLICKED, OnMainMenuPlayClicked);
        dispatcher.RemoveListener(MediatorEvents.UI_SS_HOW_TO_PLAY_CLICKED, OnMainMenuHowToPlayClicked);
    }

    private void OnMainMenuPlayClicked(IEvent payload)
    {
        ShowScreen(UIPrefabsConfig.SelectLevelScreenPrefab);
    }

    private void OnMainMenuHowToPlayClicked(IEvent payload)
    {
        ShowScreen(UIPrefabsConfig.HowToPlayScreenPrefab);
    }

    private void OnHomeClicked(IEvent payload)
    {
        ShowScreen(UIPrefabsConfig.MainMenuScreenPrefab);

        AudioManager.Instance.Play(SoundId.ClickDefault);
    }

    private void ShowScreen(GameObject screenPrefab)
    {
        if (_currentActiveScreenPrefab != screenPrefab)
        {
            if (_currentActiveScreenGo != null)
            {
                _currentActiveScreenGo.GetComponentInChildren<ScreenView>().HideAnimated();
            }
            _currentActiveScreenPrefab = screenPrefab;
            _currentActiveScreenGo = Instantiate(screenPrefab, MenuSceneCanvasView.RootTransform);
        }
    }
}
