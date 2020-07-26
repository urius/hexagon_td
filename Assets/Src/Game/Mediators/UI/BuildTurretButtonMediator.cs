using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class BuildTurretButtonMediator : EventMediator
{
    [Inject] public BuildTurretButtonView view { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        view.ButtonPointerDown += OnButtonPointerDown;
        view.ButtonPointerUp += OnButtonPointerUp;
    }

    public override void OnRemove()
    {
        base.OnRemove();

        view.ButtonPointerDown -= OnButtonPointerDown;
        view.ButtonPointerUp -= OnButtonPointerUp;
    }

    private void OnButtonPointerDown()
    {
        dispatcher.Dispatch(MediatorEvents.UI_BUILD_TURRET_MOUSE_DOWN, view.TurretTypeId);
    }

    private void OnButtonPointerUp()
    {
        dispatcher.Dispatch(MediatorEvents.UI_BUILD_TURRET_MOUSE_UP, view.TurretTypeId);
    }
}
