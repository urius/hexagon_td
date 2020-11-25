using System;
using System.Collections;
using System.Collections.Generic;
using strange.extensions.injector.api;
using strange.extensions.mediation.api;
using UnityEngine;

public class GlobalBindingsHelper
{
    private readonly ICrossContextInjectionBinder injectionBinder;
    private readonly IMediationBinder mediationBinder;

    public GlobalBindingsHelper(ICrossContextInjectionBinder injectionBinder, IMediationBinder mediationBinder)
    {
        this.injectionBinder = injectionBinder;
        this.mediationBinder = mediationBinder;
    }

    public void Bind(GlobalObjectsHolder globalObjectsHolder)
    {
        injectionBinder.Bind<LevelsCollectionProvider>().ToValue(globalObjectsHolder.LevelsCollectionProvider).CrossContext();
        injectionBinder.Bind<LocalizationProvider>().ToValue(globalObjectsHolder.LocalizationProvider).CrossContext();
        injectionBinder.Bind<UIPrefabsConfig>().ToValue(globalObjectsHolder.UIPrefabsConfig).CrossContext();
        injectionBinder.Bind<PlayerGlobalModelHolder>().ToValue(globalObjectsHolder.PlayerGlobalModelHolder).CrossContext();
        injectionBinder.Bind<LevelConfigProvider>().ToValue(globalObjectsHolder.LevelConfigProvider).CrossContext();

        mediationBinder.Bind<LocalizedButtonView>().To<LocalizedButtonViewMediator>();
        mediationBinder.Bind<LocalizedTextView>().To<LocalizedTextViewMediator>();
    }
}

[Serializable]
public class GlobalObjectsHolder
{
    [SerializeField] public LocalizationProvider LocalizationProvider;
    [SerializeField] public LevelsCollectionProvider LevelsCollectionProvider;
    [SerializeField] public UIPrefabsConfig UIPrefabsConfig;
    [SerializeField] public PlayerGlobalModelHolder PlayerGlobalModelHolder;
    [SerializeField] public LevelConfigProvider LevelConfigProvider;
}
