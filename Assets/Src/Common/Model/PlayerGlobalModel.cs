using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class PlayerGlobalModel
{
    public event Action<int> GoldAmountUpdated;
    
    public string Id;
    public int LoadsCount;
    public LevelProgressDataMin[] LevelsProgress;
    public float AudioVolume = 0.7f;
    public float MusicVolume = 0.7f;
    public float SoundsVolume = 0.7f;
    public int Gold
    {
        get
        {
            return int.Parse(Base64Helper.Base64Decode(GoldStr));
        }

        set
        {
            GoldStr = Base64Helper.Base64Encode(value.ToString());
        }
    }
    [SerializeField]
    private string GoldStr;

    public PlayerGlobalModel(PlayerGlobalModel original)
    {
        LevelsProgress = original.LevelsProgress;
        UpdateUnlockState();
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

        UpdateUnlockState();
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

    public void AddGold(int goldAmount)
    {
        var goldBefore = Gold;
        Gold += goldAmount;
        if (Gold < 0)
        {
            Gold = 0;
        }

        GoldAmountUpdated?.Invoke(goldAmount);
    }
}

[Serializable]
public struct LevelProgressDataMin
{
    public bool IsPassed;
    public bool IsUnlocked;
    public int StarsAmount;
}

