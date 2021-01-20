using System;
using System.Collections;
using System.Collections.Generic;
using DigitalRuby.Tween;
using strange.extensions.mediation.impl;
using UnityEngine;

public class HowToPlayScreenMediator : EventMediator
{
    [Inject] public HowToPlayScreenView HowToPlayScreenView { get; set; }

    private int _shownSlideIndex = 0;

    public override void OnRegister()
    {
        base.OnRegister();

        HowToPlayScreenView.NextButton.onClick.AddListener(OnNextClicked);
        HowToPlayScreenView.RightArrowButton.onClick.AddListener(OnNextClicked);
        HowToPlayScreenView.LeftArrowButton.onClick.AddListener(OnPrevClicked);
    }

    public override void OnRemove()
    {
        HowToPlayScreenView.NextButton.onClick.RemoveListener(OnNextClicked);
        HowToPlayScreenView.RightArrowButton.onClick.RemoveListener(OnNextClicked);
        HowToPlayScreenView.LeftArrowButton.onClick.RemoveListener(OnPrevClicked);

        base.OnRemove();
    }

    private void Start()
    {
        ShowSlide(_shownSlideIndex);
    }

    private void ShowSlide(int index, float duration = 0)
    {
        var slides = HowToPlayScreenView.SlideCanvasGroups;
        for (var i = 0; i < slides.Length; i++)
        {
            var targetAlpha = (i == index) ? 1 : 0;
            TweenSlideVisibility(slides[i], targetAlpha, duration);
        }
    }

    private void TweenSlideVisibility(CanvasGroup canvasGroup, int targetAlpha, float duration)
    {
        TweenFactory.Tween(canvasGroup, canvasGroup.alpha, targetAlpha, duration, TweenScaleFunctions.CubicEaseInOut, (t) => canvasGroup.alpha = t.CurrentValue);
    }

    private void OnNextClicked()
    {
        _shownSlideIndex = Math.Min(_shownSlideIndex + 1, HowToPlayScreenView.SlideCanvasGroups.Length - 1);
        ShowSlide(_shownSlideIndex, 0.5f);
    }

    private void OnPrevClicked()
    {
        _shownSlideIndex = Math.Max(_shownSlideIndex - 1, 0);
        ShowSlide(_shownSlideIndex, 0.5f);
    }
}
