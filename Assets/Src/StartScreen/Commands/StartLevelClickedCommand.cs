using strange.extensions.command.impl;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;

public class StartLevelClickedCommand : EventCommand
{
    [Inject(ContextKeys.CROSS_CONTEXT_DISPATCHER)]
    public IEventDispatcher globalDispatcher { get; set; }

    public override void Execute()
    {
        var availableBoosters = PlayerSessionModel.Instance.SelectedLevelConfig.GetAvailableBoosters();
        if (availableBoosters.Length <= 0)
        {
            dispatcher.Dispatch(CommandEvents.UI_MENU_SCENE_REQUEST_PLAY_LEVEL);
        } else
        {
            dispatcher.Dispatch(CommandEvents.UI_MENU_SCENE_REQUEST_BOOSTERS_SCREEN);
        }
    }
}
