using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class TransitionScreenView : View
{
    public event Action ShowAnimationEnded = delegate { };
    public event Action HideAnimationEnded = delegate { };

    [SerializeField] private Text _loadingText;
    [SerializeField] private Text _percentText;
    [SerializeField] private Image _loadingImage;
    [SerializeField] private Animation _animation;

    public void SetLoadingText(string text)
    {
        _loadingText.text = text;
    }

    public void SetLoadPercent(int percent)
    {
        _percentText.text = percent.ToString();
    }

    public void HideAnim()
    {
        _animation.Play("Hide");
    }

    public void Event_ShowAnimationEnded()
    {
        ShowAnimationEnded();
    }

    public void Event_HideAnimationEnded()
    {
        HideAnimationEnded();
    }

    private void FixedUpdate()
    {
        _loadingImage.transform.Rotate(-Vector3.forward, 2);
    }
}
