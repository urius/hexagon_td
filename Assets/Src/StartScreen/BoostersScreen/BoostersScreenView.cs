using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class BoostersScreenView : ScreenView
{
    public event Action PlayClicked = delegate { };

    [SerializeField] private Text _levelTitleText;
    [SerializeField] private Text _boostersTitleText;

    [SerializeField] private RectTransform _contentContainer;
    public RectTransform ContentContainerTransform => _contentContainer;

    [SerializeField] private Button _playButton;

    public void SetupTexts(string levelTitle, string boostersTitle)
    {
        _levelTitleText.text = levelTitle;
        _boostersTitleText.text = boostersTitle;
    }

    private void OnEnable()
    {
        _playButton.onClick.AddListener(OnPlayClick);
    }

    private void OnDisable()
    {
        _playButton.onClick.RemoveListener(OnPlayClick);
    }

    private void OnPlayClick()
    {
        PlayClicked();
    }
}
