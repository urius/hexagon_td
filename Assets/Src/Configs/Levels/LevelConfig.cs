﻿using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/Level", fileName = "LevelConfig")]
public class LevelConfig : ScriptableObject
{
    private static WaveConfig[] DefaultWaveConfigs = new WaveConfig[] { new WaveConfig() };

    [SerializeField] public bool IsTransposed;

    [SerializeField] private CellDataMin[] _cellConfigs = new CellDataMin[0];
    public IReadOnlyList<CellDataMin> Cells => _cellConfigs;

    [SerializeField] private CellDataMin[] _modifierConfigs = new CellDataMin[0];
    public IReadOnlyList<CellDataMin> Modifiers => _modifierConfigs;

    [SerializeField] private WaveConfig[] _waveConfigs = DefaultWaveConfigs;
    public WaveConfig[] WaveConfigs => _waveConfigs;

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
            if (cell.CellConfigMin.CellType == CellType.EnemyBase)
            {
                UpdateWaveConfigs();
            }
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

    private void UpdateWaveConfigs()
    {
        var enemeBases = _cellConfigs
            .Where(c => c.CellConfigMin.CellType == CellType.EnemyBase)
            .ToArray();

        foreach (var waveConfig in _waveConfigs)
        {
            var remainingConfigs = waveConfig.WillSpawnFromBaseConfigs.Where(p => enemeBases.Any(b => b.CellPosition == p.BaseCellPosition));
            var newConfigs = enemeBases.Where(b => waveConfig.WillSpawnFromBaseConfigs.All(p => p.BaseCellPosition != b.CellPosition))
                .Select(b => new WillSpawnFromBaseConfig(b.CellPosition));

            waveConfig.WillSpawnFromBaseConfigs = remainingConfigs.Concat(newConfigs).ToArray();
        }
    }

    public void Reset()
    {
        _cellConfigs = new CellDataMin[0];
        _waveConfigs = DefaultWaveConfigs;
        UpdateWaveConfigs();
    }

    public void Remove(Vector2Int cellPosition)
    {
        var removingCellConfig = _cellConfigs.Where(c => c.CellPosition == cellPosition);
        _cellConfigs = _cellConfigs.Where(c => c.CellPosition != cellPosition).ToArray();
        if (removingCellConfig.Any(c => c.CellConfigMin.CellType == CellType.EnemyBase))
        {
            UpdateWaveConfigs();
        }
    }
}

[Serializable]
public class WaveConfig
{
    public UnitTypeMin[] Units;
    public WillSpawnFromBaseConfig[] WillSpawnFromBaseConfigs = new WillSpawnFromBaseConfig[0];
}

[Serializable]
public class WillSpawnFromBaseConfig
{
    public WillSpawnFromBaseConfig(Vector2Int baseCellPosition)
    {
        BaseCellPosition = baseCellPosition;
    }

    public Vector2Int BaseCellPosition;
    public bool WillSpawnFrom = true;
}

