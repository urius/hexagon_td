using System;
using strange.extensions.context.impl;
using UnityEngine;

public interface IUpdateProvider
{
    event Action UpdateAction;
    event Action UpdateActionRealtime;
}

public interface IRootTransformProvider
{
    Transform transform { get; }
}

public class GameContextView : ContextView, IUpdateProvider, IRootTransformProvider
{
    public event Action UpdateAction = delegate { };
    public event Action UpdateActionRealtime = delegate { };

    public GlobalObjectsHolder GlobalObjectsHolder;
    public CellConfigProvider CellConfigProvider;
    public UnitConfigsProvider UnitConfigsProvider;
    public TurretConfigProvider TurretConfigsProvider;
    public GUIPrefabsConfig GUIPrefabsConfig;
    public GridView GridView;

    private bool _isInitialized = false;
    private LevelModel _levelModel;

    private async void Awake()
    {
        var gameContext = new GameContext(this);

        context = gameContext;
        Context.firstContext = context;
        context.Launch();

        await gameContext.InitializedTask;

        _isInitialized = true;
        _levelModel = gameContext.LevelModel;
    }

    private void FixedUpdate()
    {
        if (_isInitialized)
        {
            if (!_levelModel.IsPaused)
            {
                UpdateAction();
            }
            UpdateActionRealtime();
        }
    }
}

