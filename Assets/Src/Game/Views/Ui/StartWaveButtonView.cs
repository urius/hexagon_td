using UnityEngine;
using DigitalRuby.Tween;
using UnityEngine.UI;

public class StartWaveButtonView : ButtonView
{
    [SerializeField] private Animation _animation;
    [SerializeField] private Button _button;
    [SerializeField] private CanvasGroup _canvasGroup;

    public void SetActiveAnimated(bool isActive)
    {
        TweenFactory.RemoveTweenKey(_button, TweenStopBehavior.Complete);

        if (isActive) { gameObject.SetActive(true); }

        var alphaFrom = isActive ? 0f : 1f;
        var alphaTo = 1 - alphaFrom;
        _button.interactable = isActive;
        TweenFactory.Tween(_button, alphaFrom, alphaTo, 0.5f, TweenScaleFunctions.Linear,
            t => _canvasGroup.alpha = t.CurrentValue,
            t => gameObject.SetActive(isActive));

        _animation.Play("StartButton_idle");
    }

    protected override void OnDestroy()
    {
        TweenFactory.RemoveTweenKey(_button, TweenStopBehavior.DoNotModify);

        base.OnDestroy();
    }
}
