using Cysharp.Threading.Tasks;
using strange.extensions.mediation.impl;
using UnityEngine;

public class PopupBase : View
{
    [SerializeField] private Animation _animation;

    private UniTaskCompletionSource<bool> _showTsc = new UniTaskCompletionSource<bool>();

    public UniTask ShowTask => _showTsc.Task;

    public void EventOnShowAnimationFinished()
    {
        _showTsc.TrySetResult(true);
    }

    public void EventOnHideAnimationFinished()
    {
        Destroy(gameObject);
    }

    public void Hide()
    {
        _animation.Play("Popup hide");
    }
}
