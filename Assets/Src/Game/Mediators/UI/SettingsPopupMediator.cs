using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class SettingsPopupMediator : EventMediator
{
    [Inject] public SettingsPopup View { get; set; }
    [Inject] public PlayerGlobalModelHolder PlayerGlobalModelHolder { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        View.CloseBtnClicked += OnCloseClicked;
        View.MainMenuBtnClicked += OnMainMenuBtnClicked;
        View.SliderValueChanged += OnSliderValueChanged;
    }

    public async void Start()
    {
        var playerModel = PlayerGlobalModelHolder.PlayerGlobalModel;
        View.SetVolumeIndicators(playerModel.AudioVolume, playerModel.MusicVolume, playerModel.SoundsVolume);

        await View.ShowTask;

        dispatcher.Dispatch(MediatorEvents.UI_SETTINGS_POPUP_SHOW_ANIMATION_ENDED);
    }

    public override void OnRemove()
    {
        View.CloseBtnClicked -= OnCloseClicked;
        View.MainMenuBtnClicked -= OnMainMenuBtnClicked;
        View.SliderValueChanged -= OnSliderValueChanged;

        base.OnRemove();
    }

    private void OnSliderValueChanged()
    {
        var param = new MediatorEventsParams.UiSettingsPopupAudioValueChangedParams(View.AudioVolume, View.MusicVolume, View.SoundsVolume);
        dispatcher.Dispatch(MediatorEvents.UI_SETTINGS_POPUP_AUDIO_VALUE_CHANGED, param);
    }

    private void OnMainMenuBtnClicked()
    {
        AudioManager.Instance.Play(SoundId.ClickDefault);
        dispatcher.Dispatch(MediatorEvents.UI_SETTINGS_POPUP_MAIN_MENU_CLICKED);
    }

    private void OnCloseClicked()
    {
        AudioManager.Instance.Play(SoundId.ClickDefault);
        dispatcher.Dispatch(MediatorEvents.UI_SETTINGS_POPUP_CLOSE_CLICKED);
    }
}
