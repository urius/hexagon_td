using System;
using UnityEngine;

public class MainMenuView : ScreenView
{
    public event Action PlayButtonClicked = delegate { };
    public event Action SettingsButtonClicked = delegate { };
    public event Action HowToPlayButtonClicked = delegate { };
    public event Action SpecialThanksClicked = delegate { };

    [SerializeField] private ButtonView _playButton;
    [SerializeField] private ButtonView _settingsButton;
    [SerializeField] private ButtonView _howToPlayButton;
    [SerializeField] private ButtonView _specialThanksButton;

    public void SetTexts(string playText, string settingsText, string howToPlayText, string specialThanksText)
    {
        _playButton.SetText(playText);
        _settingsButton.SetText(settingsText);
        _howToPlayButton.SetText(howToPlayText);
        _specialThanksButton.SetText(specialThanksText);
    }

    protected override void Awake()
    {
        base.Awake();

        _playButton.OnClick += OnPlayClicked;
        _settingsButton.OnClick += OnSettingsClicked;
        _howToPlayButton.OnClick += OnHowToPlayClicked;
        _specialThanksButton.OnClick += OnSpecialThanksClicked;
    }

    private void OnHowToPlayClicked()
    {
        HowToPlayButtonClicked();
    }

    private void OnSettingsClicked()
    {
        SettingsButtonClicked();
    }

    private void OnPlayClicked()
    {
        PlayButtonClicked();
    }

    private void OnSpecialThanksClicked()
    {
        SpecialThanksClicked();
    }
}
