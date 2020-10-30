using System.Collections;
using System.Collections.Generic;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class NextLevelClickedCommand : EventCommand
{
    [Inject(ContextKeys.CROSS_CONTEXT_DISPATCHER)]
    public IEventDispatcher globalDispatcher { get; set; }

    public override async void Execute()
    {
        Retain();

        //Advance level index or go to main if it is last level
        var transitionHelper = new SwitchScenesWithTransitionSceneHelper(globalDispatcher);        
        await transitionHelper.SwitchAsync(SceneNames.MainMenu, SceneNames.Game);

        Release();
    }
}
