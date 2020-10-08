using System.Collections;
using System.Collections.Generic;
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

        //cross context
        injectionBinder.Bind<LocalizationProvider>().ToValue(mainMenuContextView.LocalizationProvider).CrossContext();
        injectionBinder.Bind<UIPrefabsConfig>().ToValue(mainMenuContextView.UIPrefabsConfig).CrossContext();

        //mediators
        mediationBinder.Bind<MainMenuView>().To<MainMenuViewMediator>();
        mediationBinder.Bind<LevelsScrollView>().To<LevelsScrollViewMediator>();
    }
}
