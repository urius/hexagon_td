using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelUnitsModel
{
    private readonly List<UnitModel> _unitModels = new List<UnitModel>();
    private Dictionary<Vector2Int, UnitModel> _cellOwners = new Dictionary<Vector2Int, UnitModel>();

    public IEnumerable<UnitModel> WaitingUnits => _unitModels.Where(m => m.CurrentState.StateName == UnitStateName.WaitingForCell);

    public UnitModel GetUnitOnCell(Vector2Int cellPosition)
    {
        _cellOwners.TryGetValue(cellPosition, out var result);
        return result;
    }

    public void OwnCellByUnit(UnitModel unitModel)
    {
        _cellOwners[unitModel.CurrentCellPosition] = unitModel;
    }

    public void FreeCell(Vector2Int cellPosition)
    {
        _cellOwners.Remove(cellPosition);
    }

    public void AddUnit(UnitModel unitModel)
    {
        OwnCellByUnit(unitModel);
        _unitModels.Add(unitModel);
    }

    public void RemoveUnit(UnitModel unitModel)
    {
        _cellOwners.Remove(unitModel.CurrentCellPosition);
        _unitModels.Remove(unitModel);
    }

    public bool IsCellWithoutUnit(Vector2Int cellPosition)
    {
        return !_cellOwners.TryGetValue(cellPosition, out var _);
    }
}
