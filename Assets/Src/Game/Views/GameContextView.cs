using System;
using DG.Tweening;
using strange.extensions.context.impl;
using UnityEngine;

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

    private void Awake()
    {
        Application.targetFrameRate = 50;

        DOTween.Init(true, false, LogBehaviour.ErrorsOnly).SetCapacity(50, 0);
        DOTween.defaultEaseType = Ease.Linear;

        context = new GameContext(this);
        context.Launch();
    }

    private void FixedUpdate()
    {
        UpdateAction();
    }
}

