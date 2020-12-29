using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPopup : PopupBase
{
    public event Action MainMenuButtonClicked = delegate { };
    public event Action NextLevelButtonClicked = delegate { };

    [SerializeField] private Button _nextLevelBtn;
    [SerializeField] private Button _mainMenuBtn;
    [SerializeField] private Text _titleTxt;
    [SerializeField] private Text _infoTxt;
    [SerializeField] private Animation[] _starsAnimations;

    public void SetTitle(string title)
    {
        _titleTxt.text = title;
    }

    public void SetInfo(string text)
    {
        _infoTxt.text = text;
    }

    public void AnimateStar(int starIndex)
    {
        _starsAnimations[starIndex].Play("Star earn");
    }

    protected override void Awake()
    {
        base.Awake();

        _mainMenuBtn.onClick.AddListener(OnMainMenuBtnClick);
        _nextLevelBtn.onClick.AddListener(OnNextLevelBtnClick);
    }

    private void OnMainMenuBtnClick()
    {
        Hide();
        MainMenuButtonClicked();
    }

    private void OnNextLevelBtnClick()
    {
        Hide();
        NextLevelButtonClicked();
    }
}
