using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;

public class PlayLevelCommand : EventCommand
{
    [Inject(ContextKeys.CROSS_CONTEXT_DISPATCHER)]
    public IEventDispatcher globalDispatcher { get; set; }

    private static bool IsExecutingFlag = false;

    public override async void Execute()
    {
        if (IsExecutingFlag) return;
        IsExecutingFlag = true;
        Retain();

        var transitionHelper = new SwitchScenesWithTransitionSceneHelper(globalDispatcher);
        await transitionHelper.SwitchAsync(SceneNames.MainMenu, SceneNames.Game);

        Release();
        IsExecutingFlag = false;
    }
}
