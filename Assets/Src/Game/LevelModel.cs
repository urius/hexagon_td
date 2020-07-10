using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class LevelModel
{
    public bool IsInitialized = false;

    public event Action<UnitModel> StartSpawnUnit = delegate { };

    public readonly IEnumerable<CellDataMin> SpawnCells;
    public readonly CellDataMin GoalCell;
    public readonly WaveModel WaveModel;

    private readonly LevelPathfinderCellsProvider _pathfinderCellsProvider;
    private readonly LevelConfig _levelConfig;
    private readonly List<UnitModel> _unitModels = new List<UnitModel>();
    private readonly Dictionary<(Vector2Int, Vector2Int), IReadOnlyList<Vector2Int>> _cachedPaths = new Dictionary<(Vector2Int, Vector2Int), IReadOnlyList<Vector2Int>>();

    private Dictionary<Vector2Int, UnitModel> _cellOwners = new Dictionary<Vector2Int, UnitModel>();

    public LevelModel(LevelConfig levelConfig)
    {
        _levelConfig = levelConfig;
        WaveModel = new WaveModel(levelConfig.WaveConfigs);

        SpawnCells = _levelConfig.Cells.Where(c => c.CellConfigMin.CellType == CellType.EnemyBase);
        GoalCell = _levelConfig.Cells.Where(c => c.CellConfigMin.CellType == CellType.GoalBase).First();

        _pathfinderCellsProvider = new LevelPathfinderCellsProvider(_levelConfig);
    }

    public UnitModel GetCellOwner(Vector2Int cellPosition)
    {
        _cellOwners.TryGetValue(cellPosition, out var result);
        return result;
    }

    internal void SpawnUnit(Vector2Int cellPosition, UnitConfig unitConfig)
    {
        var path = GetPath(cellPosition);
        var unitModel = new UnitModel(path, unitConfig);
        void OnUnitStateUpdated()
        {
            if (unitModel.State == UnitStateName.Moving)
            {
                _cellOwners.Remove(unitModel.PreviousCellPosition);
                _cellOwners[unitModel.CurrentCellPosition] = unitModel;
            }
            else if (unitModel.State == UnitStateName.Destroing)
            {
                _cellOwners.Remove(unitModel.CurrentCellPosition);
                _unitModels.Remove(unitModel);
                unitModel.StateUpdated -= OnUnitStateUpdated;
            }
        }
        unitModel.StateUpdated += OnUnitStateUpdated;

        _cellOwners[unitModel.CurrentCellPosition] = unitModel;
        _unitModels.Add(unitModel);

        StartSpawnUnit(unitModel);
    }

    public bool IsCellFree(Vector2Int cellPosition)
    {
        return !_cellOwners.TryGetValue(cellPosition, out var _);
    }

    public IReadOnlyList<CellDataMin> Cells => _levelConfig.Cells;

    private IReadOnlyList<Vector2Int> GetPath(Vector2Int startCell)
    {
        var finishCell = GoalCell.CellPosition;
        if (_cachedPaths.TryGetValue((startCell, finishCell), out var path))
        {
            return path;
        }
        else
        {
            path = Pathfinder.FindPath(_pathfinderCellsProvider, startCell, finishCell);
            _cachedPaths[(startCell, finishCell)] = path;
            return path;
        }
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
}
