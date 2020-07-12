using System;
using System.Linq;
using UnityEngine;

[CreateAssetMenu(menuName = "Configs/TurretConfigProvider", fileName = "TurretConfigProvider")]
public class TurretConfigProvider : ScriptableObject
{
    public TurretConfig[] Configs;

    public TurretConfig GetConfig(TurretType type, int levelIndex)
    {
        return Configs.FirstOrDefault(c => c.TurretType == type && c.TurretLevelIndex == levelIndex);
    }
}

[Serializable]
public class TurretConfig
{
    public TurretType TurretType;
    public int TurretLevelIndex;
    public GameObject Prefab;
    public Sprite IconSprite;
}

public enum TurretType
{
    Gun,
    Laser,
    Rocket,
    SlowField,
}
