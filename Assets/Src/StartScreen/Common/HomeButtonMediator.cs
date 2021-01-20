using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class HomeButtonMediator : EventMediator
{
    [Inject] public HomeButtonView HomeButtonView { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        HomeButtonView.OnClick += OnClickHandler;
    }

    public override void OnRemove()
    {
        HomeButtonView.OnClick -= OnClickHandler;

        base.OnRemove();
    }

    private void OnClickHandler()
    {
        dispatcher.Dispatch(MediatorEvents.UI_HOME_CLICKED);
    }
}
