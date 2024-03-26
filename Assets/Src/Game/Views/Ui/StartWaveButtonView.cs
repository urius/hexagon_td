using UnityEngine;
using UnityEngine.UI;

public class StartWaveButtonView : ButtonView
{
    [SerializeField] private Animation _animation;
    [SerializeField] private Button _button;
    [SerializeField] private CanvasGroup _canvasGroup;

    public void SetActiveAnimated(bool isActive)
    {
        gameObject.SetActive(isActive);
        _canvasGroup.alpha = isActive ? 1 : 0;

        _animation.Play("StartButton_idle");
    }
}
