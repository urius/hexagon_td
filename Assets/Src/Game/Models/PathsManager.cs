using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class PathsManager : ICellsProvider<Vector2Int>
{
    public event Action PathsUpdated = delegate { };

    private readonly Dictionary<Vector2Int, CellType> _walkableCellsByCoord = new Dictionary<Vector2Int, CellType>();
    private readonly Dictionary<Vector2Int, CellSubType> _teleportCells = new Dictionary<Vector2Int, CellSubType>();
    private readonly Dictionary<Vector2Int, bool> _tempUnwalkableCells = new Dictionary<Vector2Int, bool>();
    private readonly LevelTurretsModel _turretsModel;
    private readonly LevelUnitsModel _unitsModel;
    private readonly Dictionary<(Vector2Int, Vector2Int), IReadOnlyList<Vector2Int>> _cachedPaths = new Dictionary<(Vector2Int, Vector2Int), IReadOnlyList<Vector2Int>>();
    private readonly Dictionary<Vector2Int, ModifierType> _modifiers;

    public PathsManager(
        IReadOnlyList<CellDataMin> cells,
        Dictionary<Vector2Int, ModifierType> modifiers,
        LevelTurretsModel turretsModel,
        LevelUnitsModel unitsModel)
    {
        FillWalkableCells(cells);

        _modifiers = modifiers;
        _turretsModel = turretsModel;
        _unitsModel = unitsModel;

        _turretsModel.TurretAdded += OnTurretAdded;
        _turretsModel.TurretRemoved += OnTurretRemoved;
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

    public (Vector2Int, Vector2Int)[] GetPathsContainsCell(Vector2Int cellPosition)
    {
        var result = new List<(Vector2Int, Vector2Int)>();

        foreach (var kvp in _cachedPaths)
        {
            if (kvp.Value.Contains(cellPosition))
            {
                result.Add(kvp.Key);
            }
        }

        return result.ToArray();
    }

    public int GetCellMoveCost(Vector2Int cell)
    {
        if (_modifiers.TryGetValue(cell, out var modifier))
        {
            if (modifier == ModifierType.SpeedUp)
            {
                return 50;
            }
            else if (modifier == ModifierType.SpeedDown)
            {
                return 200;
            }
        }
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

    private void OnTurretRemoved(TurretModel turret)
    {
        //clear all pathes and recalculate pathes for current units
        ClearAllPaths();
        foreach (var unit in _unitsModel.Units)
        {
            if (!unit.IsOnLastCell && !unit.IsOnPreLastCell)
            {
                var (currentCell, finishCell) = unit.GetRestPathEdgePoints();
                unit.SubstitutePath(Pathfinder.FindPath(this, currentCell, finishCell));
            }
        }

        PathsUpdated();
    }

    private void OnTurretAdded(TurretModel turret)
    {
        //Clear pathes, that affects by new turret, so they wil be recalculated next time on demand
        var patchsToRefresh = GetPathsContainsCell(turret.Position);
        foreach (var pathEdgePoints in patchsToRefresh)
        {
            _cachedPaths.Remove(pathEdgePoints);

            PathsUpdated();
        }

        //Update pathes for units affected by new turret
        foreach (var unit in _unitsModel.Units)
        {
            if (unit.IsRestPathContainsCell(turret.Position))
            {
                var (currentCell, finishCell) = unit.GetRestPathEdgePoints();
                unit.SubstitutePath(Pathfinder.FindPath(this, currentCell, finishCell));
            }
        }
    }

    private void ClearAllPaths()
    {
        _cachedPaths.Clear();
    }
}
