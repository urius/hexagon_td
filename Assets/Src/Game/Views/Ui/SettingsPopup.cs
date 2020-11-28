using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopup : PopupBase
{
    public event Action CloseBtnClicked = delegate { };
    public event Action MainMenuBtnClicked = delegate { };
    public event Action SliderValueChanged = delegate { };

    [SerializeField] private Button _mainMenuBtn;
    [SerializeField] private Button _resumeBtn;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundsSlider;

    public float AudioVolume => _volumeSlider.value;
    public float MusicVolume => _musicSlider.value;
    public float SoundsVolume => _soundsSlider.value;

    public void SetVolumeIndicators(float master, float music, float sounds)
    {
        _volumeSlider.value = master;
        _musicSlider.value = music;
        _soundsSlider.value = sounds;
    }

    protected override void Awake()
    {
        base.Awake();

        _mainMenuBtn.onClick.AddListener(OnMainMenuBtnClick);
        _resumeBtn.onClick.AddListener(OnCloseBtnClick);
        _closeBtn.onClick.AddListener(OnCloseBtnClick);

        _volumeSlider.onValueChanged.AddListener(OnVolumeSliderValueChanged);
        _musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
        _soundsSlider.onValueChanged.AddListener(OnSoundsSliderValueChanged);
    }

    protected override void OnDestroy()
    {
        _mainMenuBtn.onClick.RemoveListener(OnMainMenuBtnClick);
        _resumeBtn.onClick.RemoveListener(OnCloseBtnClick);
        _closeBtn.onClick.RemoveListener(OnCloseBtnClick);

        _volumeSlider.onValueChanged.RemoveListener(OnVolumeSliderValueChanged);
        _musicSlider.onValueChanged.RemoveListener(OnMusicSliderValueChanged);
        _soundsSlider.onValueChanged.RemoveListener(OnSoundsSliderValueChanged);

        base.OnDestroy();
    }

    private void OnVolumeSliderValueChanged(float value)
    {
        SliderValueChanged();
    }

    private void OnMusicSliderValueChanged(float value)
    {
        SliderValueChanged();
    }

    private void OnSoundsSliderValueChanged(float arg0)
    {
        SliderValueChanged();
    }

    private void OnCloseBtnClick()
    {
        Hide();
        CloseBtnClicked();
    }

    private void OnMainMenuBtnClick()
    {
        MainMenuBtnClicked();
    }
}
