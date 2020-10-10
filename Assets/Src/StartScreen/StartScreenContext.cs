using System.Collections;
using System.Collections.Generic;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using UnityEngine;

public class StartScreenContext : MVCSContext
{
    public StartScreenContext(StartScreenContextView view) : base(view)
    {
    }

    protected override void mapBindings()
    {
        base.mapBindings();

        var mainMenuContextView = ((GameObject)contextView).GetComponent<StartScreenContextView>();

        injectionBinder.Bind<LevelsCollectionProvider>().ToValue(mainMenuContextView.LevelsCollectionProvider);
        injectionBinder.Bind<DeafultPlayerGlobalModelProvider>().ToValue(mainMenuContextView.DeafultPlayerGlobalModelProvider);

        //cross context
        injectionBinder.Bind<LocalizationProvider>().ToValue(mainMenuContextView.LocalizationProvider).CrossContext();
        injectionBinder.Bind<UIPrefabsConfig>().ToValue(mainMenuContextView.UIPrefabsConfig).CrossContext();
        injectionBinder.Bind<PlayerGlobalModelHolder>().ToSingleton().CrossContext();

        //mediators
        mediationBinder.Bind<MainMenuView>().To<MainMenuViewMediator>();
        mediationBinder.Bind<LevelsScrollView>().To<LevelsScrollViewMediator>();

        //commands
        commandBinder.Bind(ContextEvent.START).To<StartScreenStartedCommand>();
    }
}
