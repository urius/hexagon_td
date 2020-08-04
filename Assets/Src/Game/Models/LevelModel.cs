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

    public LevelModel(LevelConfig levelConfig)
    {
        _levelConfig = levelConfig;
        WaveModel = new WaveModel(levelConfig.WaveConfigs);
        PathsManager = new PathsManager(levelConfig, LevelTurretsModel, LevelUnitsModel);

        SpawnCells = _levelConfig.Cells.Where(c => c.CellConfigMin.CellType == CellType.EnemyBase).ToArray();
        GoalCell = _levelConfig.Cells.Where(c => c.CellConfigMin.CellType == CellType.GoalBase).First();
        _teleportCellPositions = _levelConfig.Cells.Where(c => c.CellConfigMin.CellType == CellType.Teleport).Select(c => c.CellPosition).ToArray();
    }

    public void DispatchTeleporting(Vector2Int previousCellPosition, Vector2Int currentCellPosition)
    {
        Teleporting(previousCellPosition, currentCellPosition);
    }

    public bool IsTeleport(Vector2Int cellPosition)
    {
        return _teleportCellPositions.Any(p => p == cellPosition);
    }

    public bool IsReadyToBuild(Vector2Int cellPosition)
    {
        //TODO: add check for path availability from all spawn cells to any goal cell
        if (IsGround(cellPosition) && !LevelTurretsModel.HaveTurret(cellPosition))
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