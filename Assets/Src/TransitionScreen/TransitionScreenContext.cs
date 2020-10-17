using System.Collections;
using System.Collections.Generic;
using strange.extensions.context.impl;
using UnityEngine;

public class TransitionScreenContext : MVCSContext
{
    public TransitionScreenContext(TransitionScreenContextView view) : base(view)
    {
    }

    protected override void mapBindings()
    {
        base.mapBindings();

        var transitionScreenContextView = ((GameObject)contextView).GetComponent<TransitionScreenContextView>();

        mediationBinder.Bind<TransitionScreenView>().To<TransitionScreenViewMediator>();
    }
}
