using System;
using strange.extensions.context.impl;
using UnityEngine;

public interface IUpdateProvider
{
    event Action UpdateAction;
}

public interface IRootTransformProvider
{
    Transform transform { get; }
}

public class GameContextView : ContextView, IUpdateProvider, IRootTransformProvider
{
    public event Action UpdateAction = delegate { };

    public GlobalObjectsHolder GlobalObjectsHolder;
    public CellConfigProvider CellConfigProvider;
    public UnitConfigsProvider UnitConfigsProvider;
    public TurretConfigProvider TurretConfigsProvider;
    public GridView GridView;

    private void Awake()
    {
        GameSettingsSetupHelper.Setup();

        context = new GameContext(this);
        Context.firstContext = context;
        context.Launch();
    }

    private void FixedUpdate()
    {
        UpdateAction();
    }
}

