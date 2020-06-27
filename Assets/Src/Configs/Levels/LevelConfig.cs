using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Level", fileName = "LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] public bool IsTransposed;

    [SerializeField] private CellDataMin[] _cellConfigs = new CellDataMin[0];
    public IReadOnlyList<CellDataMin> Cells => _cellConfigs;

    public bool IsCellFree(Vector2Int cellPosition)
    {
        return !_cellConfigs.Any(c => c.CellPosition == cellPosition);
    }

    public CellDataMin GetCell(Vector2Int cellPosition)
    {
        return _cellConfigs.Where(c => c.CellPosition == cellPosition).FirstOrDefault();
    }

    public bool AddCell(CellDataMin cell)
    {
        if (IsCellFree(cell.CellPosition))
        {
            _cellConfigs = _cellConfigs.Append(cell).ToArray();
            return true;
        }

        return false;
    }

    public void Reset()
    {
        _cellConfigs = new CellDataMin[0];
    }

    public void Remove(Vector2Int cellPosition)
    {
        _cellConfigs = _cellConfigs.Where(c => c.CellPosition != cellPosition).ToArray();
    }
}
