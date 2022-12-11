﻿using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Cysharp.Threading.Tasks;
using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;

public class MainMenuClickedCommand : EventCommand
{
    [Inject(ContextKeys.CROSS_CONTEXT_DISPATCHER)]
    public IEventDispatcher globalDispatcher { get; set; }

    public override async void Execute()
    {
        Retain();

        var saveDataTask = new SaveUserDataCommand().Execute();

        var transitionHelper = new SwitchScenesWithTransitionSceneHelper(globalDispatcher);
        var switchScenesTask = transitionHelper.SwitchAsync(SceneNames.Game, SceneNames.MainMenu).AsUniTask();

        await switchScenesTask;

        Release();
    }
}
