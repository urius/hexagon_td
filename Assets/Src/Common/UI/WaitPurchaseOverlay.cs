using DG.Tweening;
using UnityEngine;

public class WaitPurchaseOverlay : MonoBehaviour
{
    [SerializeField] private CanvasGroup _canvasGroup;
    [SerializeField] private RectTransform _loadingImageTransform;

    void Start()
    {
        _canvasGroup.alpha = 0;
        gameObject.SetActive(false);
    }

    public void ToWaitMode()
    {
        _canvasGroup.alpha = 0;
        gameObject.SetActive(true);

        _canvasGroup.DOKill();
        _canvasGroup.DOFade(1f, 0.5f);
        
        //TweenFactory.RemoveTweenKey(this, TweenStopBehavior.DoNotModify);
        //TweenFactory.Tween(this, 0f, 1f, 0.5f, TweenScaleFunctions.Linear, t => _canvasGroup.alpha = t.CurrentValue);
    }

    public void ToDefaultMode()
    {
        _canvasGroup.DOKill();
        _canvasGroup.DOFade(0f, 0.5f);
        
        //TweenFactory.RemoveTweenKey(this, TweenStopBehavior.DoNotModify);
        //TweenFactory.Tween(this, 1f, 0f, 0.5f, TweenScaleFunctions.Linear, t => _canvasGroup.alpha = t.CurrentValue, _ => gameObject.SetActive(false));
    }

    private void Update()
    {
        _loadingImageTransform.Rotate(0, 0, -10);
    }

    private void OnDestroy()
    {
        _canvasGroup.DOKill();
        //TweenFactory.RemoveTweenKey(this, TweenStopBehavior.DoNotModify);
    }
}
