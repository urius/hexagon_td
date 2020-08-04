using strange.extensions.dispatcher.eventdispatcher.api;
using strange.extensions.mediation.impl;
using UnityEngine;

public class BuildTurretViewMediator : EventMediator
{
    [Inject] public BuildTurretView BuildTurretView { get; set; }
    [Inject] public LevelModel LevelModel { get; set; }
    [Inject] public ICellPositionConverter cellPositionConverter { get; set; }
    [Inject] public WorldMousePositionProvider WorldMousePositionProvider { get; set; }

    private Vector3 _lastCellCoords;

    public override void OnRegister()
    {
        base.OnRegister();

        var cellCoords = cellPositionConverter.WorldToCell(WorldMousePositionProvider.Position);
        BuildTurretView.transform.position = cellPositionConverter.CellToWorld(cellCoords);
        _lastCellCoords = cellCoords;
    }

    public override void OnRemove()
    {
        base.OnRemove();
    }

    private void Update()
    {
        var cellCoords = cellPositionConverter.WorldToCell(WorldMousePositionProvider.Position);

        if (cellCoords != _lastCellCoords)
        {
            _lastCellCoords = cellCoords;
            BuildTurretView.transform.position = cellPositionConverter.CellToWorld(cellCoords);

            var isOkToBuild = LevelModel.IsReadyToBuild(new Vector2Int(cellCoords.x, cellCoords.y));
            BuildTurretView.SetOkToBuild(isOkToBuild);
        }
    }
}
