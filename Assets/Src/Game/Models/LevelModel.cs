using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelModel
{
    public event Action<Vector2Int, Vector2Int> Teleporting = delegate { };

    public bool IsInitialized = false;

    public readonly IEnumerable<CellDataMin> SpawnCells;
    public readonly CellDataMin GoalCell;
    public readonly WaveModel WaveModel;
    public readonly LevelTurretsModel LevelTurretsModel = new LevelTurretsModel();
    public readonly LevelUnitsModel LevelUnitsModel = new LevelUnitsModel();
    public readonly PathsManager PathsManager;

    private readonly LevelConfig _levelConfig;

    private readonly Vector2Int[] _teleportCellPositions;
    private readonly Dictionary<Vector2Int, ModifierType> _modifiers = new Dictionary<Vector2Int, ModifierType>();

    public LevelModel(LevelConfig levelConfig)
    {
        _levelConfig = levelConfig;
        WaveModel = new WaveModel(levelConfig.WaveConfigs);

        SpawnCells = _levelConfig.Cells.Where(c => c.CellConfigMin.CellType == CellType.EnemyBase).ToArray();
        GoalCell = _levelConfig.Cells.Where(c => c.CellConfigMin.CellType == CellType.GoalBase).First();
        _teleportCellPositions = _levelConfig.Cells.Where(c => c.CellConfigMin.CellType == CellType.Teleport).Select(c => c.CellPosition).ToArray();
        _modifiers = levelConfig.Modifiers.ToDictionary(c => c.CellPosition, c => (ModifierType)c.CellConfigMin.CellSubType);

        PathsManager = new PathsManager(levelConfig.Cells, _modifiers, LevelTurretsModel, LevelUnitsModel);
    }

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

    public bool IsReadyToBuild(Vector2Int cellPosition)
    {
        if (IsGround(cellPosition) && !LevelTurretsModel.HaveTurret(cellPosition) && LevelUnitsModel.IsCellWithoutUnit(cellPosition))
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

    public IReadOnlyList<CellDataMin> Cells => _levelConfig.Cells;
    public IReadOnlyList<CellDataMin> Modifiers => _levelConfig.Modifiers;

    public IReadOnlyList<Vector2Int> GetPath(Vector2Int cellPosition)
    {
        return PathsManager.GetPath(cellPosition, GoalCell.CellPosition);
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