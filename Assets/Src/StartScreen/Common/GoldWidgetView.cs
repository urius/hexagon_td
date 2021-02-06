using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class GoldWidgetView : View
{
    public event Action ButtonClicked = delegate { };

    [SerializeField] private Button _button;
    [SerializeField] private Text _amountText;
    [SerializeField] private Animator _animator;

    public void SetAmount(int amount)
    {
        _amountText.text = amount.ToString("# ### ##0");
    }

    public void ToShowState()
    {
        _animator.SetBool("CanShow", true);
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
        ButtonClicked();
    }
}
