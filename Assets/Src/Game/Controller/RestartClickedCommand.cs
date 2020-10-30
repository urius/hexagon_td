using System.Collections;
using System.Collections.Generic;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class RestartClickedCommand : EventCommand
{
    [Inject(ContextKeys.CROSS_CONTEXT_DISPATCHER)]
    public IEventDispatcher globalDispatcher { get; set; }

    public override async void Execute()
    {
        Retain();

        var transitionHelper = new SwitchScenesWithTransitionSceneHelper(globalDispatcher);
        await transitionHelper.SwitchAsync(SceneNames.Game, SceneNames.Game);

        Release();
    }
}
