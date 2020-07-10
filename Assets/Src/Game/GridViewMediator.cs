using System;
using strange.extensions.context.api;
using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class GridViewMediator : EventMediator
{
    [Inject] public GridView gridView { get; set; }
    [Inject] public LevelModel levelModel { get; set; }
    [Inject] public CellConfigProvider cellConfigProvider { get; set; }
    [Inject] public UnitModelByView modelByView { get; set; }

    public override void OnRegister()
    {
        levelModel.StartSpawnUnit += OnStartSpawnUnit;
    }

    public override void OnRemove()
    {
        levelModel.StartSpawnUnit -= OnStartSpawnUnit;
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
            var prefab = cellConfigProvider.GetConfig(cellDataMin.CellConfigMin.CellType).Prefab;
            gridView.DrawCell(cellDataMin.CellPosition, prefab);
        }
    }
}
