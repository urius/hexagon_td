using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class SelectLevelScreenViewMediator : EventMediator
{
    [Inject] public SelectLevelScreenView SelectLevelScreenView { get; set; }
    [Inject] public PlayerGlobalModelHolder PlayerGlobalModelHolder { get; set; }
    [Inject] public LocalizationProvider LocalizationProvider { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        dispatcher.AddListener(MediatorEvents.UI_SL_SELECT_LEVEL_CLICKED, OnLevelClicked);
        SelectLevelScreenView.StartClicked += OnStartClicked;
    }

    private void OnDestroy()
    {
        dispatcher.RemoveListener(MediatorEvents.UI_SL_SELECT_LEVEL_CLICKED, OnLevelClicked);
        SelectLevelScreenView.StartClicked -= OnStartClicked;
    }

    private void OnStartClicked()
    {
        dispatcher.Dispatch(MediatorEvents.UI_SL_START_LEVEL_CLICKED);
    }

    private void OnLevelClicked(IEvent payload)
    {
        var levelIndex = (int)payload.data;

        var levelProgress = PlayerGlobalModelHolder.PlayerGlobalModel.LevelsProgress[levelIndex];

        if (levelProgress.isUnlocked)
        {
            SelectLevelScreenView.ShowStartButton();
        }
        else
        {
            var str = string.Format(
                LocalizationProvider.Get(LocalizationGroupId.StartScreen, "levels_menu_level_locked"),
                levelIndex + 1);

            SelectLevelScreenView.ShowLockedText(str);
        }
    }
}
