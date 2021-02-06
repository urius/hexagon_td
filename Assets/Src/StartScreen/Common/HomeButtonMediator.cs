using System;
using System.Collections;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using DigitalRuby.Tween;
using strange.extensions.mediation.impl;
using UnityEngine;

public class HomeButtonMediator : EventMediator
{
    [Inject] public HomeButtonView HomeButtonView { get; set; }

    public override async void OnRegister()
    {
        base.OnRegister();

        HomeButtonView.OnClick += OnClickHandler;

        HomeButtonView.Alpha = 0;

        await UniTask.Delay(400);
        TweenFactory.Tween(this, 0f, 1f, 0.2f, TweenScaleFunctions.Linear, t => HomeButtonView.Alpha = t.CurrentValue);
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
