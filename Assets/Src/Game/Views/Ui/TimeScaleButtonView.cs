using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class TimeScaleButtonView : View
{
    public Action Clicked = delegate { };

    [SerializeField] private Button _button;
    [SerializeField] private Text _text;

    public void ShowSpeedMultiplier(int mult)
    {
        _text.text = string.Empty;
        for (var i = 0; i < mult; i++)
        {
            _text.text += "►";
        }
    }

    private void OnEnable()
    {
        _button.onClick.AddListener(OnButtonClick);
    }

    private void OnDisable()
    {
        _button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        Clicked();
    }
}
