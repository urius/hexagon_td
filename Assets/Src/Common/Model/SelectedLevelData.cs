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
    }

    public bool IsBoosterSelected(BoosterId boosterId)
    {
        return _boosterIds.Contains(boosterId);
    }

    public bool AddBooster(BoosterId boosterId)
    {
        if (_boosterIds.Add(boosterId))
        {
            BoosterAdded(boosterId);
            return true;
        }

        return false;
    }

    public bool RemoveBooster(BoosterId boosterId)
    {
        if (_boosterIds.Remove(boosterId))
        {
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
