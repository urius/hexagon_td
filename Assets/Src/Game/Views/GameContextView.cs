using System;
using strange.extensions.context.impl;

public interface IUpdateProvider
{
    event Action UpdateAction;
}

public class GameContextView : ContextView, IUpdateProvider
{
    public event Action UpdateAction = delegate { };

    public LevelConfigProvider LevelConfigProvider;
    public CellConfigProvider CellConfigProvider;
    public UnitConfigsProvider UnitConfigsProvider;
    public TurretConfigProvider TurretConfigsProvider;
    public UIPrefabsConfig UIPrefabsConfig;
    public GridView GridView;
    public LocalizationProvider LocalizationProvider;

    private void Awake()
    {
        GameSettingsSetupHelper.Setup();

        context = new GameContext(this);
        context.Launch();
    }

    private void FixedUpdate()
    {
        UpdateAction();
    }
}

