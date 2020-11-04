using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerGlobalModel
{
    public static bool TryLoad(out PlayerGlobalModel playerGlobalModel)
    {
        return SaveLoadHelper.TryLoadSerialized("pgm", out playerGlobalModel);
    }

    public void Save()
    {
        SaveLoadHelper.SaveSerialized(this, "pgm");
    }

    public PlayerGlobalModel(PlayerGlobalModel original)
    {
        LevelsProgress = original.LevelsProgress;

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

    public void SetLevelPassed(int levelIndex, int starsAmount)
    {
        LevelsProgress[levelIndex].isUnlocked = true;
        LevelsProgress[levelIndex].isPassed = true;
        LevelsProgress[levelIndex].StarsAmount = starsAmount;
    }

    public void AdjustLevelsAmount(int levelsAmount)
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

