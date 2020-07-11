using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelPathfinderCellsProvider : ICellsProvider<Vector2Int>
{
    private readonly Vector2Int[][] _deltaCoords = new Vector2Int[][]
    {
        new Vector2Int[]
        {
            new Vector2Int(1 ,0),
            new Vector2Int(-1 ,0),
            new Vector2Int(0 ,1),
            new Vector2Int(0 ,-1),
            new Vector2Int(-1, 1),
            new Vector2Int(-1, -1),
        },
        new Vector2Int[]
        {
            new Vector2Int(1 ,0),
            new Vector2Int(-1 ,0),
            new Vector2Int(0 ,1),
            new Vector2Int(0 ,-1),
            new Vector2Int(1, 1),
            new Vector2Int(1, -1),
        }
    };
    private readonly Dictionary<Vector2Int, CellDataMin> _walkableCellsByCoord = new Dictionary<Vector2Int, CellDataMin>();

    public LevelPathfinderCellsProvider(LevelConfig levelConfig)
    {
        FillWalkableCells(levelConfig.Cells);
    }

    private void FillWalkableCells(IReadOnlyList<CellDataMin> cells)
    {
        foreach (var cell in cells)
        {
            if (cell.CellConfigMin.CellType != CellType.Wall)
            {
                _walkableCellsByCoord[cell.CellPosition] = cell;
            }
        }
    }

    public int GetCellMoveCost(Vector2Int cell)
    {
        return 100;
    }

    public IEnumerable<Vector2Int> GetNearCells(Vector2Int cellPosition)
    {
        var currentCell = _walkableCellsByCoord[cellPosition];
        var result = new List<Vector2Int>();

        if (currentCell.CellConfigMin.CellType == CellType.Teleport)
        {
            var teleportType = currentCell.CellConfigMin.CellSubType;
            foreach (var cell in _walkableCellsByCoord.Values)
            {
                if (cell.CellConfigMin.CellType == CellType.Teleport
                    && cell.CellConfigMin.CellSubType == teleportType
                    && cell != currentCell)
                {
                    result.Add(cell.CellPosition);
                }
            }
        }
        result.AddRange(GetPositionsNearCell(currentCell.CellPosition));

        return result;
    }

    private IEnumerable<Vector2Int> GetPositionsNearCell(Vector2Int cellPosition)
    {
        foreach (var deltaCoord in _deltaCoords[Math.Abs(cellPosition.y % 2)])
        {
            if (_walkableCellsByCoord.TryGetValue(cellPosition + deltaCoord, out var cell))
            {
                yield return cell.CellPosition;
            }
        }
    }

    public bool IsCellEquals(Vector2Int cellA, Vector2Int cellB)
    {
        return cellA == cellB;
    }
}
