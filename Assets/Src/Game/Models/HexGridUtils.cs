using System;
using System.Collections.Generic;
using UnityEngine;

public class HexGridUtils
{
    private static readonly Vector2Int[][] _deltaCoords = new Vector2Int[][]
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

    public static IEnumerable<Vector2Int> GetPositionsNearCell(Vector2Int cellPosition)
    {
        foreach (var deltaCoord in _deltaCoords[Math.Abs(cellPosition.y % 2)])
        {
            yield return cellPosition + deltaCoord;
        }
    }
}
