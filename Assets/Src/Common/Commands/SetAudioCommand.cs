public struct SetAudioCommand
{
    public void Execute(float audioVolume, float musicVolume, float soundsVolume)
    {
        SetAudioVolumePercent(audioVolume);
        SetMusicVolumePercent(musicVolume);
        SetSoundsVolumePercent(soundsVolume);
    }

    private void SetAudioVolumePercent(float audioValue)
    {
        PlayerSessionModel.Model.AudioVolume = audioValue;

        AudioManager.Instance.SetMasterVolume(audioValue);
    }

    private void SetMusicVolumePercent(float musicValue)
    {
        PlayerSessionModel.Model.MusicVolume = musicValue;

        AudioManager.Instance.SetMusicVolume(musicValue);
    }

    private void SetSoundsVolumePercent(float soundsVolume)
    {
        PlayerSessionModel.Model.SoundsVolume = soundsVolume;

        AudioManager.Instance.SetSoundsVolume(soundsVolume);
    }
}
