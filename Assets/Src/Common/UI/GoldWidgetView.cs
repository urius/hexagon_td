using System;
using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using DigitalRuby.Tween;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class GoldWidgetView : View
{
    public event Action ButtonClicked = delegate { };

    [SerializeField] private Button _button;
    [SerializeField] private Text _amountText;
    [SerializeField] private Animator _animator;
    [SerializeField] private RectTransform _rectTransform;
    public RectTransform RectTransform => _rectTransform;
    [SerializeField] private RectTransform _flyTargerRectTransform;
    public RectTransform FlyTargerRectTransform => _flyTargerRectTransform;

    private int _currentAmount;

    public void SetAmount(int amount)
    {
        TweenFactory.RemoveTweenKey(this, TweenStopBehavior.DoNotModify);
        _currentAmount = amount;
        _amountText.text = amount.ToSpaceSeparatedAmount();
    }

    public void SetAmountAnimated(int amount)
    {
        AudioManager.Instance.Play(SoundId.DNAAmountChanged);

        TweenFactory.RemoveTweenKey(this, TweenStopBehavior.DoNotModify);
        TweenFactory.Tween(this, _currentAmount, amount, 1f, TweenScaleFunctions.CubicEaseOut,
            t => _amountText.text = ((int)t.CurrentValue).ToSpaceSeparatedAmount(),
            t => _currentAmount = amount);
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
        TweenFactory.RemoveTweenKey(this, TweenStopBehavior.DoNotModify);

        _button.onClick.RemoveListener(OnButtonClick);
    }

    private void OnButtonClick()
    {
        ButtonClicked();
    }
}
