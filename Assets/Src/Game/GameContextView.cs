using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using strange.extensions.context.api;
using strange.extensions.context.impl;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class GameContextView : ContextView
{
    [SerializeField] public LevelConfigProvider LevelConfigProvider;
    [SerializeField] public CellConfigProvider CellConfigProvider;
    [SerializeField] public UnitConfigsProvider UnitConfigsProvider;
    [SerializeField] public GridView GridView;

    private void Awake()
    {
        context = new GameContext(this);
        context.Launch();
    }
}

public class GameContextViewMediator : EventMediator
{
    [Inject(ContextKeys.CONTEXT_DISPATCHER)] public IEventDispatcher eventDispatcher { get; set; }

    public override void OnRegister()
    {
        base.OnRegister();

        DOTween.Init(true, false, LogBehaviour.ErrorsOnly).SetCapacity(50, 0);
        DOTween.defaultEaseType = Ease.Linear;

        StartCoroutine(DispatchSecondPassed());
    }

    private IEnumerator DispatchSecondPassed()
    {
        while (true)
        {
            eventDispatcher.Dispatch(MediatorEvents.SECOND_PASSED);
            yield return new WaitForSeconds(1f);
        }
    }
}






public class GameContext : MVCSContext
{
    public GameContext(GameContextView view) : base(view)
    {
    }

    protected override void mapBindings()
    {
        base.mapBindings();

        var gameContextView = ((GameObject)contextView).GetComponent<GameContextView>();
        injectionBinder.Bind<LevelModel>().ToValue(new LevelModel(gameContextView.LevelConfigProvider.LevelConfig));
        injectionBinder.Bind<CellConfigProvider>().ToValue(gameContextView.CellConfigProvider);
        injectionBinder.Bind<UnitConfigsProvider>().ToValue(gameContextView.UnitConfigsProvider);
        injectionBinder.Bind<UnitModelByView>().ToSingleton();
        injectionBinder.Bind<ICellPositionConverter>().ToValue(gameContextView.GridView);

        mediationBinder.Bind<GameContextView>().To<GameContextViewMediator>();
        mediationBinder.Bind<GridView>().To<GridViewMediator>();
        mediationBinder.Bind<UnitView>().To<UnitViewMediator>();

        commandBinder.Bind(MediatorEvents.SECOND_PASSED).To<SecondPassedCommand>();
        commandBinder.Bind(MediatorEvents.DRAW_GRID_COMPLETE).To<StartLevelCommand>().Once();
        commandBinder.Bind(MediatorEvents.UNIT_SPAWNED).To<UpdateUnitStateCommand>();
        commandBinder.Bind(MediatorEvents.UNIT_MOVE_TO_NEXT_CELL_FINISHED).To<UpdateUnitStateCommand>();
    }
}

