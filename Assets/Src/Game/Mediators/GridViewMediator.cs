﻿using System;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class GridViewMediator : EventMediator
{
    private GameObject _flyingTurret;

    [Inject] public GridView gridView { get; set; }
    [Inject] public LevelModel levelModel { get; set; }
    [Inject] public CellConfigProvider cellConfigProvider { get; set; }
    [Inject] public ModelByViewHolder modelByView { get; set; }
    [Inject] public ICellPositionConverter cellPositionConverter { get; set; }
    [Inject] public GridViewProvider gridViewProvider { get; set; }
    [Inject] public ICellSizeProvider cellSizeProvider { get; set; }
    [Inject] public TurretConfigProvider turretsConfigProvider { get; set; }
    [Inject] public WorldMousePositionProvider worldMousePositionProvider { get; set; }
    [Inject] public FlyingTurretConfigProvider FlyingTurretConfigProvider { get; set; }

    public override void OnRegister()
    {
        dispatcher.AddListener(CommandEvents.START_SPAWN_UNIT, OnStartSpawnUnit);
        dispatcher.AddListener(CommandEvents.UNIT_DESTROYING, OnUnitDestroyStarted);
        dispatcher.AddListener(MediatorEvents.UI_BUILD_TURRET_MOUSE_DOWN, OnBuildTurretMouseDown);
        dispatcher.AddListener(MediatorEvents.UI_BUILD_TURRET_MOUSE_UP, OnBuildTurretMouseUp);
        dispatcher.AddListener(MediatorEvents.UNIT_DESTROY_ANIMATION_FINISHED, OnUnitDestroyAnimationFinished);
        levelModel.Teleporting += OnTeleporting;

        gridViewProvider.SetGridView(gridView);
    }

    public override void OnRemove()
    {
        dispatcher.RemoveListener(CommandEvents.START_SPAWN_UNIT, OnStartSpawnUnit);
        dispatcher.RemoveListener(CommandEvents.UNIT_DESTROYING, OnUnitDestroyStarted);
        dispatcher.RemoveListener(MediatorEvents.UI_BUILD_TURRET_MOUSE_DOWN, OnBuildTurretMouseDown);
        dispatcher.RemoveListener(MediatorEvents.UI_BUILD_TURRET_MOUSE_UP, OnBuildTurretMouseUp);
        dispatcher.RemoveListener(MediatorEvents.UNIT_DESTROY_ANIMATION_FINISHED, OnUnitDestroyAnimationFinished);
        levelModel.Teleporting -= OnTeleporting;
    }

    private void OnTeleporting(Vector2Int from, Vector2Int to)
    {
        gridView.GetTeleportCellView(from).PlayTeleportOutAnimation();
        gridView.GetTeleportCellView(to).PlayTeleportInAnimation();
    }

    private void OnUnitDestroyStarted(IEvent payload)
    {
        var unitModel = (UnitModel)payload.data;
        modelByView.Remove(unitModel);
    }

    private void OnUnitDestroyAnimationFinished(IEvent payload)
    {
        var unitView = (UnitView)payload.data;
        Destroy(unitView.gameObject);
    }

    private void OnBuildTurretMouseDown(IEvent payload)
    {
        if (_flyingTurret != null)
        {
            Destroy(_flyingTurret);
        }
        var turretType = (TurretType)payload.data;
        var turretConfig = turretsConfigProvider.GetConfig(turretType, 0);
        FlyingTurretConfigProvider.SetConfig(turretConfig);
        _flyingTurret = Instantiate(turretConfig.BuildModePrefab);
    }

    private void OnBuildTurretMouseUp(IEvent payload)
    {
        if (_flyingTurret != null)
        {
            Destroy(_flyingTurret);
            _flyingTurret = null;

            var offset = new Vector3(0, 0, cellSizeProvider.CellSize.y);
            dispatcher.Dispatch(MediatorEvents.REQUEST_BUILD_TURRET,
                new MediatorEventsParams.RequestBuildParams((TurretType)payload.data, gridView.WorldToCell(worldMousePositionProvider.Position + offset)));
        }
    }

    private async void OnStartSpawnUnit(IEvent payload)
    {
        var unitModel = (UnitModel)payload.data;
        var baseCellView = gridView.GetSpawnCellView(unitModel.CurrentCellPosition);
        GameObject unitGo = null;
        void PLatformBottomPointReached()
        {
            unitGo = Instantiate(unitModel.Prefab, baseCellView.SpawnPoint);
            var lookVector = gridView.CellVec2ToWorld(unitModel.NextCellPosition) - gridView.CellVec2ToWorld(unitModel.CurrentCellPosition);
            unitGo.transform.rotation = Quaternion.LookRotation(lookVector, Vector3.up);
            var unitView = unitGo.GetComponent<UnitView>();
            modelByView.Add(unitView, unitModel);
        }

        baseCellView.PLatformBottomPointReached += PLatformBottomPointReached;
        await baseCellView.PlaySpawnAnimationAsync();
        baseCellView.PLatformBottomPointReached -= PLatformBottomPointReached;

        unitGo.transform.SetParent(gridView.transform, true);

        dispatcher.Dispatch(MediatorEvents.UNIT_SPAWNED, unitModel);
    }

    private void Start()
    {
        DrawGrid();

        dispatcher.Dispatch(MediatorEvents.DRAW_GRID_COMPLETE);
    }

    private void DrawGrid()
    {
        foreach (var cellDataMin in levelModel.Cells)
        {
            var config = cellConfigProvider.GetConfig(cellDataMin.CellConfigMin.CellType, cellDataMin.CellConfigMin.CellSubType);
            var cellType = config.CellConfigMin.CellType;
            gridView.DrawCell(cellDataMin.CellPosition, config.Prefab, cellType == CellType.Ground || cellType == CellType.Wall);
        }

        foreach (var cellDataMin in levelModel.Modifiers)
        {
            var config = cellConfigProvider.GetConfig(cellDataMin.CellConfigMin.CellType, cellDataMin.CellConfigMin.CellSubType);
            var cellType = config.CellConfigMin.CellType;
            gridView.DrawModifier(cellDataMin.CellPosition, config.Prefab);
        }

        gridView.BakeStatic();
    }
}
