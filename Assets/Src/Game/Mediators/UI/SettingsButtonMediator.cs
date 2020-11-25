using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class SettingsButtonMediator : EventMediator
{
    [Inject] public SettingsButtonView SettingsButtonView { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        SettingsButtonView.SettingsClicked += OnSettingsClicked;
    }

    private void OnDestroy()
    {
        SettingsButtonView.SettingsClicked -= OnSettingsClicked;
    }

    private void OnSettingsClicked()
    {
        dispatcher.Dispatch(MediatorEvents.UI_SETTINGS_CLICKED);
    }
}
