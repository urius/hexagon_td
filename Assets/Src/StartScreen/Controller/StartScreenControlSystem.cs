using strange.extensions.dispatcher.eventdispatcher.api;

public class StartScreenControlSystem : EventSystemBase
{
    private bool _needToSaveSettings = false;

    public override void Start()
    {
        dispatcher.AddListener(MediatorEvents.UI_SETTINGS_POPUP_AUDIO_VALUE_CHANGED, OnSettingsPopupAudioValueChanged);
        dispatcher.AddListener(MediatorEvents.UI_SETTINGS_POPUP_CLOSE_CLICKED, OnSettingsPopupCloseClicked);
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
