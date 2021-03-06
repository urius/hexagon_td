﻿using System;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class GameScreenPanelMediator : EventMediator
{
    [Inject] public GameScreenPanelView ScreenPanelView { get; set; }
    [Inject] public ScreenPanelViewHolder ScreenPanelViewHolder { get; set; }
    [Inject] public ICellPositionConverter CellPositionConverter { get; set; }
    [Inject] public WorldMousePositionProvider WorldMousePositionProvider { get; set; }
    [Inject] public GUIPrefabsConfig GUIPrefabsConfig { get; set; }

    private Vector3 _screenMouseDownPos;
    private float _clickSensitivityPixelsSqr;

    public override void OnRegister()
    {
        base.OnRegister();

        _clickSensitivityPixelsSqr = (float)Math.Pow(Screen.width * 0.02f, 2);
        ScreenPanelViewHolder.SetViev(ScreenPanelView);

        ScreenPanelView.PointerDown += OnGameScreenPointerDown;
        ScreenPanelView.PointerUp += OnGameScreenPointerUp;
        dispatcher.AddListener(MediatorEvents.EARN_MONEY_ANIMATION, OnEarnMoneyAnimationRequest);
        dispatcher.AddListener(MediatorEvents.REPAIR_BASE_ANIMATION, OnRepairBaseAnimationRequest);
    }

    public override void OnRemove()
    {
        base.OnRemove();

        ScreenPanelView.PointerDown -= OnGameScreenPointerDown;
        ScreenPanelView.PointerUp -= OnGameScreenPointerUp;
        dispatcher.RemoveListener(MediatorEvents.EARN_MONEY_ANIMATION, OnEarnMoneyAnimationRequest);
        dispatcher.RemoveListener(MediatorEvents.REPAIR_BASE_ANIMATION, OnRepairBaseAnimationRequest);
    }

    private void OnGameScreenPointerDown()
    {
        dispatcher.Dispatch(MediatorEvents.UI_GAME_SCREEN_MOUSE_DOWN);

        _screenMouseDownPos = Input.mousePosition;
    }

    private void OnGameScreenPointerUp()
    {
        dispatcher.Dispatch(MediatorEvents.UI_GAME_SCREEN_MOUSE_UP);

        if ((_screenMouseDownPos - Input.mousePosition).sqrMagnitude < _clickSensitivityPixelsSqr)
        {
            var worldMousePosition = WorldMousePositionProvider.Position;
            var cellMousePosition = CellPositionConverter.CellVec3ToVec2(CellPositionConverter.WorldToCell(worldMousePosition));

            dispatcher.Dispatch(MediatorEvents.UI_GAME_SCREEN_CLICK, cellMousePosition);
        }
    }

    private void OnEarnMoneyAnimationRequest(IEvent payload)
    {
        var data = payload.data as MediatorEventsParams.EarnMoneyAnimationParams;
        ShowFlyingText(data.WorldPosition, $"+{data.MoneyAmount}$", Color.green);
    }

    private void OnRepairBaseAnimationRequest(IEvent payload)
    {
        var data = payload.data as MediatorEventsParams.RepairBaseAnimationParams;
        foreach(var worldPosition in data.WorldPositions)
        {
            ShowFlyingText(worldPosition, $"+{data.PointsAmount}", Color.cyan);
        }
    }

    private void ShowFlyingText(Vector3 worldPosition, string text, Color color)
    {
        var flyingTextGo = Instantiate(GUIPrefabsConfig.FlyingTextPrefab, ScreenPanelView.transform);
        var flyingText = flyingTextGo.GetComponent<FlyingTextView>();

        flyingText.SetText(text);
        flyingText.SetColor(color);
        var rectTransform = flyingTextGo.GetComponent<RectTransform>();
        var viewportPoint = Camera.main.WorldToViewportPoint(worldPosition);
        rectTransform.anchorMin = viewportPoint;
        rectTransform.anchorMax = viewportPoint;
    }
}
