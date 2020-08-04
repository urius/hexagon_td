using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathsManager : ICellsProvider<Vector2Int>
{
    private readonly Dictionary<Vector2Int, CellType> _walkableCellsByCoord = new Dictionary<Vector2Int, CellType>();
    private readonly Dictionary<Vector2Int, CellSubType> _teleportCells = new Dictionary<Vector2Int, CellSubType>();
    private readonly Dictionary<Vector2Int, bool> _tempUnwalkableCells = new Dictionary<Vector2Int, bool>();
    private readonly LevelTurretsModel _turretsModel;
    private readonly LevelUnitsModel _unitsModel;
    private readonly Dictionary<(Vector2Int, Vector2Int), IReadOnlyList<Vector2Int>> _cachedPaths = new Dictionary<(Vector2Int, Vector2Int), IReadOnlyList<Vector2Int>>();

    public PathsManager(LevelConfig levelConfig, LevelTurretsModel turretsModel, LevelUnitsModel unitsModel)
    {
        FillWalkableCells(levelConfig.Cells);

        _turretsModel = turretsModel;
        _unitsModel = unitsModel;
    }

    public IReadOnlyList<Vector2Int> GetPath(Vector2Int startCell, Vector2Int finishCell)
    {
        if (_cachedPaths.TryGetValue((startCell, finishCell), out var path))
        {
            return path;
        }
        else
        {
            path = Pathfinder.FindPath(this, startCell, finishCell);
            _cachedPaths[(startCell, finishCell)] = path;
            return path;
        }
    }

    public bool IsPathExists(Vector2Int startCell, Vector2Int finishCell, Vector2Int excludeCell)
    {
        SetCellTempWalkable(excludeCell, false);
        var path = Pathfinder.FindPath(this, startCell, finishCell);
        SetCellTempWalkable(excludeCell, true);
        return (path.Count() != 0);
    }

    public void SetCellTempWalkable(Vector2Int cell, bool isWalkable)
    {
        if (isWalkable)
        {
            _tempUnwalkableCells.Remove(cell);
        }
        else
        {
            _tempUnwalkableCells[cell] = isWalkable;
        }
    }

    public bool ContainsInCachedPaths(Vector2Int cellPosition)
    {
        foreach (var path in _cachedPaths.Values)
        {
            if (path.Contains(cellPosition))
            {
                return true;
            }
        }

        return false;
    }

    public int GetCellMoveCost(Vector2Int cell)
    {
        return 100;
    }

    public IEnumerable<Vector2Int> GetWalkableNearCells(Vector2Int cellPosition)
    {
        var currentCellType = _walkableCellsByCoord[cellPosition];
        var result = new List<Vector2Int>();

        var nearCells = HexGridUtils.GetPositionsNearCell(cellPosition)
            .Where(IsOkToWalk);

        if (currentCellType == CellType.Teleport)
        {
            var currentTeleportType = _teleportCells[cellPosition];
            foreach (var kvp in _teleportCells)
            {
                if (currentTeleportType == kvp.Value
                    && kvp.Key != cellPosition)
                {
                    result.Add(kvp.Key);
                }
            }
        }
        result.AddRange(nearCells);

        return result;
    }

    private bool IsOkToWalk(Vector2Int cell)
    {
        return _walkableCellsByCoord.TryGetValue(cell, out var _)
            && !_turretsModel.HaveTurret(cell)
            //&& _unitsModel.IsCellWithoutUnit(cell)
            && !_tempUnwalkableCells.TryGetValue(cell, out _);
    }

    public bool IsCellEquals(Vector2Int cellA, Vector2Int cellB)
    {
        return cellA == cellB;
    }

    private void FillWalkableCells(IReadOnlyList<CellDataMin> cells)
    {
        foreach (var cell in cells)
        {
            if (cell.CellConfigMin.CellType != CellType.Wall)
            {
                _walkableCellsByCoord[cell.CellPosition] = cell.CellConfigMin.CellType;
                if (cell.CellConfigMin.CellType == CellType.Teleport)
                {
                    _teleportCells[cell.CellPosition] = cell.CellConfigMin.CellSubType;
                }
            }
        }
    }
}
