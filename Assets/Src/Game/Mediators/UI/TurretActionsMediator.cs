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
    [Inject] public UIPrefabsConfig UIPrefabsConfig { get; set; }

    private TurretModel _turretModel;
    private TurretActionsView _view;

    public TurretActionsMediator()
    {
    }


    public void Initialize(TurretModel turretModel)
    {
        _turretModel = turretModel;
        var camera = Camera.main;

        var screenPoint = camera.WorldToScreenPoint(CellPositionConverter.CellVec2ToWorld(turretModel.Position));
        var go = GameObject.Instantiate(UIPrefabsConfig.TurretActionsPrefab, ScreenPanelViewProvider.ScreenPanelView.transform);
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

        OnRegister();
    }

    private void OnRegister()
    {
        dispatcher.AddListener(MediatorEvents.UI_GAME_SCREEN_MOUSE_DOWN, OnGameScreenMouseDown);
    }

    private void OnRemove()
    {
        dispatcher.RemoveListener(MediatorEvents.UI_GAME_SCREEN_MOUSE_DOWN, OnGameScreenMouseDown);
    }

    private void OnGameScreenMouseDown(IEvent payload)
    {
        DestroyView();
    }

    private void DestroyView()
    {
        if (_view != null)
        {
            GameObject.Destroy(_view.gameObject);
            _view = null;
        }

        OnRemove();
    }
}
