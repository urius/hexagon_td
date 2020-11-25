using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class TimeScaleButtonMediator : EventMediator
{
    [Inject] public TimeScaleButtonView View { get; set; }

    private void Start()
    {
        View.Clicked += OnChangeTimeScaleClicked;

        View.ShowSpeedMultiplier((int)Time.timeScale);
    }

    private void OnDestroy()
    {
        View.Clicked -= OnChangeTimeScaleClicked;
    }

    private void OnChangeTimeScaleClicked()
    {
        dispatcher.Dispatch(MediatorEvents.UI_TIME_SCALE_CHANGE_CLICKED);

        View.ShowSpeedMultiplier((int)Time.timeScale);
    }
}
