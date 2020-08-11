using System;
using strange.extensions.context.impl;
using UnityEngine;

public class GameContext : MVCSContext
{
    public GameContext(GameContextView view) : base(view)
    {
    }

    protected override void mapBindings()
    {
        base.mapBindings();

        var gameContextView = ((GameObject)contextView).GetComponent<GameContextView>();
        injectionBinder.Bind<IUpdateProvider>().ToValue(gameContextView);
        injectionBinder.Bind<IViewManager>().To<ViewsManager>().ToSingleton();
        var levelModel = new LevelModel(gameContextView.LevelConfigProvider.LevelConfig);
        injectionBinder.Bind<LevelModel>().ToValue(levelModel);
        injectionBinder.Bind<LevelUnitsModel>().ToValue(levelModel.LevelUnitsModel);
        injectionBinder.Bind<CellConfigProvider>().ToValue(gameContextView.CellConfigProvider);
        injectionBinder.Bind<UnitConfigsProvider>().ToValue(gameContextView.UnitConfigsProvider);
        injectionBinder.Bind<TurretConfigProvider>().ToValue(gameContextView.TurretConfigsProvider);
        injectionBinder
            .Bind<IUnitModelByViewProvider>()
            .Bind<IUnitViewsProvider>()
            .Bind<ITurretModelByViewProvider>()
            .Bind<ModelByViewHolder>()
            .To<ModelByViewHolder>()
            .ToSingleton();
        injectionBinder.Bind<GridViewProvider>().ToSingleton();
        injectionBinder.Bind<WorldMousePositionProvider>().ToSingleton();
        injectionBinder.Bind<ICellPositionConverter>().ToValue(gameContextView.GridView);

        //mediators
        mediationBinder.Bind<GridView>().To<GridViewMediator>();
        mediationBinder.Bind<UnitView>().To<UnitViewMediator>();
        mediationBinder.Bind<GameCameraView>().To<CameraViewMediator>();
        mediationBinder.Bind<BuildTurretView>().To<BuildTurretViewMediator>();
        injectionBinder.Bind<GunTurretMediator>().To<GunTurretMediator>();//custom
        injectionBinder.Bind<LaserTurretMediator>().To<LaserTurretMediator>();//custom
        mediationBinder.Bind<GameScreenPanelView>().To<GameScreenPanelMediator>();
        mediationBinder.Bind<BuildTurretButtonView>().To<BuildTurretButtonMediator>();
        //debug ui
        mediationBinder.Bind<DebugPanelView>().To<DebugPanelMediator>();

        //commands
        commandBinder.Bind(MediatorEvents.DRAW_GRID_COMPLETE)
            .InSequence()
            .To<StartLevelCommand>()
            //.To<ProcessUpdatesCommand>()
            .Once();
        commandBinder.Bind(CommandEvents.SECOND_PASSED).To<SecondPassedCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.REQUEST_BUILD_TURRET).To<RequestBuildTurretCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.TURRET_DETECTED_UNIT_IN_ATTACK_ZONE).To<ChooseTurretTargetCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.TURRET_TARGET_LOCKED).To<TurretLockTargetCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.TURRET_TARGET_LEAVE_ATTACK_ZONE).To<TurretTargetLeaveCommand>().Pooled();

        //controllers&systems
        injectionBinder.Bind<ProcessUpdatesSystem>().ToSingleton();
        injectionBinder.Bind<UnitsControlSystem>().ToSingleton();

        //debug
        commandBinder.Bind(MediatorEvents.DEBUG_BUTTON_CLICKED).To<DebugCommand>();
    }

    protected override void postBindings()
    {
        base.postBindings();
    }
}
