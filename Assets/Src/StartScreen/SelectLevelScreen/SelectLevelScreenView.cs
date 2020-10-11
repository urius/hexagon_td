using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class SelectLevelScreenView : View
{
    public event Action StartClicked = delegate { };

    [SerializeField] private Button _startButton;
    [SerializeField] private Text _lockedText;

    protected override void Awake()
    {
        base.Awake();

        _startButton.onClick.AddListener(OnStartClick);
    }

    private void OnStartClick()
    {
        StartClicked();
    }

    public void ShowLockedText(string text)
    {
        _lockedText.gameObject.SetActive(true);
        _lockedText.text = text;

        _startButton.gameObject.SetActive(false);
    }

    public void ShowStartButton()
    {
        _startButton.gameObject.SetActive(true);
        _lockedText.gameObject.SetActive(false);
    }
}
