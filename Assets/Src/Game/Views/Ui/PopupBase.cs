using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class PopupBase : MonoBehaviour
{
    [SerializeField] private Animation _animation;

    private TaskCompletionSource<bool> _showTsc = new TaskCompletionSource<bool>();

    protected Task ShowTask => _showTsc.Task;

    public void EventOnShowAnimationFinished()
    {
        _showTsc.SetResult(true);
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
