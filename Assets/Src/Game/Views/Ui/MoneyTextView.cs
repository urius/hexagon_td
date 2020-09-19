using System;
using System.Globalization;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class MoneyTextView : View
{
    [SerializeField] private Text _text;
    [SerializeField] private Animation _blinkAnimation;

    private NumberFormatInfo _numberFormatInfo;
    private int _targetAmount;
    private int _deltaAmount;
    private int _currentAmount;
    private bool _isAnimating = false;

    public void SetAmount(int amount, bool animated)
    {
        if (animated)
        {
            _isAnimating = true;
            _targetAmount = amount;
            var delta = _targetAmount - _currentAmount;
            _deltaAmount = (int)Math.Ceiling(delta / 10f);
        }
        else
        {
            _isAnimating = false;
            SetAmountInternal(amount);
        }
    }

    public void Blink()
    {
        _blinkAnimation.Play();
    }

    protected override void Awake()
    {
        base.Awake();

        _numberFormatInfo = (NumberFormatInfo)CultureInfo.InvariantCulture.NumberFormat.Clone();
    }

    private void SetAmountInternal(int amount)
    {
        _currentAmount = amount;
        _text.text = $"{amount.ToString("# ### ##0", _numberFormatInfo)}$";
    }

    private void Update()
    {
        if (_isAnimating)
        {
            if (Math.Abs(_targetAmount - _currentAmount) <= Math.Abs(_deltaAmount))
            {
                _currentAmount = _targetAmount;
                _isAnimating = false;
            }
            else
            {
                _currentAmount += _deltaAmount;
            }

            SetAmountInternal(_currentAmount);
        }
    }
}
