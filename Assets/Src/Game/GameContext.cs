using System;
using System.Threading.Tasks;
using strange.extensions.context.impl;
using UnityEngine;

public class GameContext : MVCSContext
{
    private TaskCompletionSource<bool> _initializedTsc = new TaskCompletionSource<bool>();

    public GameContext(GameContextView view) : base(view)
    {
    }

    public LevelModel LevelModel { get; private set; }
    public Task InitializedTask => _initializedTsc.Task;

    protected override void mapBindings()
    {
        base.mapBindings();

        var gameContextView = ((GameObject)contextView).GetComponent<GameContextView>();

        new GlobalBindingsHelper(injectionBinder, mediationBinder).Bind(gameContextView.GlobalObjectsHolder);

        var levelModel = new LevelModel(gameContextView.GlobalObjectsHolder.LevelConfigProvider.LevelConfig);
        LevelModel = levelModel;

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
        injectionBinder.Bind<GUIPrefabsConfig>().ToValue(gameContextView.GUIPrefabsConfig);

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
        mediationBinder.Bind<SettingsPopup>().To<SettingsPopupMediator>();
        mediationBinder.Bind<SettingsButtonView>().To<SettingsButtonMediator>();
        mediationBinder.Bind<TimeScaleButtonView>().To<TimeScaleButtonMediator>();
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
        commandBinder.Bind(MediatorEvents.UI_SETTINGS_POPUP_MAIN_MENU_CLICKED).To<MainMenuClickedCommand>().Pooled();        
        commandBinder.Bind(MediatorEvents.UI_WIN_POPUP_NEXT_LEVEL_CLICKED).To<NextLevelClickedCommand>().Pooled();
        commandBinder.Bind(MediatorEvents.UI_TIME_SCALE_CHANGE_CLICKED).To<TimeScaleChangeCommand>().Pooled();

        //controllers&systems
        injectionBinder.Bind<ProcessUpdatesSystem>().ToSingleton();
        injectionBinder.Bind<UnitsControlSystem>().ToSingleton();
        injectionBinder.Bind<BulletsHitSystem>().ToSingleton();
        injectionBinder.Bind<TurretsControlSystem>().ToSingleton();
        injectionBinder.Bind<WavesControlSystem>().ToSingleton();
        injectionBinder.Bind<PlayerDataUpdateSystem>().ToSingleton();        

        //debug
        commandBinder.Bind(MediatorEvents.DEBUG_BUTTON_CLICKED).To<DebugCommand>();
    }

    protected override void postBindings()
    {
        base.postBindings();

        _initializedTsc.TrySetResult(true);
    }
}
