using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class SelectedLevelData
{
    public event Action<BoosterId> BoosterAdded = delegate { };
    public event Action<BoosterId> BoosterRemoved = delegate { };

    public int LevelIndex = -1;

    public readonly BoosterValues BoosterValues = new BoosterValues();

    private readonly HashSet<BoosterId> _boosterIds = new HashSet<BoosterId>();
    private readonly LevelConfig[] _allLevels;

    public SelectedLevelData(LevelConfig[] allLevels)
    {
        _allLevels = allLevels;
    }

    public LevelConfig LevelConfig => LevelIndex >= 0 ? _allLevels[LevelIndex] : null;
    public BoosterId[] BoosterIds => _boosterIds.ToArray();

    public void ResetBoosters()
    {
        _boosterIds.Clear();
        BoosterValues.Reset();
    }

    public bool IsBoosterSelected(BoosterId boosterId)
    {
        return _boosterIds.Contains(boosterId);
    }

    public bool AddBooster(BoosterId boosterId)
    {
        if (_boosterIds.Add(boosterId))
        {
            BoosterValues.Update(_boosterIds);
            BoosterAdded(boosterId);
            return true;
        }

        return false;
    }

    public bool RemoveBooster(BoosterId boosterId)
    {
        if (_boosterIds.Remove(boosterId))
        {
            BoosterValues.Update(_boosterIds);
            BoosterRemoved(boosterId);
            return true;
        }

        return false;
    }

    internal void AdvanceSelectedLevel()
    {
        LevelIndex++;
        if (LevelIndex > _allLevels.Length - 1)
        {
            LevelIndex = 0;
        }
    }
}

public class BoosterValues
{
    public int StartMoneyMultiplier { get; private set; }
    public float IncreaseRewardMultiplier { get; private set; }
    public int BaseArmorMultiplier { get; private set; }
    public int RepairAfterWavePoints { get; private set; }
    public float TurretReloadTimeMultiplier { get; private set; }
    public float EnemiesSpeedMultiplier { get; private set; }

    public BoosterValues()
    {
        Reset();
    }

    public void Reset()
    {
        StartMoneyMultiplier = 1;
        IncreaseRewardMultiplier = 1;
        BaseArmorMultiplier = 1;
        RepairAfterWavePoints = 0;
        TurretReloadTimeMultiplier = 1;
        EnemiesSpeedMultiplier = 1;
    }

    public void Update(IEnumerable<BoosterId> boosterIds)
    {
        Reset();
        foreach(var boosterId in boosterIds)
        {
            var config = BoostersConfigProvider.GetBoosterConfigById(boosterId);
            UpdateByBoosterConfig(config);
        }
    }

    private void UpdateByBoosterConfig(BoosterConfig config)
    {
        switch(config.BoosterId)
        {
            case BoosterId.StartMoneyX2:
                StartMoneyMultiplier = config.Value;
                break;
            case BoosterId.IncreasedReward:
                IncreaseRewardMultiplier = (100 + config.Value) * 0.01f;
                break;
            case BoosterId.BaseCapacityX2:
                BaseArmorMultiplier = config.Value;
                break;
            case BoosterId.RepairBaseAfterWave_1:
            case BoosterId.RepairBaseAfterWave_10:
                RepairAfterWavePoints += config.Value;
                break;
            case BoosterId.ExtraTurretsPower:
                TurretReloadTimeMultiplier = (100 - config.Value) * 0.01f;
                break;
            case BoosterId.SlowEnemies:
                EnemiesSpeedMultiplier = (100 - config.Value) * 0.01f;
                break;
        }
    }
}
