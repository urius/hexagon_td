using System;
using UnityEngine;

public interface IScreenPanelViewProvider
{
    GameScreenPanelView ScreenPanelView { get; }
}

public class ScreenPanelViewHolder : IScreenPanelViewProvider
{
    private GameScreenPanelView _view;

    public ScreenPanelViewHolder()
    {
    }

    public void SetViev(GameScreenPanelView view)
    {
        _view = view;
    }

    public GameScreenPanelView ScreenPanelView => _view;
}
