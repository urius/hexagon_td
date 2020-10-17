using System.Collections;
using System.Collections.Generic;
using strange.extensions.context.impl;
using UnityEngine;

public class TransitionScreenContextView : ContextView
{
    private void Awake()
    {
        context = new TransitionScreenContext(this);
        context.Launch();
    }
}
