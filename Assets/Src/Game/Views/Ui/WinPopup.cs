using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class WinPopup : PopupBase
{
    [SerializeField] private Button _nextLevelBtn;
    [SerializeField] private Button _mainMenuBtn;
    [SerializeField] private Text _titleTxt;
    [SerializeField] private Text _infoTxt;
    [SerializeField] private Image _star1Image;
    [SerializeField] private Image _star2Image;
    [SerializeField] private Image _star3Image;
    [SerializeField] private Color _filledStarColor;

    private void Awake()
    {
        _mainMenuBtn.onClick.AddListener(OnMainMenuBtnClick);
        _nextLevelBtn.onClick.AddListener(OnNextLevelBtnClick);
    }

    private void OnMainMenuBtnClick()
    {
        Hide();
    }

    private void OnNextLevelBtnClick()
    {
        Hide();
    }
}
