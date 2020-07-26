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
    public GameObject BuildModePrefab;
    public Sprite IconSprite;
}

public enum TurretType
{
    Gun = 0,
    Laser = 1,
    Rocket = 2,
    SlowField = 3,
}
