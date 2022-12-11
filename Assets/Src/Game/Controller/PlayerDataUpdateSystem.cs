using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;
public class PlayerDataUpdateSystem : EventSystemBase
{
    [Inject] public LevelModel LevelModel { get; set; }

    private bool _needToSaveSettings = false;

    public override void Start()
    {
        dispatcher.AddListener(MediatorEvents.UI_SETTINGS_POPUP_AUDIO_VALUE_CHANGED, OnAudioValueChanged);
        dispatcher.AddListener(MediatorEvents.UI_SETTINGS_CLICKED, OnSettingsClicked);
        dispatcher.AddListener(MediatorEvents.UI_SETTINGS_POPUP_SHOW_ANIMATION_ENDED, OnSettingsPopupShown);
        dispatcher.AddListener(MediatorEvents.UI_SETTINGS_POPUP_CLOSE_CLICKED, OnSettingsPopupCloseClicked);

        LevelModel.GameSpeedChanged += OnGameSpeedChanged;
    }

    private void OnSettingsPopupShown(IEvent payload)
    {
        LevelModel.SetPauseMode(true);
    }

    private void OnSettingsPopupCloseClicked(IEvent payload)
    {
        LevelModel.SetPauseMode(false);

        if (_needToSaveSettings)
        {
            _needToSaveSettings = false;
            new SaveSettingsCommand().Execute();
        }
    }

    private void OnSettingsClicked(IEvent payload)
    {
        LevelModel.SetTimeScale(1);
    }

    private void OnGameSpeedChanged()
    {
        Time.timeScale = LevelModel.TimeScale;
    }

    private void OnAudioValueChanged(IEvent payload)
    {
        _needToSaveSettings = true;

        var data = payload.data as MediatorEventsParams.UiSettingsPopupAudioValueChangedParams;
        new SetAudioCommand().Execute(data.AudioVolume, data.MusicVolume, data.SoundsVolume);
    }
}
