using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelModel
{
    public event Action<Vector2Int, Vector2Int> Teleporting = delegate { };
    public event Action MoneyAmountUpdated = delegate { };
    public event Action InsufficientMoneyTriggered = delegate { };

    public bool IsInitialized = false;

    public readonly IEnumerable<CellDataMin> SpawnCells;
    public readonly CellDataMin GoalCell;
    public readonly WaveModel WaveModel;
    public readonly LevelTurretsModel LevelTurretsModel = new LevelTurretsModel();
    public readonly LevelUnitsModel LevelUnitsModel = new LevelUnitsModel();
    public readonly PathsManager PathsManager;

    public readonly int ModifierRepairValue;
    public readonly int ModifierMineDamage;
    public readonly int ModifierMoneyAmount;
    public readonly int ModifierBigMoneyAmount;

    private readonly LevelConfig _levelConfig;

    private readonly Vector2Int[] _teleportCellPositions;
    private readonly Dictionary<Vector2Int, ModifierType> _modifiers = new Dictionary<Vector2Int, ModifierType>();

    private int _money;

    public LevelModel(LevelConfig levelConfig)
    {
        _levelConfig = levelConfig;
        WaveModel = new WaveModel(levelConfig.WaveConfigs);

        SpawnCells = _levelConfig.Cells.Where(c => c.CellConfigMin.CellType == CellType.EnemyBase).ToArray();
        GoalCell = _levelConfig.Cells.Where(c => c.CellConfigMin.CellType == CellType.GoalBase).First();
        _teleportCellPositions = _levelConfig.Cells.Where(c => c.CellConfigMin.CellType == CellType.Teleport).Select(c => c.CellPosition).ToArray();
        _modifiers = levelConfig.Modifiers.ToDictionary(c => c.CellPosition, c => (ModifierType)c.CellConfigMin.CellSubType);

        PathsManager = new PathsManager(levelConfig.Cells, _modifiers, LevelTurretsModel, LevelUnitsModel);

        _money = _levelConfig.StartMoneyAmount;
        ModifierRepairValue = _levelConfig.ModifierRepairValue;
        ModifierMineDamage = _levelConfig.ModifierMineDamage;
        ModifierMoneyAmount = _levelConfig.ModifierMoneyAmount;
        ModifierBigMoneyAmount = _levelConfig.ModifierBigMoneyAmount;
    }

    public int Money => _money;

    public void DispatchTeleporting(Vector2Int previousCellPosition, Vector2Int currentCellPosition)
    {
        Teleporting(previousCellPosition, currentCellPosition);
    }

    public bool IsTeleport(Vector2Int cellPosition)
    {
        return _teleportCellPositions.Any(p => p == cellPosition);
    }

    public float GetSpeedMultiplier(Vector2Int cell)
    {
        if (_modifiers.TryGetValue(cell, out var modifier))
        {
            switch (modifier)
            {
                case ModifierType.SpeedUp:
                    return 2;
                case ModifierType.SpeedDown:
                    return 0.5f;
            }
        }

        return 1;
    }

    public bool TryGetModifier(Vector2Int cell, out ModifierType modifier)
    {
        return _modifiers.TryGetValue(cell, out modifier);
    }

    public bool IsReadyToBuild(Vector2Int cellPosition)
    {
        if (IsGround(cellPosition)
            && !HasNoBuildModifier(cellPosition)
            && !LevelTurretsModel.HaveTurret(cellPosition)
            && LevelUnitsModel.IsCellWithoutUnit(cellPosition))
        {
            if (PathsManager.ContainsInCachedPaths(cellPosition))
            {
                foreach (var spawnCell in SpawnCells)
                {
                    if (!PathsManager.IsPathExists(spawnCell.CellPosition, GoalCell.CellPosition, cellPosition))
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        return false;
    }

    public void Update()
    {
        LevelTurretsModel.Update();
    }

    private bool IsGround(Vector2Int cellPosition)
    {
        var cell = _levelConfig.Cells.FirstOrDefault(c => c.CellPosition == cellPosition);
        return cell != null && cell.CellConfigMin.CellType == CellType.Ground;
    }

    private bool HasNoBuildModifier(Vector2Int cellPosition)
    {
        return _modifiers.TryGetValue(cellPosition, out var modifier) && modifier == ModifierType.NoBuild;
    }

    public IReadOnlyList<CellDataMin> Cells => _levelConfig.Cells;
    public IReadOnlyList<CellDataMin> Modifiers => _levelConfig.Modifiers;

    public IReadOnlyList<Vector2Int> GetPath(Vector2Int cellPosition)
    {
        return PathsManager.GetPath(cellPosition, GoalCell.CellPosition);
    }

    public IEnumerable<IReadOnlyList<Vector2Int>> GetPaths()
    {
        return SpawnCells.Select(c => GetPath(c.CellPosition)).ToArray();
    }

    public bool TryAddMoney(int amount)
    {
        if (_money + amount < 0)
        {
            return false;
        }
        _money += amount;
        MoneyAmountUpdated();

        return true;
    }

    public bool TrySubstractMoney(int amount)
    {
        return TryAddMoney(-amount);
    }

    public void TriggerInsufficientMoney()
    {
        InsufficientMoneyTriggered();
    }
}

public class WaveModel
{
    private readonly WaveConfig[] _waveConfigs;

    public WaveModel(WaveConfig[] waveConfigs)
    {
        _waveConfigs = waveConfigs;
    }

    public int WaveIndex { get; private set; }
    public int UnitIndex { get; private set; }
    public bool IsCurrentWaveEmpty => UnitIndex >= CurrentWave.Units.Length;

    private WaveConfig CurrentWave => _waveConfigs[WaveIndex];

    public UnitTypeMin GetUnitAndIncrement()
    {
        var result = CurrentWave.Units[UnitIndex];
        UnitIndex++;
        return result;
    }

    public void Reset()
    {
        WaveIndex = 0;
        UnitIndex = 0;
    }
}