using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelUnitsModel
{
    public event Action<Vector2Int> CellOwned = delegate { };
    public event Action<Vector2Int> CellReleased = delegate { };
    public event Action<UnitModel> UnitRemoved = delegate { };

    private readonly List<UnitModel> _unitModels = new List<UnitModel>();
    private Dictionary<Vector2Int, UnitModel> _cellOwners = new Dictionary<Vector2Int, UnitModel>();

    public IEnumerable<UnitModel> WaitingUnits => _unitModels.Where(m => m.CurrentState.StateName == UnitStateName.WaitingForCell);
    public IEnumerable<UnitModel> Units => _unitModels;

    public UnitModel GetUnitOnCell(Vector2Int cellPosition)
    {
        _cellOwners.TryGetValue(cellPosition, out var result);
        return result;
    }

    public void OwnCellByUnit(UnitModel unitModel)
    {
        _cellOwners[unitModel.CurrentCellPosition] = unitModel;

        CellOwned(unitModel.CurrentCellPosition);
    }

    public void FreeCell(Vector2Int cellPosition)
    {
        if (_cellOwners.Remove(cellPosition))
        {
            CellReleased(cellPosition);
        }
    }

    public void AddUnit(UnitModel unitModel)
    {
        OwnCellByUnit(unitModel);
        _unitModels.Add(unitModel);
    }

    public void DestroyUnit(UnitModel unitModel)
    {
        if (unitModel.IsDestroying) return;

        unitModel.SetDestroingState();

        FreeCell(unitModel.CurrentCellPosition);

        if (unitModel.PreviousCellPosition != unitModel.CurrentCellPosition
            && _cellOwners.TryGetValue(unitModel.PreviousCellPosition, out var unitOnCell)
            && unitOnCell == unitModel)
        {
            FreeCell(unitModel.PreviousCellPosition);
        }
        _unitModels.Remove(unitModel);

        UnitRemoved(unitModel);
    }

    public bool IsCellWithoutUnit(Vector2Int cellPosition)
    {
        return !_cellOwners.TryGetValue(cellPosition, out var _);
    }
}
