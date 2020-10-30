using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LosePopup : PopupBase
{
    public event Action MainMenuButtonClicked = delegate { };
    public event Action RestartButtonClicked = delegate { };

    [SerializeField] private Button _restartLevelBtn;
    [SerializeField] private Button _mainMenuBtn;
    [SerializeField] private Text _titleTxt;
    [SerializeField] private Text _infoTxt;

    public void SetTitle(string title)
    {
        _titleTxt.text = title;
    }

    public void SetInfo(string text)
    {
        _infoTxt.text = text;
    }

    protected override void Awake()
    {
        base.Awake();

        _mainMenuBtn.onClick.AddListener(OnMainMenuBtnClick);
        _restartLevelBtn.onClick.AddListener(OnRestartBtnClick);
    }

    private void OnMainMenuBtnClick()
    {
        Hide();
        MainMenuButtonClicked();
    }

    private void OnRestartBtnClick()
    {
        Hide();
        RestartButtonClicked();
    }
}
