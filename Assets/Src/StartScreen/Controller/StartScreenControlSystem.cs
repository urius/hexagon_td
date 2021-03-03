using System;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class StartScreenControlSystem : EventSystemBase
{
    private bool _needToSaveSettings = false;

    public override void Start()
    {
        dispatcher.AddListener(MediatorEvents.UI_SETTINGS_POPUP_AUDIO_VALUE_CHANGED, OnSettingsPopupAudioValueChanged);
        dispatcher.AddListener(MediatorEvents.UI_SETTINGS_POPUP_CLOSE_CLICKED, OnSettingsPopupCloseClicked);
        dispatcher.AddListener(MediatorEvents.UI_BOOSTERS_SCREEN_BOOSTER_CLICKED, OnBoosterClicked);
        dispatcher.AddListener(MediatorEvents.UI_HOME_CLICKED, OnHomeClicked);
    }

    private void OnHomeClicked(IEvent payload)
    {
        PlayerSessionModel.Instance.ResetSelectedBoosters();
    }

    private void OnBoosterClicked(IEvent payload)
    {
        var boosterId = (BoosterId)payload.data;

        var playerSession = PlayerSessionModel.Instance;
        var boosterConfig = BoostersConfigProvider.GetBoosterConfigById(boosterId);
        if (!playerSession.SelectedLevelData.IsBoosterSelected(boosterId))
        {
            if (playerSession.PlayerGlobalModel.TrySpendGold(boosterConfig.Price))
            {
                playerSession.SaveGoldFlag = true;
                playerSession.SelectedLevelData.AddBooster(boosterId);
            } else
            {
                dispatcher.Dispatch(MediatorEvents.UI_GOLD_CLICKED);
            }
        }
        else if (playerSession.SelectedLevelData.RemoveBooster(boosterId))
        {
            playerSession.PlayerGlobalModel.AddGold(boosterConfig.Price);
        }
    }

    private void OnSettingsPopupAudioValueChanged(IEvent payload)
    {
        _needToSaveSettings = true;

        var data = payload.data as MediatorEventsParams.UiSettingsPopupAudioValueChangedParams;
        new SetAudioCommand().Execute(data.AudioVolume, data.MusicVolume, data.SoundsVolume);
    }

    private async void OnSettingsPopupCloseClicked(IEvent payload)
    {
        if (_needToSaveSettings)
        {
            _needToSaveSettings = false;

            await new SaveSettingsCommand().ExecuteAsync();
        }
    }
}
