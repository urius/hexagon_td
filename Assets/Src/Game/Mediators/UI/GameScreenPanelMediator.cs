using System;
using strange.extensions.mediation.impl;
using UnityEngine;

public class GameScreenPanelMediator : EventMediator
{
    [Inject] public GameScreenPanelView ScreenPanelView { get; set; }
    [Inject] public ScreenPanelViewHolder ScreenPanelViewHolder { get; set; }
    [Inject] public ICellPositionConverter CellPositionConverter { get; set; }
    [Inject] public WorldMousePositionProvider WorldMousePositionProvider { get; set; }

    private Vector3 _screenMouseDownPos;
    private float _clickSensitivityPixelsSqr;

    public override void OnRegister()
    {
        base.OnRegister();

        _clickSensitivityPixelsSqr = (float)Math.Pow(Screen.width * 0.02f, 2);
        ScreenPanelViewHolder.SetViev(ScreenPanelView);

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

        _screenMouseDownPos = Input.mousePosition;
    }

    private void OnGameScreenPointerUp()
    {
        dispatcher.Dispatch(MediatorEvents.UI_GAME_SCREEN_MOUSE_UP);

        if ((_screenMouseDownPos - Input.mousePosition).sqrMagnitude < _clickSensitivityPixelsSqr)
        {
            var worldMousePosition = WorldMousePositionProvider.Position;
            var cellMousePosition = CellPositionConverter.CellVec3ToVec2(CellPositionConverter.WorldToCell(worldMousePosition));

            dispatcher.Dispatch(MediatorEvents.UI_GAME_SCREEN_CLICK, cellMousePosition);
        }
    }
}
