using System;
using DG.Tweening;
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
        HowToPlayScreenView.FinishButton.onClick.AddListener(OnFinishClicked);
    }

    public override void OnRemove()
    {
        HowToPlayScreenView.NextButton.onClick.RemoveListener(OnNextClicked);
        HowToPlayScreenView.RightArrowButton.onClick.RemoveListener(OnNextClicked);
        HowToPlayScreenView.LeftArrowButton.onClick.RemoveListener(OnPrevClicked);
        HowToPlayScreenView.FinishButton.onClick.AddListener(OnFinishClicked);

        base.OnRemove();
    }

    private void Start()
    {
        HowToPlayScreenView.SlideCanvasGroups[0].gameObject.SetActive(true);
        ShowSlide(_shownSlideIndex);

        AnalyticsManager.Instance.SendHowToPlayOpened();
    }

    private void ShowSlide(int index, float duration = 0)
    {
        var slides = HowToPlayScreenView.SlideCanvasGroups;
        for (var i = 0; i < slides.Length; i++)
        {
            var targetAlpha = (i == index) ? 1 : 0;
            TweenSlideVisibility(slides[i], targetAlpha, duration);
        }

        UpdateNavButtons(index);
    }

    private void UpdateNavButtons(int index)
    {
        HowToPlayScreenView.LeftArrowButton.interactable = index != 0;
        var isLastSlide = index == HowToPlayScreenView.SlideCanvasGroups.Length - 1;
        HowToPlayScreenView.RightArrowButton.interactable = !isLastSlide;
        HowToPlayScreenView.NextButton.gameObject.SetActive(!isLastSlide);
        HowToPlayScreenView.FinishButton.gameObject.SetActive(isLastSlide);
    }

    private void TweenSlideVisibility(CanvasGroup canvasGroup, int targetAlpha, float duration)
    {
        if ((targetAlpha >= 1 && canvasGroup.alpha >= targetAlpha) || (targetAlpha <= 0 && canvasGroup.alpha <= 0))
        {
            return;
        }

        canvasGroup.gameObject.SetActive(true);
        
        canvasGroup.DOFade(targetAlpha, duration)
            .SetEase(Ease.InOutCubic)
            .OnComplete(() => canvasGroup.gameObject.SetActive(canvasGroup.alpha > 0));
            
        // TweenFactory.Tween(canvasGroup, canvasGroup.alpha, targetAlpha, duration, TweenScaleFunctions.CubicEaseInOut,
        //     t => canvasGroup.alpha = t.CurrentValue,
        //     t => canvasGroup.gameObject.SetActive(t.CurrentValue > 0));
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

    private void OnFinishClicked()
    {
        dispatcher.Dispatch(MediatorEvents.UI_HOME_CLICKED);

        AnalyticsManager.Instance.SendHowToPlayFinishClicked();
    }
}
