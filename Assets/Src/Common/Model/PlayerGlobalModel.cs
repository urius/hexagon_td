using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using UnityEngine;

public class PlayerGlobalModelHolder
{
    private TaskCompletionSource<bool> _modelInnitializedTsc = new TaskCompletionSource<bool>();

    public PlayerGlobalModel PlayerGlobalModel { get; private set; }
    public Task ModelInnitializedTask => _modelInnitializedTsc.Task;

    public void SetModel(PlayerGlobalModel model)
    {
        PlayerGlobalModel = model;
        _modelInnitializedTsc.TrySetResult(true);
    }
}

[Serializable]
public class PlayerGlobalModel
{
    public PlayerGlobalModel(PlayerGlobalModel original, int levelsAmount = -1)
    {
        LevelsProgress = original.LevelsProgress;
        AdjustLevelsAmount(levelsAmount);
    }

    public LevelProgressDataMin[] LevelsProgress;

    private void AdjustLevelsAmount(int levelsAmount)
    {
        if (levelsAmount > 0 && LevelsProgress.Length < levelsAmount)
        {
            var list = new List<LevelProgressDataMin>(LevelsProgress);
            list.AddRange(new LevelProgressDataMin[levelsAmount - LevelsProgress.Length]);
            LevelsProgress = list.ToArray();
        }
    }

    public LevelProgressDataMin GetProgressByLevel(int levelIndex)
    {
        return LevelsProgress[levelIndex];
    }
}

[Serializable]
public struct LevelProgressDataMin
{
    public bool isPassed;
    public bool isUnlocked;
    public int StarsAmount;
}