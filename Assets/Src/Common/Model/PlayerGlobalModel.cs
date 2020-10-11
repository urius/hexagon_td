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

        UpdateUnlockState();
    }
    public LevelProgressDataMin[] LevelsProgress;

    public void UpdateUnlockState()
    {
        var lastPassedLevelIndex = -1;
        for (var i = 0; i < LevelsProgress.Length; i++)
        {
            if (LevelsProgress[i].isPassed)
            {
                lastPassedLevelIndex = i;
            }

            if (lastPassedLevelIndex >= 0 && (i - lastPassedLevelIndex) <= 2)
            {
                LevelsProgress[i].isUnlocked = true;
            }
        }
    }

    public LevelProgressDataMin GetProgressByLevel(int levelIndex)
    {
        return LevelsProgress[levelIndex];
    }

    private void AdjustLevelsAmount(int levelsAmount)
    {
        if (levelsAmount > 0 && LevelsProgress.Length < levelsAmount)
        {
            var list = new List<LevelProgressDataMin>(LevelsProgress);
            list.AddRange(new LevelProgressDataMin[levelsAmount - LevelsProgress.Length]);
            LevelsProgress = list.ToArray();
        }
    }
}

[Serializable]
public struct LevelProgressDataMin
{
    public bool isPassed;
    public bool isUnlocked;
    public int StarsAmount;
}