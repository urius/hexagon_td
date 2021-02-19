using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class LevelModel
{
    public event Action<Vector2Int, Vector2Int> Teleporting = delegate { };
    public event Action MoneyAmountUpdated = delegate { };
    public event Action InsufficientMoneyTriggered = delegate { };
    public event Action GoalCountUpdated = delegate { };
    public event Action LevelFinished = delegate { };
    public event Action GameSpeedChanged = delegate { };
    public event Action PauseModeChanged = delegate { };

    public bool IsWon = false;
    public bool IsDefeated = false;

    public readonly IReadOnlyList<CellDataMin> SpawnCells;
    public readonly IReadOnlyList<CellDataMin> GoalCells;
    public readonly WaveModel WaveModel;
    public readonly LevelTurretsModel LevelTurretsModel = new LevelTurretsModel();
    public readonly LevelUnitsModel LevelUnitsModel = new LevelUnitsModel();
    public readonly PathsManager PathsManager;

    public readonly int ModifierRepairValue;
    public readonly int ModifierMineDamage;
    public readonly int ModifierMoneyAmount;

    public readonly int ModifierBigMoneyAmount;
    public readonly int WaveCompletionReward;
    public readonly LevelConfig LevelConfig;

    private readonly Vector2Int[] _teleportCellPositions;
    private readonly Dictionary<Vector2Int, ModifierType> _modifiers = new Dictionary<Vector2Int, ModifierType>();
    private readonly TaskCompletionSource<bool> _levelStartedTsc = new TaskCompletionSource<bool>();

    public LevelModel(LevelConfig levelConfig)
    {
        LevelConfig = levelConfig;
        WaveModel = new WaveModel(levelConfig.WavesSettings);

        SpawnCells = LevelConfig.Cells.Where(c => c.CellConfigMin.CellType == CellType.EnemyBase).ToArray();
        GoalCells = LevelConfig.Cells.Where(c => c.CellConfigMin.CellType == CellType.GoalBase).ToArray();
        _teleportCellPositions = LevelConfig.Cells.Where(c => c.CellConfigMin.CellType == CellType.Teleport).Select(c => c.CellPosition).ToArray();
        _modifiers = levelConfig.Modifiers.ToDictionary(c => c.CellPosition, c => (ModifierType)c.CellConfigMin.CellSubType);

        PathsManager = new PathsManager(levelConfig.Cells, _modifiers, LevelTurretsModel, LevelUnitsModel);

        Money = LevelConfig.StartMoneyAmount;
        ModifierRepairValue = LevelConfig.ModifierRepairValue;
        ModifierMineDamage = LevelConfig.ModifierMineDamage;
        ModifierMoneyAmount = LevelConfig.ModifierMoneyAmount;
        ModifierBigMoneyAmount = LevelConfig.ModifierBigMoneyAmount;
        WaveCompletionReward = LevelConfig.WaveCompletedReward;

        ResetGoalCapacity();
    }

    public int TimeScale { get; private set; } = 1;
    public int Money { get; private set; }
    public int GoalCount { get; private set; }
    public int MaxGoalCapacity => LevelConfig.DefaulGoalCapacity;
    public int DestroyUnitReward => LevelConfig.DestroyUnitReward;
    public Task StartLevelTask => _levelStartedTsc.Task;
    public bool IsLevelFinished => IsWon || IsDefeated;
    public bool IsPaused { get; private set; }

    public void SetTimeScale(int timeScale)
    {
        var isTimeScaleChanged = TimeScale != timeScale;
        TimeScale = timeScale;

        if (isTimeScaleChanged)
        {
            GameSpeedChanged();
        }
    }

    public void SetPauseMode(bool isPaused)
    {
        IsPaused = isPaused;

        PauseModeChanged();
    }

    public int GetAccuracyRate()
    {
        var percent = (float)GoalCount / MaxGoalCapacity;
        if (percent <= 0.3)
        {
            return 1;
        }
        else if (percent < 1)
        {
            return 2;
        }
        return 3;
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
                    var havePath = false;
                    foreach (var goalCell in GoalCells)
                    {
                        if (PathsManager.IsPathExists(spawnCell.CellPosition, goalCell.CellPosition, cellPosition))
                        {
                            havePath = true;
                            break;
                        }
                    }

                    if (!havePath)
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

    public void SetLevelStarted()
    {
        _levelStartedTsc.TrySetResult(true);
    }

    public int GetContinueWavePrice()
    {
        return (WaveModel.WaveIndex + 1) * 100;
    }

    private bool IsGround(Vector2Int cellPosition)
    {
        var cell = LevelConfig.Cells.FirstOrDefault(c => c.CellPosition == cellPosition);
        return cell != null && cell.CellConfigMin.CellType == CellType.Ground;
    }

    private bool HasNoBuildModifier(Vector2Int cellPosition)
    {
        return _modifiers.TryGetValue(cellPosition, out var modifier) && modifier == ModifierType.NoBuild;
    }

    public bool IsTransposed => LevelConfig.IsTransposed;
    public IReadOnlyList<CellDataMin> Cells => LevelConfig.Cells;
    public IReadOnlyList<CellDataMin> Modifiers => LevelConfig.Modifiers;

    public IReadOnlyList<Vector2Int> GetPath(Vector2Int cellPosition)
    {
        var result = PathsManager.GetPath(cellPosition, GoalCells[0].CellPosition);

        if (GoalCells.Count > 1)
        {
            for (var i = 1; i < GoalCells.Count; i++)
            {
                var tempPath = PathsManager.GetPath(cellPosition, GoalCells[i].CellPosition);
                if (tempPath.Count > 0 && (tempPath.Count < result.Count || result.Count <= 0))
                {
                    result = tempPath;
                }
            }
        }

        return result;
    }

    public IReadOnlyList<IReadOnlyList<Vector2Int>> GetPaths()
    {
        return SpawnCells.Select(c => GetPath(c.CellPosition)).ToArray();
    }

    public bool TryAddMoney(int amount)
    {
        if (Money + amount < 0)
        {
            return false;
        }
        Money += amount;
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

    public void SubstractGoalCapacity()
    {
        if (GoalCount <= 0)
        {
            return;
        }
        GoalCount--;
        GoalCountUpdated();

        if (GoalCount == 0)
        {
            WaveModel.TerminateWave();
            SetTimeScale(1);
            FinishLevel(false);
        }
    }

    public void ResetLevel()
    {
        GoalCount = LevelConfig.DefaulGoalCapacity;
        IsWon = IsDefeated = false;
        GoalCountUpdated();

        WaveModel.Reset();
    }

    public void ContinueCurrentWave()
    {
        ResetGoalCapacity();

        WaveModel.StartWave();

        SetPauseMode(false);
    }

    public void FinishLevel(bool isWin)
    {
        IsWon = isWin;
        IsDefeated = !isWin;

        SetPauseMode(true);

        LevelFinished();
    }

    private void ResetGoalCapacity()
    {
        GoalCount = LevelConfig.DefaulGoalCapacity;
        GoalCountUpdated();
    }
}

public class WaveModel
{
    public event Action WaveStateChanged = delegate { };

    private readonly WaveSetting[] _wavesSettings;

    private (UnitTypeMin, UnitSkin)[] _currentWaveUnitsFlattened;

    public WaveModel(WaveSetting[] wavesSettings)
    {
        _wavesSettings = wavesSettings;
    }

    public int WaveIndex { get; private set; } = -1;
    public int UnitIndex { get; private set; }
    public WaveState PreviousWaveState { get; private set; } = WaveState.Undefined;

    private WaveState _waveState = WaveState.BeforeFirstWave;
    public WaveState WaveState { get { return _waveState; } private set { PreviousWaveState = _waveState; _waveState = value; } }
    public bool IsCurrentWaveEmpty => UnitIndex >= _currentWaveUnitsFlattened.Length;
    public int TotalWavesCount => _wavesSettings.Length;

    public (UnitTypeMin, UnitSkin) GetUnitAndIncrement()
    {
        var result = _currentWaveUnitsFlattened[UnitIndex];
        UnitIndex++;
        return result;
    }

    public bool IsBaseAllowedToSpawn(int baseIndex)
    {
        return !_wavesSettings[WaveIndex].DisabledBaseIndices.Contains(baseIndex);
    }

    public bool IsBaseAllowedToSpawnOnNextWave(int baseIndex)
    {
        return !_wavesSettings[WaveIndex + 1].DisabledBaseIndices.Contains(baseIndex);
    }

    public void Reset()
    {
        WaveIndex = -1;
        UpdateInnerData();

        UnitIndex = 0;

        StartWave();
    }

    public void AdvanceWave()
    {
        WaveIndex++;
        UpdateInnerData();

        UnitIndex = 0;
    }

    public void StartWave()
    {
        WaveState = WaveState.InWave;
        WaveStateChanged();
    }

    public void TerminateWave()
    {
        WaveState = WaveState.Terminated;
        WaveStateChanged();
    }

    public void EndWave()
    {
        WaveState = (WaveIndex < TotalWavesCount - 1) ? WaveState.BetweenWaves : WaveState.AfterLastWave;
        WaveStateChanged();
    }

    private void UpdateInnerData()
    {
        var tempUnits = new List<(UnitTypeMin, UnitSkin)>();

        foreach (var wavePart in _wavesSettings[WaveIndex].WaveParts)
        {
            tempUnits.AddRange(Enumerable.Range(0, wavePart.Amount).Select(i => (wavePart.UnitType, wavePart.OverrideSkin)));
        }
        _currentWaveUnitsFlattened = tempUnits.ToArray();
    }
}

public enum WaveState
{
    Undefined,
    BeforeFirstWave,
    InWave,
    BetweenWaves,
    AfterLastWave,
    Terminated,
}