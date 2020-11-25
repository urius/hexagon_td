using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class PlayerDataUpdateSystem : EventSystemBase
{
    [Inject] public PlayerGlobalModelHolder PlayerGlobalModelHolder { get; set; }

    public override void Start()
    {
        dispatcher.AddListener(MediatorEvents.UI_SETTINGS_POPUP_AUDIO_VALUE_CHANGED, OnAudioValueChanged);
    }

    private void OnAudioValueChanged(IEvent payload)
    {
        var data = payload.data as MediatorEventsParams.UiSettingsPopupAudioValueChangedParams;

        SetAudioVolumePercent(data.AudioVolume);
        SetMusicVolumePercent(data.MusicVolume);
        SetSoundsVolumePercent(data.SoundsVolume);
    }

    private void SetAudioVolumePercent(float audioValue)
    {
        PlayerGlobalModelHolder.PlayerGlobalModel.AudioVolume = audioValue;

        AudioManager.Instance.SetMasterVolume(audioValue);
    }

    private void SetMusicVolumePercent(float musicValue)
    {
        PlayerGlobalModelHolder.PlayerGlobalModel.MusicVolume = musicValue;

        AudioManager.Instance.SetMusicVolume(musicValue);
    }

    private void SetSoundsVolumePercent(float soundsVolume)
    {
        PlayerGlobalModelHolder.PlayerGlobalModel.SoundsVolume = soundsVolume;

        AudioManager.Instance.SetSoundsVolume(soundsVolume);
    }
}
