using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.mediation.impl;
using UnityEngine;

public class DebugPanelMediator : EventMediator
{
    [Inject] public DebugPanelView DebugPanelView { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        DebugPanelView.Button1.onClick.AddListener(OnButton1Click);
    }

    public override void OnRemove()
    {
        DebugPanelView.Button1.onClick.RemoveAllListeners();

        base.OnRemove();
    }

    private void OnButton1Click()
    {
        dispatcher.Dispatch(MediatorEvents.DEBUG_BUTTON_CLICKED, 1);
    }
}
