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
        injectionBinder.Bind<LevelTurretsModel>().ToValue(levelModel.LevelTurretsModel);
        injectionBinder.Bind<CellConfigProvider>().ToValue(gameContextView.CellConfigProvider);
        injectionBinder.Bind<UnitConfigsProvider>().ToValue(gameContextView.UnitConfigsProvider);
        injectionBinder.Bind<TurretConfigProvider>().ToValue(gameContextView.TurretConfigsProvider);
        injectionBinder.Bind<UIPrefabsConfig>().ToValue(gameContextView.UIPrefabsConfig);
        injectionBinder.Bind<LocalizationProvider>().ToValue(gameContextView.LocalizationProvider);
        injectionBinder
            .Bind<IUnitModelByViewProvider>()
            .Bind<IUnitViewsProvider>()
            .Bind<ModelByViewHolder>()
            .To<ModelByViewHolder>()
            .ToSingleton();
        injectionBinder
            .Bind<IGridViewProvider>()
            .Bind<ICellSizeProvider>()
            .Bind<GridViewProvider>()
            .To<GridViewProvider>()
            .ToSingleton();
        injectionBinder
            .Bind<IScreenPanelViewProvider>()
            .Bind<ScreenPanelViewHolder>()
            .To<ScreenPanelViewHolder>()
            .ToSingleton();
        injectionBinder.Bind<WorldMousePositionProvider>().ToSingleton();
        injectionBinder.Bind<FlyingTurretConfigProvider>().ToSingleton();        
        injectionBinder.Bind<ICellPositionConverter>().ToValue(gameContextView.GridView);

        //mediators
        mediationBinder.Bind<GridView>().To<GridViewMediator>();
        mediationBinder.Bind<UnitView>().To<UnitViewMediator>();
        mediationBinder.Bind<GameCameraView>().To<CameraViewMediator>();
        mediationBinder.Bind<BuildTurretView>().To<BuildTurretViewMediator>();
        injectionBinder.Bind<GunTurretMediator>().To<GunTurretMediator>();//custom
        injectionBinder.Bind<RocketTurretMediator>().To<RocketTurretMediator>();//custom
        injectionBinder.Bind<LaserTurretMediator>().To<LaserTurretMediator>();//custom
        injectionBinder.Bind<TurretActionsMediator>().To<TurretActionsMediator>();//custom
        injectionBinder.Bind<ShowPathsMediator>().To<ShowPathsMediator>();//custom
        mediationBinder.Bind<GameScreenPanelView>().To<GameScreenPanelMediator>();
        mediationBinder.Bind<BuildTurretButtonView>().To<BuildTurretButtonMediator>();
        //debug ui
        mediationBinder.Bind<DebugPanelView>().To<DebugPanelMediator>();

        //commands
        commandBinder.Bind(MediatorEvents.DRAW_GRID_COMPLETE)
            .InSequence()
            .To<StartLevelCommand>()
            .Once();
        commandBinder.Bind(CommandEvents.SECOND_PASSED).To<SecondPassedCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.REQUEST_BUILD_TURRET).To<RequestBuildTurretCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.UI_GAME_SCREEN_CLICK).To<GameScreenClickedCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.TURRET_DETECTED_UNIT_IN_ATTACK_ZONE).To<ChooseTurretTargetCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.TURRET_TARGET_LOCKED).To<TurretLockTargetCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.TURRET_TARGET_LEAVE_ATTACK_ZONE).To<TurretTargetLeaveCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.TURRET_SELL_CLICKED).To<TurretSellClickedCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.TURRET_UPGRADE_CLICKED).To<TurretUpgradeClickedCommand>().Pooled();

        //controllers&systems
        injectionBinder.Bind<ProcessUpdatesSystem>().ToSingleton();
        injectionBinder.Bind<UnitsControlSystem>().ToSingleton();
        injectionBinder.Bind<BulletsHitSystem>().ToSingleton();        

        //debug
        commandBinder.Bind(MediatorEvents.DEBUG_BUTTON_CLICKED).To<DebugCommand>();
    }

    protected override void postBindings()
    {
        base.postBindings();
    }
}
