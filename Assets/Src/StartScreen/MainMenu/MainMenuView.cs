using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class MainMenuView : View
{
    public event Action PlayButtonClicked = delegate { };
    public event Action SettingsButtonClicked = delegate { };
    public event Action HowToPlayButtonClicked = delegate { };

    [SerializeField] private ButtonView _playButton;
    [SerializeField] private ButtonView _settingsButton;
    [SerializeField] private ButtonView _howToPlayButton;

    public void SetTexts(string playText, string settingsText, string howToPlayText)
    {
        _playButton.SetText(playText);
        _settingsButton.SetText(settingsText);
        _howToPlayButton.SetText(howToPlayText);
    }

    protected override void Awake()
    {
        base.Awake();

        _playButton.OnClick += OnPlayClicked;
        _settingsButton.OnClick += OnSettingsClicked;
        _howToPlayButton.OnClick += OnHowToPlayClicked;
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
}
