using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LosePopup : PopupBase
{
    public event Action MainMenuButtonClicked = delegate { };
    public event Action ContinueButtonClicked = delegate { };

    [SerializeField] private Button _continueLevelBtn;
    [SerializeField] private Button _mainMenuBtn;
    [SerializeField] private Text _titleTxt;
    [SerializeField] private Text _infoTxt;
    [SerializeField] private Text _continuePriceTxt;
    [SerializeField] private GameObject _goldWidgetGO;

    public void SetTitle(string title)
    {
        _titleTxt.text = title;
    }

    public void SetInfo(string text)
    {
        _infoTxt.text = text;
    }

    public void SetContinuePrice(int goldPrice)
    {
        _continuePriceTxt.text = goldPrice.ToSpaceSeparatedAmount();
    }

    protected override void Awake()
    {
        base.Awake();

        _mainMenuBtn.onClick.AddListener(OnMainMenuBtnClick);
        _continueLevelBtn.onClick.AddListener(OnContinueBtnClick);
    }

    protected override async void Start()
    {
        base.Start();

        await ShowTask;
        _goldWidgetGO.SetActive(true);
    }

    private void OnMainMenuBtnClick()
    {
        Hide();
        MainMenuButtonClicked();
    }

    private void OnContinueBtnClick()
    {
        ContinueButtonClicked();
    }
}
