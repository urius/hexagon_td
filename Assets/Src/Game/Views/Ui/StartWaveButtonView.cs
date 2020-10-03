using UnityEngine;

public class StartWaveButtonView : ButtonView
{
    [SerializeField] private Animation _animation;

    public void SetActiveAnimated(bool isActive)
    {
        _animation.Play(isActive ? "StartButton_show" : "StartButton_hide");
    }

    public void EventOnShowAnimationFinished()
    {
        _animation.Play("StartButton_idle");
    }
}
