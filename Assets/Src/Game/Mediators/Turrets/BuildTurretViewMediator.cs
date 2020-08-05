using strange.extensions.mediation.impl;
using UnityEngine;

public class BuildTurretViewMediator : EventMediator
{
    [Inject] public BuildTurretView BuildTurretView { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public ICellPositionConverter cellPositionConverter { get; set; }
    [Inject] public WorldMousePositionProvider WorldMousePositionProvider { get; set; }

    private Vector3Int _lastCellCoords;

    public override void OnRegister()
    {
        base.OnRegister();

        var cellCoords = cellPositionConverter.WorldToCell(WorldMousePositionProvider.Position);
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

    private void Update()
    {
        var cellCoords = cellPositionConverter.WorldToCell(WorldMousePositionProvider.Position);

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
