using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;

[Serializable]
public class PlayerGlobalModel
{
    public event Action<int> GoldAmountUpdated;
    public event Action<int> GoldAnimationRequested;

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

    internal void TriggerGoldAnimation(int amount)
    {
        GoldAnimationRequested?.Invoke(amount);
    }

    public PlayerGlobalModel(UserDataDto dto)
    {
        LoadsCount = dto.loads;
        LevelsProgress = FromLevelsProgressDto(dto.levels_progress);
        AudioVolume = dto.settings.audio;
        MusicVolume = dto.settings.music;
        SoundsVolume = dto.settings.sounds;
        GoldStr = dto.gold_str;
    }

    public UserDataDto ToSaveDto()
    {
        return new UserDataDto()
        {
            loads = LoadsCount,
            levels_progress = ToLevelsProgressDto(LevelsProgress),
            settings = GetAudioSettingsDto(),
            gold_str = GoldStr,
        };
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

        UpdateUnlockState();
    }

    public void AddGold(int goldAmount, bool silentMode = false)
    {
        var goldBefore = Gold;
        Gold += goldAmount;
        if (Gold < 0)
        {
            Gold = 0;
        }

        if (!silentMode)
        {
            GoldAmountUpdated?.Invoke(goldAmount);
        }
    }

    public bool TrySpendGold(int amount)
    {
        if (amount <= Gold)
        {
            AddGold(-amount);
            return true;
        }
        return false;
    }

    private LevelProgressDataMin[] FromLevelsProgressDto(IReadOnlyList<LevelProgressDto> levelsProgressDto)
    {
        var result = new LevelProgressDataMin[levelsProgressDto.Count];

        for (var i = 0; i < levelsProgressDto.Count; i++)
        {
            var levelDto = levelsProgressDto[i];
            result[i] = new LevelProgressDataMin()
            {
                IsPassed = levelDto.is_passed,
                IsUnlocked = levelDto.is_unlocked,
                StarsAmount = levelDto.stars_amount,
            };
        }

        return result;
    }

    private LevelProgressDto[] ToLevelsProgressDto(IReadOnlyList<LevelProgressDataMin> levelsProgressItems)
    {
        var resultCount = levelsProgressItems.Count;
        for (var i = 0; i < levelsProgressItems.Count; i++)
        {
            var index = levelsProgressItems.Count - 1 - i;
            if (levelsProgressItems[index].IsDefaultState)
            {
                resultCount = index;
            }
            else
            {
                break;
            }
        }

        var result = new LevelProgressDto[resultCount];
        for (var i = 0; i < resultCount; i++)
        {
            var item = levelsProgressItems[i];
            result[i] = new LevelProgressDto()
            {
                is_passed = item.IsPassed,
                is_unlocked = item.IsUnlocked,
                stars_amount = item.StarsAmount,
            };
        }

        return result;
    }

    private PlayerAudioSettingsDto GetAudioSettingsDto()
    {
        return new PlayerAudioSettingsDto
        {
            audio = AudioVolume,
            music = MusicVolume,
            sounds = SoundsVolume,
        };
    }
}

[Serializable]
public struct LevelProgressDataMin
{
    public bool IsPassed;
    public bool IsUnlocked;
    public int StarsAmount;

    public bool IsDefaultState => IsPassed == false && IsUnlocked == false && StarsAmount == 0;
}

