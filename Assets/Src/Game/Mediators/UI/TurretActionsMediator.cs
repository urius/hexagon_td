using System;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using UnityEngine;

public class TurretActionsMediator
{
    [Inject(ContextKeys.CONTEXT_DISPATCHER)]
    public IEventDispatcher dispatcher { get; set; }

    [Inject] public ICellPositionConverter CellPositionConverter { get; set; }
    [Inject] public IScreenPanelViewProvider ScreenPanelViewProvider { get; set; }
    [Inject] public GUIPrefabsConfig GUIPrefabsConfig { get; set; }
    [Inject] public LocalizationProvider Loc { get; set; }
    [Inject] public TurretConfigProvider TurretConfigProvider { get; set; }

    private TurretActionsView _view;
    private TurretModel _turretModel;

    public TurretActionsMediator()
    {
    }

    public void Initialize(TurretModel turretModel)
    {
        _turretModel = turretModel;

        var camera = Camera.main;

        var screenPoint = camera.WorldToScreenPoint(CellPositionConverter.CellVec2ToWorld(turretModel.Position));
        var go = GameObject.Instantiate(GUIPrefabsConfig.TurretActionsPrefab, ScreenPanelViewProvider.ScreenPanelView.transform);
        var rectTransform = go.GetComponent<RectTransform>();
        _view = go.GetComponent<TurretActionsView>();
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(rectTransform, screenPoint, camera, out var worldPoint))
        {
            go.GetComponent<RectTransform>().position = worldPoint;
        }

        var rightBoundScreen = camera.WorldToScreenPoint(_view.RightBound.position);
        var leftBoundScreen = camera.WorldToScreenPoint(_view.LeftBound.position);

        if (rightBoundScreen.x > Screen.width)
        {
            _view.Animation.Play("show_right");
        }
        else if (leftBoundScreen.x < 0)
        {
            _view.Animation.Play("show_left");
        }
        else
        {
            _view.Animation.Play("show_default");
        }

        SetupTexts(turretModel.TurretConfig);

        Activate();

        AudioManager.Instance.Play(SoundId.ClickOnTurret);
    }

    private void SetupTexts(TurretConfig turretConfig)
    {
        var turretNameId = string.Empty;
        switch (turretConfig.TurretType)
        {
            case TurretType.Gun:
                turretNameId = "gun";
                break;
            case TurretType.Laser:
                turretNameId = "laser";
                break;
            case TurretType.Rocket:
                turretNameId = "rocket";
                break;
            case TurretType.SlowField:
                turretNameId = "slow";
                break;
        }
        var turretName = Loc.Get(LocalizationGroupId.TurretName, turretNameId);
        var levelStr = Loc.Get(LocalizationGroupId.TurretActions, "level");
        _view.SetTurretInfoTexts(turretName, $"{levelStr} {turretConfig.TurretLevelIndex + 1}");

        var sellStr = Loc.Get(LocalizationGroupId.TurretActions, "sell");
        _view.SetSellTexts(sellStr, $"+{(int)(turretConfig.Price * 0.5)}$");

        var nextLevelConfig = TurretConfigProvider.GetConfig(turretConfig.TurretType, turretConfig.TurretLevelIndex + 1);
        _view.SetUpgradeButtonEnabled(nextLevelConfig != null);
        if (nextLevelConfig != null)
        {
            var upgradeTxt = Loc.Get(LocalizationGroupId.TurretActions, "upgrade");
            _view.SetUpgradeTexts(upgradeTxt, $"{nextLevelConfig.Price - turretConfig.Price}$");
        }
    }

    private void Activate()
    {
        dispatcher.AddListener(MediatorEvents.UI_GAME_SCREEN_MOUSE_DOWN, OnGameScreenMouseDown);
        dispatcher.AddListener(MediatorEvents.UI_BUILD_TURRET_MOUSE_DOWN, OnBuildTurretMouseDown);
        _view.SellClicked += OnSellClicked;
        _view.UpgradeClicked += OnUpgradeClicked;
    }

    private void Deactivate()
    {
        dispatcher.RemoveListener(MediatorEvents.UI_GAME_SCREEN_MOUSE_DOWN, OnGameScreenMouseDown);
        dispatcher.RemoveListener(MediatorEvents.UI_BUILD_TURRET_MOUSE_DOWN, OnBuildTurretMouseDown);
        _view.SellClicked -= OnSellClicked;
        _view.UpgradeClicked -= OnUpgradeClicked;
    }

    private void OnSellClicked()
    {
        dispatcher.Dispatch(MediatorEvents.TURRET_SELL_CLICKED, _turretModel);
        DestroyView();
    }

    private void OnUpgradeClicked()
    {
        dispatcher.Dispatch(MediatorEvents.TURRET_UPGRADE_CLICKED, _turretModel);
        DestroyView(); //TODO: upgrate on the fly, without destroying
    }

    private void OnGameScreenMouseDown(IEvent payload)
    {
        DestroyView();
    }

    private void OnBuildTurretMouseDown(IEvent payload)
    {
        DestroyView();
    }

    private void DestroyView()
    {
        Deactivate();

        if (_view != null)
        {
            GameObject.Destroy(_view.gameObject);
            _view = null;

            dispatcher.Dispatch(MediatorEvents.TURRET_DESELECTED);
        }
    }
}
