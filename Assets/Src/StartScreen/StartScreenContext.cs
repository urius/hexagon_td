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

        injectionBinder.Bind<DeafultPlayerGlobalModelProvider>().ToValue(mainMenuContextView.DeafultPlayerGlobalModelProvider);

        new GlobalBindingsHelper(injectionBinder).Bind(mainMenuContextView.GlobalObjectsHolder); 

        //mediators
        mediationBinder.Bind<MainMenuView>().To<MainMenuViewMediator>();
        mediationBinder.Bind<SelectLevelScreenView>().To<SelectLevelScreenViewMediator>();
        mediationBinder.Bind<LevelsScrollView>().To<LevelsScrollViewMediator>();
        mediationBinder.Bind<LocalizedButtonView>().To<LocalizedButtonViewMediator>();

        //commands
        commandBinder.Bind(ContextEvent.START).To<StartScreenStartedCommand>();
        commandBinder.Bind(MediatorEvents.UI_SL_START_LEVEL_CLICKED).To<StartLevelClickedCommand>();
    }
}
