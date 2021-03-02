using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;

public class StartLevelClickedCommand : EventCommand
{
    [Inject(ContextKeys.CROSS_CONTEXT_DISPATCHER)]
    public IEventDispatcher globalDispatcher { get; set; }

    public override async void Execute()
    {
        Retain();

        var availableBoosters = PlayerSessionModel.Instance.SelectedLevelConfig.GetAvailableBoosters();
        if (availableBoosters.Length <= 0)
        {
            var transitionHelper = new SwitchScenesWithTransitionSceneHelper(globalDispatcher);
            await transitionHelper.SwitchAsync(SceneNames.MainMenu, SceneNames.Game);
        } else
        {
            dispatcher.Dispatch(CommandEvents.UI_MENU_SCENE_REQUEST_BOOSTERS_SCREEN);
        }

        Release();
    }
}
