using System;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class MenuSceneCanvasViewMediator : EventMediator
{
    [Inject] public MenuSceneCanvasView MenuSceneCanvasView { get; set; }
    [Inject] public UIPrefabsConfig UIPrefabsConfig { get; set; }
    [Inject] public PlayerSessionModel PlayerGlobalModelHolder { get; set; }

    private GameObject _currentActiveScreenPrefab;
    private GameObject _currentActiveScreenGo;

    private RectTransform RootTransform => MenuSceneCanvasView.RootTransform;

    public override void OnRegister()
    {
        base.OnRegister();

        dispatcher.AddListener(MediatorEvents.UI_HOME_CLICKED, OnHomeClicked);
        dispatcher.AddListener(MediatorEvents.UI_GOLD_CLICKED, OnGoldClicked);
        dispatcher.AddListener(MediatorEvents.UI_SS_PLAY_CLICKED, OnMainMenuPlayClicked);
        dispatcher.AddListener(MediatorEvents.UI_SS_HOW_TO_PLAY_CLICKED, OnMainMenuHowToPlayClicked);
        dispatcher.AddListener(MediatorEvents.UI_SETTINGS_CLICKED, OnMainMenuSettingsClicked);
        dispatcher.AddListener(MediatorEvents.UI_SS_SPECIAL_THANKS_CLICKED, OnMainMenuSpecialThanksClicked);
    }

    private void OnDestroy()
    {
        dispatcher.RemoveListener(MediatorEvents.UI_HOME_CLICKED, OnHomeClicked);
        dispatcher.RemoveListener(MediatorEvents.UI_GOLD_CLICKED, OnGoldClicked);
        dispatcher.RemoveListener(MediatorEvents.UI_SS_PLAY_CLICKED, OnMainMenuPlayClicked);
        dispatcher.RemoveListener(MediatorEvents.UI_SS_HOW_TO_PLAY_CLICKED, OnMainMenuHowToPlayClicked);
        dispatcher.RemoveListener(MediatorEvents.UI_SETTINGS_CLICKED, OnMainMenuSettingsClicked);
        dispatcher.RemoveListener(MediatorEvents.UI_SS_SPECIAL_THANKS_CLICKED, OnMainMenuSpecialThanksClicked);
    }

    private void OnGoldClicked(IEvent payload)
    {
        var goldStoreWindowGO = Instantiate(CommonUIPrefabsConfig.Instance.GoldStoreWindowPrefab, RootTransform);
    }

    private void Start()
    {
        if (PlayerSessionModel.Instance.SelectedLevelIndex < 0)
        {
            ShowScreen(UIPrefabsConfig.MainMenuScreenPrefab);
        }
        else
        {
            ShowScreen(UIPrefabsConfig.SelectLevelScreenPrefab);
        }
    }

    private void OnMainMenuPlayClicked(IEvent payload)
    {
        ShowScreen(UIPrefabsConfig.SelectLevelScreenPrefab);
    }

    private void OnMainMenuHowToPlayClicked(IEvent payload)
    {
        ShowScreen(UIPrefabsConfig.HowToPlayScreenPrefab);
    }

    private void OnMainMenuSettingsClicked(IEvent payload)
    {
        Instantiate(UIPrefabsConfig.SettingsForStartScreenPrefab, RootTransform);
    }

    private void OnMainMenuSpecialThanksClicked(IEvent payload)
    {
        ShowScreen(UIPrefabsConfig.SpecialThanksPrefab);
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
            _currentActiveScreenGo = Instantiate(screenPrefab, RootTransform);
        }
    }
}
