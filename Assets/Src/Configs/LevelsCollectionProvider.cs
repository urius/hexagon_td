using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "LevelsCollection", menuName = "Configs/LevelsCollectionProvider")]
public class LevelsCollectionProvider : ScriptableObject
{
    public static LevelsCollectionProvider Instance { get; private set; }

    [SerializeField]
    private LevelConfig[] _levels;
    public LevelConfig[] Levels => _levels;

    public void OnEnable()
    {
        Instance = this;
    }

    public int GetLevelIndexByConfig(LevelConfig levelConfig)
    {
        return Array.IndexOf<LevelConfig>(_levels, levelConfig);
    }
}
