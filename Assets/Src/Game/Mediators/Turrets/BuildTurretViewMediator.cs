using System;
using strange.extensions.mediation.impl;
using UnityEngine;

public class BuildTurretViewMediator : EventMediator
{
    [Inject] public BuildTurretView BuildTurretView { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public ICellPositionConverter cellPositionConverter { get; set; }
    [Inject] public WorldMousePositionProvider WorldMousePositionProvider { get; set; }
    [Inject] public ICellSizeProvider CellSizeProvider { get; set; }

    private Vector3Int _lastCellCoords;

    public override void OnRegister()
    {
        base.OnRegister();

        var cellCoords = GetCellCoords();
        BuildTurretView.transform.position = cellPositionConverter.CellToWorld(cellCoords);
        _lastCellCoords = cellCoords;

        LevelModel.LevelUnitsModel.CellOwned += OnCellOwningStatusUpdated;
        LevelModel.LevelUnitsModel.CellReleased += OnCellOwningStatusUpdated;
    }

    public override void OnRemove()
    {
        LevelModel.LevelUnitsModel.CellOwned -= OnCellOwningStatusUpdated;
        LevelModel.LevelUnitsModel.CellReleased -= OnCellOwningStatusUpdated;

        base.OnRemove();
    }

    private Vector3Int GetCellCoords()
    {
        var offset = new Vector3(0, 0, CellSizeProvider.CellSize.y);
        return cellPositionConverter.WorldToCell(WorldMousePositionProvider.Position + offset);
    }

    private void Update()
    {
        var cellCoords = GetCellCoords();

        if (cellCoords != _lastCellCoords)
        {
            _lastCellCoords = cellCoords;
            BuildTurretView.transform.position = cellPositionConverter.CellToWorld(cellCoords);

            CheckReadyToBuild();
        }
    }

    private void OnCellOwningStatusUpdated(Vector2Int cell)
    {
        if (cell.x == _lastCellCoords.x && cell.y == _lastCellCoords.y)
        {
            CheckReadyToBuild();
        }
    }

    private void CheckReadyToBuild()
    {
        var isOkToBuild = LevelModel.IsReadyToBuild(new Vector2Int(_lastCellCoords.x, _lastCellCoords.y));
        BuildTurretView.SetOkToBuild(isOkToBuild);
    }
}
