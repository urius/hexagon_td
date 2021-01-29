using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class PlayerGlobalModel
{
    private const string SavedFileName = "virus_defence_player_model";

    public LevelProgressDataMin[] LevelsProgress;
    public float AudioVolume = 0.7f;
    public float MusicVolume = 0.7f;
    public float SoundsVolume = 0.7f;

    public static bool TryLoad(out PlayerGlobalModel playerGlobalModel)
    {
        return SaveLoadHelper.TryLoadSerialized(SavedFileName, out playerGlobalModel);
    }

    public PlayerGlobalModel(PlayerGlobalModel original)
    {
        LevelsProgress = original.LevelsProgress;

        UpdateUnlockState();
    }

    public void Save()
    {
        SaveLoadHelper.SaveSerialized(this, SavedFileName);
    }

    public void UpdateUnlockState()
    {
        var lastPassedLevelIndex = -1;
        for (var i = 0; i < LevelsProgress.Length; i++)
        {
            if (LevelsProgress[i].IsPassed)
            {
                lastPassedLevelIndex = i;
            }

            if (lastPassedLevelIndex >= 0 && (i - lastPassedLevelIndex) <= 2)
            {
                LevelsProgress[i].IsUnlocked = true;
            }
        }
    }

    public LevelProgressDataMin GetProgressByLevel(int levelIndex)
    {
        return LevelsProgress[levelIndex];
    }

    public void SetLevelPassed(int levelIndex, int starsAmount)
    {
        LevelsProgress[levelIndex].IsUnlocked = true;
        LevelsProgress[levelIndex].IsPassed = true;
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
    public bool IsPassed;
    public bool IsUnlocked;
    public int StarsAmount;
}

