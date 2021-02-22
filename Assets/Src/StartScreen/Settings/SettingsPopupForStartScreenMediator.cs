using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class SettingsPopupForStartScreenMediator : EventMediator
{
    [Inject] public SettingsPopupForStartScreen View { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        View.CloseBtnClicked += OnCloseClicked;
        View.SliderValueChanged += OnSliderValueChanged;
    }

    public async void Start()
    {
        var playerModel = PlayerSessionModel.Model;
        View.SetupVolumeIndicators(playerModel.AudioVolume, playerModel.MusicVolume, playerModel.SoundsVolume);
        View.SetId(playerModel.Id);

        await View.ShowTask;

        dispatcher.Dispatch(MediatorEvents.UI_SETTINGS_POPUP_SHOW_ANIMATION_ENDED);
    }

    public override void OnRemove()
    {
        View.CloseBtnClicked -= OnCloseClicked;
        View.SliderValueChanged -= OnSliderValueChanged;

        base.OnRemove();
    }

    private void OnSliderValueChanged()
    {
        var param = new MediatorEventsParams.UiSettingsPopupAudioValueChangedParams(View.AudioVolume, View.MusicVolume, View.SoundsVolume);
        dispatcher.Dispatch(MediatorEvents.UI_SETTINGS_POPUP_AUDIO_VALUE_CHANGED, param);
    }

    private void OnCloseClicked()
    {
        AudioManager.Instance.Play(SoundId.ClickDefault);
        dispatcher.Dispatch(MediatorEvents.UI_SETTINGS_POPUP_CLOSE_CLICKED);
    }
}
