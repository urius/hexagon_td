using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class StartWaveButtonView : View
{
    public event Action OnClick = delegate { };

    [SerializeField] private Text _text;
    [SerializeField] private Animation _animation;

    public void SetText(string text)
    {
        _text.text = text;        
    }

    public void SetActiveAnimated(bool isActive)
    {
        _animation.Play(isActive ? "StartButton_show" : "StartButton_hide");
    }

    public void EventOnClick()
    {
        OnClick();
    }

    public void EventOnShowAnimationFinished()
    {
        _animation.Play("StartButton_idle");
    }
}
