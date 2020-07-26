using strange.extensions.mediation.impl;

public class GameScreenPanelMediator : EventMediator
{
    [Inject] public GameScreenPanelView ScreenPanelView { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        ScreenPanelView.PointerDown += OnGameScreenPointerDown;
        ScreenPanelView.PointerUp += OnGameScreenPointerUp;
    }

    public override void OnRemove()
    {
        base.OnRemove();

        ScreenPanelView.PointerDown -= OnGameScreenPointerDown;
        ScreenPanelView.PointerUp -= OnGameScreenPointerUp;
    }

    private void OnGameScreenPointerDown()
    {
        dispatcher.Dispatch(MediatorEvents.UI_GAME_SCREEN_MOUSE_DOWN);
    }

    private void OnGameScreenPointerUp()
    {
        dispatcher.Dispatch(MediatorEvents.UI_GAME_SCREEN_MOUSE_UP);
    }
}
