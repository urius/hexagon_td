using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingsPopupForStartScreen : PopupBase
{
    public event Action CloseBtnClicked = delegate { };
    public event Action SliderValueChanged = delegate { };

    [SerializeField] private Button _closeBtnX;
    [SerializeField] private Button _closeBtn;
    [SerializeField] private Slider _volumeSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private Slider _soundsSlider;
    [SerializeField] private InputField _idText;

    public float AudioVolume => _volumeSlider.value;
    public float MusicVolume => _musicSlider.value;
    public float SoundsVolume => _soundsSlider.value;

    public void SetupVolumeIndicators(float master, float music, float sounds)
    {
        UnsubscribeFromSliders();

        _volumeSlider.value = master;
        _musicSlider.value = music;
        _soundsSlider.value = sounds;

        SubscribeToSliders();
    }

    public void SetId(string id)
    {
        _idText.text = id;
    }

    protected override void Awake()
    {
        base.Awake();

        _closeBtnX.onClick.AddListener(OnCloseBtnClick);
        _closeBtn.onClick.AddListener(OnCloseBtnClick);

        SubscribeToSliders();
    }

    private void SubscribeToSliders()
    {
        UnsubscribeFromSliders();
        _volumeSlider.onValueChanged.AddListener(OnVolumeSliderValueChanged);
        _musicSlider.onValueChanged.AddListener(OnMusicSliderValueChanged);
        _soundsSlider.onValueChanged.AddListener(OnSoundsSliderValueChanged);
    }

    protected override void OnDestroy()
    {
        _closeBtnX.onClick.RemoveListener(OnCloseBtnClick);
        _closeBtn.onClick.RemoveListener(OnCloseBtnClick);

        UnsubscribeFromSliders();

        base.OnDestroy();
    }

    private void UnsubscribeFromSliders()
    {
        _volumeSlider.onValueChanged.RemoveListener(OnVolumeSliderValueChanged);
        _musicSlider.onValueChanged.RemoveListener(OnMusicSliderValueChanged);
        _soundsSlider.onValueChanged.RemoveListener(OnSoundsSliderValueChanged);
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
}
