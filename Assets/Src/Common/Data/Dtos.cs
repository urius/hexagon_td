using System;

[Serializable]
public class UserDataDto
{
    public int loads;
    public LevelProgressDto[] levels_progress;
    public PlayerAudioSettingsDto settings;
    public string gold_str;
}

[Serializable]
public struct PlayerAudioSettingsDto
{
    public float audio;
    public float music;
    public float sounds;
}

[Serializable]
public struct LevelProgressDto
{
    public bool is_passed;
    public bool is_unlocked;
    public int stars_amount;
}