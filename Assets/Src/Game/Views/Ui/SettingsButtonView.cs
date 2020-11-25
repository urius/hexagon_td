using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;
using UnityEngine.UI;

public class SettingsButtonView : View
{
    public event Action SettingsClicked = delegate { };

    [SerializeField] private Button _settingsButton;

    protected override void Awake()
    {
        base.Awake();

        _settingsButton.onClick.AddListener(OnSettingsClicked);
    }

    protected override void OnDestroy()
    {
        _settingsButton.onClick.RemoveListener(OnSettingsClicked);

        base.OnDestroy();
    }

    private void OnSettingsClicked()
    {
        SettingsClicked();
    }
}
