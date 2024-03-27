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

        new SaveUserDataCommand().Execute();

        var transitionHelper = new SwitchScenesWithTransitionSceneHelper(globalDispatcher);
        await transitionHelper.SwitchAsync(SceneNames.Game, SceneNames.MainMenu);

        Release();
    }
}
