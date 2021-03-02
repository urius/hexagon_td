using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Level", fileName = "LevelConfig")]
public class LevelConfig : ScriptableObject
{
    [SerializeField] public string NameKey;
    [SerializeField] public bool IsTransposed;

    [SerializeField] public int StartMoneyAmount = 500;
    [SerializeField] public int WaveCompletedReward = 150;
    [SerializeField] public int ModifierRepairValue = 5;
    [SerializeField] public int ModifierMineDamage = 5;
    [SerializeField] public int ModifierMoneyAmount = 10;
    [SerializeField] public int ModifierBigMoneyAmount = 30;
    [SerializeField] public int DefaulGoalCapacity = 10;
    [SerializeField] public int DestroyUnitReward = 10;

    [SerializeField] private CellDataMin[] _cellConfigs = new CellDataMin[0];
    public IReadOnlyList<CellDataMin> Cells => _cellConfigs;

    [SerializeField] private CellDataMin[] _modifierConfigs = new CellDataMin[0];
    public IReadOnlyList<CellDataMin> Modifiers => _modifierConfigs;

    [Header("Boosters settings")]
    [SerializeField] public bool IsBoostersDisabled = false;
    [SerializeField] public BoosterId[] SpecialDisabledBoosters = new BoosterId[0];
    
    [Header("Waves settings")]
    [SerializeField] private WaveSetting[] _wavesSettings;
    public WaveSetting[] WavesSettings => _wavesSettings;

    public bool IsBoosterEnabled(BoosterId boosterId)
    {
        return !IsBoostersDisabled && Array.IndexOf(SpecialDisabledBoosters, boosterId) <= -1;
    }

    public bool IsCellFree(Vector2Int cellPosition)
    {
        return !_cellConfigs.Any(c => c.CellPosition == cellPosition);
    }

    public bool IsGround(Vector2Int cellPosition)
    {
        return _cellConfigs.Any(c => c.CellPosition == cellPosition && c.CellConfigMin.CellType == CellType.Ground);
    }

    public bool HasModifier(Vector2Int cellPosition)
    {
        return _cellConfigs.Any(c => c.CellPosition == cellPosition && c.CellConfigMin.CellType == CellType.Modifier);
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

    public bool AddModifier(CellDataMin modifier)
    {
        if (IsGround(modifier.CellPosition))
        {
            _modifierConfigs = _modifierConfigs
                .Where(c => c.CellPosition != modifier.CellPosition)
                .Append(modifier)
                .ToArray();

            return true;
        }

        return false;
    }

    public void RemoveModifier(Vector2Int cellPosition)
    {
        _modifierConfigs = _modifierConfigs
            .Where(c => c.CellPosition != cellPosition)
            .ToArray();
    }

    public void Reset()
    {
        _cellConfigs = new CellDataMin[0];
        _wavesSettings = new WaveSetting[0];
    }

    public void Remove(Vector2Int cellPosition)
    {
        var removingCellConfig = _cellConfigs.Where(c => c.CellPosition == cellPosition);
        _cellConfigs = _cellConfigs.Where(c => c.CellPosition != cellPosition).ToArray();
    }
}

