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

        new GlobalBindingsHelper(injectionBinder).Bind(gameContextView.GlobalObjectsHolder);

        var levelModel = new LevelModel(gameContextView.GlobalObjectsHolder.LevelConfigProvider.LevelConfig);

        injectionBinder
            .Bind<IUpdateProvider>()
            .Bind<IRootTransformProvider>()
            .ToValue(gameContextView);
        injectionBinder.Bind<IViewManager>().To<ViewsManager>().ToSingleton();
        injectionBinder.Bind<LevelModel>().ToValue(levelModel);
        injectionBinder.Bind<WaveModel>().ToValue(levelModel.WaveModel);
        injectionBinder.Bind<LevelUnitsModel>().ToValue(levelModel.LevelUnitsModel);
        injectionBinder.Bind<LevelTurretsModel>().ToValue(levelModel.LevelTurretsModel);
        injectionBinder.Bind<CellConfigProvider>().ToValue(gameContextView.CellConfigProvider);
        injectionBinder.Bind<UnitConfigsProvider>().ToValue(gameContextView.UnitConfigsProvider);
        injectionBinder.Bind<TurretConfigProvider>().ToValue(gameContextView.TurretConfigsProvider);

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
        injectionBinder.Bind<SlowFieldTurretMediator>().To<SlowFieldTurretMediator>();//custom
        injectionBinder.Bind<TurretActionsMediator>().To<TurretActionsMediator>();//custom
        mediationBinder.Bind<ShowPathsView>().To<ShowPathsMediator>();
        mediationBinder.Bind<UICanvasView>().To<UiCanvasViewMediator>();
        mediationBinder.Bind<GameScreenPanelView>().To<GameScreenPanelMediator>();
        mediationBinder.Bind<BuildTurretButtonView>().To<BuildTurretButtonMediator>();
        mediationBinder.Bind<StartWaveButtonView>().To<StartWaveButtonViewMediator>();
        mediationBinder.Bind<WaveTextView>().To<WaveTextViewMediator>();
        mediationBinder.Bind<MoneyTextView>().To<MoneyTextViewMediator>();
        mediationBinder.Bind<ButtonView>().To<ButtonViewMediator>();
        mediationBinder.Bind<WinPopup>().To<WinPopupMediator>();
        mediationBinder.Bind<LosePopup>().To<LosePopupMediator>();
        //debug ui
        mediationBinder.Bind<DebugPanelView>().To<DebugPanelMediator>();

        //commands
        commandBinder.Bind(MediatorEvents.DRAW_GRID_COMPLETE)
            .InSequence()
            .To<StartLevelCommand>()
            .Once();
        commandBinder.Bind(MediatorEvents.REQUEST_BUILD_TURRET).To<RequestBuildTurretCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.UI_GAME_SCREEN_CLICK).To<GameScreenClickedCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.UI_WIN_POPUP_MAIN_MENU_CLICKED).To<MainMenuClickedCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.UI_LOSE_POPUP_MAIN_MENU_CLICKED).To<MainMenuClickedCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.UI_WIN_POPUP_NEXT_LEVEL_CLICKED).To<NextLevelClickedCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.UI_LOSE_POPUP_RESTART_LEVEL_CLICKED).To<RestartClickedCommand>().Pooled();

        //controllers&systems
        injectionBinder.Bind<ProcessUpdatesSystem>().ToSingleton();
        injectionBinder.Bind<UnitsControlSystem>().ToSingleton();
        injectionBinder.Bind<BulletsHitSystem>().ToSingleton();
        injectionBinder.Bind<TurretsControlSystem>().ToSingleton();
        injectionBinder.Bind<WavesControlSystem>().ToSingleton();

        //debug
        commandBinder.Bind(MediatorEvents.DEBUG_BUTTON_CLICKED).To<DebugCommand>();
    }

    protected override void postBindings()
    {
        base.postBindings();
    }
}
