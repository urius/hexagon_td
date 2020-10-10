using System.Collections;
using System.Collections.Generic;
using strange.extensions.context.impl;
using UnityEngine;

public class StartScreenContextView : ContextView
{
    [SerializeField] public LocalizationProvider LocalizationProvider;
    [SerializeField] public LevelsCollectionProvider LevelsCollectionProvider;
    [SerializeField] public UIPrefabsConfig UIPrefabsConfig;
    [SerializeField] public DeafultPlayerGlobalModelProvider DeafultPlayerGlobalModelProvider;    

    private void Awake()
    {
        GameSettingsSetupHelper.Setup();

        context = new StartScreenContext(this);
        context.Launch();
    }
}
