using System;
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
    [Inject] public UnitModelByView modelByView { get; set; }
    [Inject] public GridViewProvider gridViewProvider { get; set; }
    [Inject] public TurretConfigProvider turretsConfigProvider { get; set; }
    [Inject] public WorldMousePositionProvider worldMousePositionProvider { get; set; }

    public override void OnRegister()
    {
        levelModel.StartSpawnUnit += OnStartSpawnUnit;
        dispatcher.AddListener(MediatorEvents.UI_BUILD_TURRET_MOUSE_DOWN, OnBuildTurretMouseDown);
        dispatcher.AddListener(MediatorEvents.UI_BUILD_TURRET_MOUSE_UP, OnBuildTurretMouseUp);

        gridViewProvider.SetGridView(gridView);
    }

    public override void OnRemove()
    {
        levelModel.StartSpawnUnit -= OnStartSpawnUnit;
        dispatcher.RemoveListener(MediatorEvents.UI_BUILD_TURRET_MOUSE_DOWN, OnBuildTurretMouseDown);
        dispatcher.RemoveListener(MediatorEvents.UI_BUILD_TURRET_MOUSE_UP, OnBuildTurretMouseUp);
    }

    private void Update()
    {
        if (_flyingTurret != null)
        {
            _flyingTurret.transform.position = gridView.CellToWorld(gridView.WorldToCell(worldMousePositionProvider.Position));
        }
    }

    private void OnBuildTurretMouseDown(IEvent payload)
    {
        var turretType = (TurretType)payload.data;
        _flyingTurret = Instantiate(turretsConfigProvider.GetConfig(turretType, 0).BuildModePrefab);
    }

    private void OnBuildTurretMouseUp(IEvent payload)
    {
        if (_flyingTurret != null)
        {
            Destroy(_flyingTurret);
            _flyingTurret = null;
        }
    }

    private async void OnStartSpawnUnit(UnitModel unitModel)
    {
        var baseCellView = gridView.GetSpawnCellView(unitModel.CurrentCellPosition);
        GameObject unitGo = null;
        void PLatformBottomPointReached()
        {
            unitGo = Instantiate(unitModel.Prefab, baseCellView.SpawnPoint);
            var lookVector = gridView.CellVec2ToWorld(unitModel.NextCellPosition) - gridView.CellVec2ToWorld(unitModel.CurrentCellPosition);
            unitGo.transform.rotation = Quaternion.LookRotation(lookVector, Vector3.up);

            modelByView.ModelByView[unitGo.GetComponent<UnitView>()] = unitModel;
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
            var prefab = cellConfigProvider.GetConfig(cellDataMin.CellConfigMin.CellType, cellDataMin.CellConfigMin.CellSubType).Prefab;
            gridView.DrawCell(cellDataMin.CellPosition, prefab);
        }
    }
}
