using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class ScreenView : View
{
    [SerializeField] private Animation _animation;

    public void HideAnimated()
    {
        _animation.Play("Screen hide");
    }

    public void OnHideAnimationEnded()
    {
        Destroy(gameObject);
    }
}
